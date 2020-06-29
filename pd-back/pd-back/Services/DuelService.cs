using System;
using System.Collections.Generic;
using System.Linq;
using PhotoDuel.Models;
using PhotoDuel.Models.Web.Request;
using PhotoDuel.Models.Web.Response;

namespace PhotoDuel.Services
{
    public class DuelService
    {
        private readonly IDbService _dbService;

        public DuelService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public bool Vote(Duel duel, UserMeta voter, Vote vote)
        {
            // this method should only been invoked with actual vote selected
            if (vote == Models.Web.Request.Vote.None) throw new InvalidOperationException();
            // user can only vote for running duel
            if (duel.Status != DuelStatus.Started) return false;
            // user cannot vote if he participates is duel
            if (duel.Creator.User.Id == voter.Id || duel.Opponent.User.Id == voter.Id) return false;
            // user cannot vote if he already voted
            if (duel.Creator.Voters.Concat(duel.Opponent.Voters).Any(v => v.Id == voter.Id)) return false;

            // vote for creator
            if (vote == Models.Web.Request.Vote.Creator)
            {
                duel.Creator.Voters.Add(voter);
                _dbService.PushAsync<Duel, UserMeta>("duels", duel.Id, d => d.Creator.Voters, voter);
            }
            // vote for opponent
            else
            {
                duel.Opponent.Voters.Add(voter);
                _dbService.PushAsync<Duel, UserMeta>("duels", duel.Id, d => d.Opponent.Voters, voter);
            }
            
            return true;
        }

        public DuelResponse CreateDuel(CreateDuelRequest request)
        {
            var user = _dbService.Collection<User>("users").FirstOrDefault(u => u.Id == request.UserId);
            if (user == null) throw new InvalidOperationException("User does not exist");

            // load current duel if it exists
            var currentDuel = _dbService.Collection<Duel>("duels")
                .FirstOrDefault(
                    d => d.Status != DuelStatus.Finished
                         && (
                             d.Creator.User.Id == user.Id
                             || d.Opponent != null && d.Opponent.User.Id == user.Id
                         )
                );
            
            if (currentDuel != null) throw new InvalidOperationException("There is current duel already");
            if (request.Image.Length > 300) throw new ArgumentException("Image url is too long");
            
            // create new duel object
            var duel = new Duel
            {
                ChallengeId = request.ChallengeId, // TODO check if valid
                // ChallengeText = request.ChallengeText // TODO
                Creator = new Duellist
                {
                    Image = request.Image,
                    Time = Utils.Now(),
                    User = user.ToMeta(),
                    Voters = new List<UserMeta>()
                },
                Opponent = null,
                Status = DuelStatus.Created,
                Type = request.Type,
                TimeStart = 0,
                TimeFinish = 0,
                Id = Utils.RandomString(8)
            };
            
            // write to database
            _dbService.UpdateAsync("duels", duel);

            // return
            return new DuelResponse
            {
                Duel = duel
            };
        }

        public DuelResponse JoinDuel(JoinDuelRequest request)
        {
            // load user
            var user = _dbService.Collection<User>("users").FirstOrDefault(u => u.Id == request.UserId);
            if (user == null) throw new InvalidOperationException("User does not exist");
            
            // load duel
            var duel = _dbService.Collection<Duel>("duels").FirstOrDefault(d => d.Id == request.DuelId);

            // check if duel already has opponent
            if (duel == null || duel.Opponent != null)
            {
                return new DuelResponse
                {
                    Duel = null
                };
            }
            
            // check if own
            if (duel.Creator.User.Id == request.UserId) throw new InvalidOperationException("This is your own duel");

            // create new duel object
            duel.Opponent = new Duellist
            {
                Image = request.Image,
                Time = Utils.Now(),
                User = user.ToMeta(),
                Voters = new List<UserMeta>()
            };
            duel.Status = DuelStatus.Started;
            duel.TimeStart = Utils.Now();
            duel.TimeFinish = Utils.Now() + 24 * 3600000L;
            // TODO notification
            
            // write to db
            _dbService.UpdateAsync("duels", duel);

            // return
            return new DuelResponse
            {
                Duel = duel
            };
        }

        public Duel UpdateStory(string userId, string duelId, string storyUrl)
        {
            // load user
            var user = _dbService.Collection<User>("users").FirstOrDefault(u => u.Id == userId);
            if (user == null) throw new InvalidOperationException("User does not exist");
            // load duel
            var duel = _dbService.Collection<Duel>("duels").FirstOrDefault(d => d.Id == duelId);
            if (duel == null || duel.Status != DuelStatus.Started) return null;
            
            if (duel.Creator.User.Id != userId && duel.Opponent.User.Id != userId)
            {
                throw new InvalidOperationException("This is not a participant of this duel");
            }
            
            // check what user
            if (duel.Creator.User.Id == userId)
            {
                duel.Creator.Story = storyUrl;
            }
            else
            {
                duel.Opponent.Story = storyUrl;
            }
            
            // update db
            _dbService.UpdateAsync("duels", duel);
            return duel;
        }

        public bool DeleteDuel(string userId, string duelId)
        {
            var duel = _dbService.Collection<Duel>("duels").FirstOrDefault(d => d.Id == duelId);
            
            if (duel != null && duel.Creator.User.Id == userId && duel.Status == DuelStatus.Created)
            {
                _dbService.DeleteAsync<Duel>("duels", duel.Id);
                return true;
            }

            return false;
        }

        public bool ReportDuel(string userId, string duelId)
        {
            // load user
            var user = _dbService.Collection<User>("users").FirstOrDefault(u => u.Id == userId);
            if (user == null) throw new InvalidOperationException("User does not exist");
            // load duel
            var duel = _dbService.Collection<Duel>("duels").FirstOrDefault(d => d.Id == duelId);
            if (duel == null) return false;
            
            // create new report
            var report = new Report
            {
                Id = Utils.RandomString(16),
                Duel = duel,
                Reporter = user.ToMeta(),
                Time = Utils.Now()
            };
            
            // write to db
            _dbService.UpdateAsync<Report>("reports", report);

            return true;
        }
    }
}