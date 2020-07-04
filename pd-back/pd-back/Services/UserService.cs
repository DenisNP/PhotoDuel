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

        public Duel LoadAdditional(string duelId, List<Duel> myDuels, out string message)
        {
            Duel additionalDuel = null;
            message = "";
            if (!string.IsNullOrEmpty(duelId))
            {
                additionalDuel = myDuels.FirstOrDefault(d => d.Id == duelId);
                if (additionalDuel == null)
                {
                    additionalDuel = _dbService.ById<Duel>(duelId);
                    if (additionalDuel == null)
                    {
                        message = "Такой дуэли не существует";
                    }
                }
            }

            return additionalDuel;
        }

        public bool TryVote(Duel duel, User user, Vote vote, out string message)
        {
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