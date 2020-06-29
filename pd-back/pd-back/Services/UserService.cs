using System.Collections.Generic;
using System.Linq;
using PhotoDuel.Models;
using PhotoDuel.Models.Web;
using PhotoDuel.Models.Web.Request;
using PhotoDuel.Models.Web.Response;

namespace PhotoDuel.Services
{
    public class UserService
    {
        private readonly IDbService _dbService;
        private readonly DuelService _duelService;

        public UserService(IDbService dbService, DuelService duelService)
        {
            _dbService = dbService;
            _duelService = duelService;
        }

        public InitResponse Init(InitRequest request)
        {
            var user = LoadUser(request);
            var publicDuels = LoadPublicDuels().ToArray();
            var friendDuels = LoadFriendDuels(request.FriendIds).ToArray();
            var myDuels = LoadMyDuels(request.UserId).ToList();
            var winners = LoadPantheon().ToArray();

            var currentDuel = myDuels.SingleOrDefault(
                d => d.Status != DuelStatus.Finished
                     && (
                         d.Creator.User.Id == request.UserId
                         || d.Opponent != null && d.Opponent.User.Id == request.UserId
                     )
            );

            var message = "";
            // check if voting or load duel by link
            if (!string.IsNullOrEmpty(request.DuelId))
            {
                var reqDuel = _dbService.Collection<Duel>("duels").FirstOrDefault(d => d.Id == request.DuelId);
                if (reqDuel != null)
                {
                    if (request.Vote == Vote.None)
                    {
                        // conflicted current duel
                        if (currentDuel != null && currentDuel.Id != reqDuel.Id)
                        {
                            message = "Сначала завершите текущую дуэль";
                        }
                        // set requested duel as current
                        else
                        {
                            currentDuel = reqDuel;
                        }
                    }
                    else
                    {
                        // try to vote for requested duel
                        if (_duelService.Vote(reqDuel, user.ToMeta(), request.Vote))
                        {
                            message = "Ваш голос засчитан";
                            myDuels.Add(reqDuel);
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
            }

            return new InitResponse
            {
                User = user,
                Duel = currentDuel,
                PublicDuels = publicDuels,
                FriendDuels = friendDuels,
                MyDuels = myDuels.ToArray(),
                Pantheon = winners,
                Message = message
            };
        }

        private IEnumerable<Duel> LoadPublicDuels()
        {
            return _dbService.Collection<Duel>("duels")
                .Where(d => d.Status == DuelStatus.Created && d.Type == DuelType.Public)
                .OrderByDescending(d => d.Creator.Time)
                .Take(100);
        }

        private IEnumerable<Duel> LoadFriendDuels(string[] friendIds)
        {
            return _dbService.Collection<Duel>("duels")
                .Where(
                    d => d.Status == DuelStatus.Created
                         && d.Type == DuelType.Friends
                         && friendIds.Contains(d.Creator.User.Id)
                )
                .OrderByDescending(d => d.Creator.Time);
        }

        private IEnumerable<Duel> LoadMyDuels(string userId)
        {
            return _dbService.Collection<Duel>("duels")
                .Where(
                    d => d.Creator.User.Id == userId
                         || (d.Opponent != null && d.Opponent.User.Id == userId)
                         || d.Creator.Voters.Any(v => v.Id == userId)
                         || (d.Opponent != null && d.Opponent.Voters.Any(v => v.Id == userId))
                )
                .OrderByDescending(d => d.Creator.Time);
        }

        private IEnumerable<Winner> LoadPantheon()
        {
            var duels = _dbService.Collection<Duel>("duels")
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

        private User LoadUser(InitRequest request)
        {
            var user = _dbService.Collection<User>("users").FirstOrDefault(u => u.Id == request.UserId);

            if (user == null)
            {
                user = new User
                {
                    Id = request.UserId,
                    Rating = 0
                };
            }

            user.Name = request.UserName;
            user.Photo = request.UserPhoto;
            
            _dbService.UpdateAsync("users", user);

            return user;
        }
    }
}