using System;
using System.Collections.Generic;
using System.Linq;
using PhotoDuel.Models;
using PhotoDuel.Services.Abstract;

namespace PhotoDuel.Services
{
    public class UserService
    {
        private const int DailyChallenges = 3;
        private const int DailyShuffles = 2;
        
        private readonly IDbService _dbService;
        private readonly DuelService _duelService;
        private readonly ContentService _contentService;
        private readonly ISocialService _socialService;
        private readonly Random _random = new Random();

        public UserService(IDbService dbService, DuelService duelService, ContentService contentService, ISocialService socialService)
        {
            _dbService = dbService;
            _duelService = duelService;
            _contentService = contentService;
            _socialService = socialService;
        }

        public User CheckShuffles(User user)
        {
            var today = DateTime.UtcNow.Date.ToUnixTimeMs();
            if (user.LastShuffle < today)
            {
                // full update
                user.ShufflesLeft = DailyShuffles + 1;
                return ShuffleChallenges(user, true);
            }
            else if (user.PublicDuel != null)
            {
                // check current public challenge
                var duel = _dbService.ById<Duel>(user.PublicDuel.Id);
                if (duel != null && duel.Status != DuelStatus.Created)
                {
                    user.PublicDuel = null;
                    _dbService.UpdateAsync(user);
                }
            }

            return user;
        }

        public User ShuffleChallenges(User user, bool changeCategories = false)
        {
            if (user.ShufflesLeft <= 0) return user;
            user.ShufflesLeft--;

            // get category ids
            var categoryIds = changeCategories || user.ChallengeIds.Length < DailyChallenges
                ? _contentService.GetRandomCategoryIds(DailyChallenges)
                : user.ChallengeIds.Select(c => _contentService.GetCategoryByChallenge(c)).ToArray();

            // generate new challenges
            var challenges = categoryIds.Select(c => _contentService.GetRandomChallenge(c)).ToArray();
            user.ChallengeIds = challenges;
            
            // TODO force change
            
            // get public
            var count = _dbService.Collection<Duel>().Count(d => d.IsPublic && d.Status == DuelStatus.Created);
            if (count > 0)
            {
                var randIndex = _random.Next(count);

                var publicDuels = _dbService.Collection<Duel>()
                    .Where(d => d.IsPublic && d.Status == DuelStatus.Created)
                    .Skip(randIndex)
                    .Take(1)
                    .ToList();

                var publicDuel = publicDuels.Count > 0 ? publicDuels.First() : null;
                user.PublicDuel = publicDuel;
            }
            else
            {
                user.PublicDuel = null;
            }

            // update and return
            user.LastShuffle = Utils.Now();
            _dbService.UpdateAsync(user);
            return user;
        }

        public User ShuffleChallenges(string userId)
        {
            var user = _dbService.ById<User>(userId, false);
            return ShuffleChallenges(user);
        }

        public bool TryVote(Duel duel, User user, Vote vote, out string message)
        {
            if (duel == null || vote == Vote.None)
            {
                message = "";
                return false;
            }
            
            if (_duelService.Vote(duel, user.ToMeta(), vote))
            {
                message = "Ваш голос засчитан";
                return true;
            }

            message = "Вы больше не можете голосовать за эту дуэль";
            return false;
        }

        public List<Duel> LoadMyDuels(string userId)
        {
            var myDuels = _dbService.Collection<Duel>()
                .Where(
                    d => d.Creator.User.Id == userId
                         || (d.Opponent != null && d.Opponent.User.Id == userId)
                         || d.Creator.Voters.Any(v => v.Id == userId)
                         || (d.Opponent != null && d.Opponent.Voters.Any(v => v.Id == userId))
                )
                .OrderByDescending(d => d.Creator.Time)
                .ToList();

            return myDuels;
        }

        public IEnumerable<Winner> LoadPantheon()
        {
            var duels = _dbService.Collection<Duel>()
                .Where(d => d.Status == DuelStatus.Finished)
                .OrderByDescending(d => d.Creator.Time)
                .Take(100)
                .ToList();

            var winners = new List<Winner>();
            foreach (var duel in duels)
            {
                // add creator to pantheon if he is winner or there is a tie with non-zero votes 
                if (duel.Creator.Voters.Count > 0 && duel.Creator.Voters.Count >= duel.Opponent.Voters.Count)
                {
                    winners.Add(new Winner(duel.Creator, duel.ChallengeId));
                }
                // add opponent to pantheon if he is winner or there is a tie with non-zero votes 
                if (duel.Opponent.Voters.Count > 0 && duel.Opponent.Voters.Count >= duel.Creator.Voters.Count)
                {
                    winners.Add(new Winner(duel.Opponent, duel.ChallengeId));
                }
            }

            return winners;
        }

        public User LoadUser(string userId)
        {
            var user = _dbService.ById<User>(userId) ?? new User {Id = userId};

            var vkUser = _socialService.GetUser(userId);
            user.Name = vkUser.Name;
            user.Photo = vkUser.Photo;
            
            _dbService.UpdateAsync(user);
            return user;
        }
    }
}