using System.Collections.Generic;
using System.Linq;
using PhotoDuel.Models;
using PhotoDuel.Services.Abstract;

namespace PhotoDuel.Services
{
    public class UserService
    {
        private readonly IDbService _dbService;
        private readonly DuelService _duelService;
        private readonly ISocialService _socialService;

        public UserService(IDbService dbService, DuelService duelService, ISocialService socialService)
        {
            _dbService = dbService;
            _duelService = duelService;
            _socialService = socialService;
        }

        public Duel PreloadAndVote(User user, string duelId, Vote vote, ref Duel currentDuel, out string message)
        {
            message = "";
            var reqDuel = _dbService.ById<Duel>(duelId);
            if (reqDuel != null)
            {
                if (vote == Vote.None)
                {
                    // conflicted current duel
                    if (currentDuel != null && currentDuel.Id != reqDuel.Id)
                    {
                        message = "Сначала завершите текущую дуэль";
                    }
                    // set requested duel as current
                    else if (reqDuel.Status == DuelStatus.Created)
                    {
                        currentDuel = reqDuel;
                    }
                }
                else
                {
                    // try to vote for requested duel
                    if (_duelService.Vote(reqDuel, user.ToMeta(), vote))
                    {
                        message = "Ваш голос засчитан";
                        return reqDuel;
                    }
                    else
                    {
                        message = "Вы больше не можете голосовать за эту дуэль";
                    }
                }
            }
            // requested duel not found
            else
            {
                message = "Дуэли по этой ссылке больше нет";
            }
            
            return null;
        }

        public IEnumerable<Duel> LoadPublicDuels()
        {
            return _dbService.Collection<Duel>()
                .Where(d => d.Status == DuelStatus.Created && d.Type == DuelType.Public)
                .OrderByDescending(d => d.Creator.Time)
                .Take(100);
        }

        public IEnumerable<Duel> LoadFriendDuels(string[] friendIds)
        {
            return _dbService.Collection<Duel>()
                .Where(
                    d => d.Status == DuelStatus.Created
                         && d.Type == DuelType.Friends
                         && friendIds.Contains(d.Creator.User.Id)
                )
                .OrderByDescending(d => d.Creator.Time);
        }

        public (List<Duel> myDuels, Duel currentDuel) LoadMyDuels(string userId)
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
            
            var currentDuel = myDuels.SingleOrDefault(
                d => d.Status != DuelStatus.Finished
                     && (
                         d.Creator.User.Id == userId
                         || d.Opponent != null && d.Opponent.User.Id == userId
                     )
            );

            return (myDuels, currentDuel);
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
            var user = _dbService.ById<User>(userId);

            if (user == null)
            {
                user = new User
                {
                    Id = userId,
                    Rating = 0
                };
            }

            var vkUser = _socialService.GetUser(userId);
            user.Name = vkUser.Name;
            user.Photo = vkUser.Photo;
            
            _dbService.UpdateAsync(user);
            return user;
        }
    }
}