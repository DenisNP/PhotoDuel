using System;
using System.Collections.Generic;
using System.Linq;
using PhotoDuel.Models;
using PhotoDuel.Services.Abstract;

namespace PhotoDuel.Services
{
    public class DuelService
    {
        private readonly IDbService _dbService;
        private readonly ContentService _contentService;
        private readonly ISocialService _socialService;

        public DuelService(IDbService dbService, ContentService contentService, ISocialService socialService)
        {
            _dbService = dbService;
            _contentService = contentService;
            _socialService = socialService;
        }
        
        public Duel LoadAdditional(string userId, Vote vote, string duelId, List<Duel> myDuels, out string message)
        {
            // load duel by id
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
            
            // check if add to my
            var hasCurrent = myDuels.Count(Duel.IsCurrentDuelOf(userId).Compile()) > 0;
            if (hasCurrent && vote == Models.Vote.None)
            {
                message = "Сначала завершите текущую дуэль";
                myDuels.Add(additionalDuel);
            }

            return additionalDuel;
        }

        public bool Vote(Duel duel, UserMeta voter, Vote vote)
        {
            // this method should only been invoked with actual vote selected
            if (vote == Models.Vote.None) throw new InvalidOperationException();
            // user can only vote for running duel
            if (duel.Status != DuelStatus.Started) return false;
            // user cannot vote if he participates is duel
            if (duel.Creator.User.Id == voter.Id || duel.Opponent.User.Id == voter.Id) return false;
            // user cannot vote if he already voted
            if (duel.Creator.Voters.Concat(duel.Opponent.Voters).Any(v => v.Id == voter.Id)) return false;

            // vote for creator
            if (vote == Models.Vote.Creator)
            {
                duel.Creator.Voters.Add(voter);
                _dbService.PushAsync<Duel, UserMeta>(duel.Id, d => d.Creator.Voters, voter);
            }
            // vote for opponent
            else
            {
                duel.Opponent.Voters.Add(voter);
                _dbService.PushAsync<Duel, UserMeta>(duel.Id, d => d.Opponent.Voters, voter);
            }
            
            return true;
        }

        public Duel CreateDuel(string userId, string image, int challengeId)
        {
            var user = _dbService.ById<User>(userId, false);

            // load current duel if it exists
            var currentDuel = _dbService.Collection<Duel>().FirstOrDefault(Duel.IsCurrentDuelOf(userId));
            
            if (currentDuel != null && !currentDuel.IsPublic) throw new InvalidOperationException("There is current duel already");
            if (image.Length > 300) throw new ArgumentException("Image url is too long");
            if (!_contentService.HasChallengeId(challengeId)) throw new ArgumentException("Wrong challengeId");
            
            // create new duel object
            var duel = new Duel
            {
                ChallengeId = challengeId,
                // ChallengeText = request.ChallengeText // TODO
                Creator = new Duellist
                {
                    Image = image,
                    Time = Utils.Now(),
                    User = user.ToMeta(),
                    Voters = new List<UserMeta>()
                },
                Opponent = null,
                Status = DuelStatus.Created,
                IsPublic = false,
                TimeStart = 0,
                TimeFinish = 0,
                Id = Utils.RandomString(8)
            };
            
            // write to database
            _dbService.UpdateAsync(duel);

            // return
            return duel;
        }

        public Duel JoinDuel(string userId, string duelId, string image)
        {
            // load user
            var user = _dbService.ById<User>(userId, false);
            // load duel
            var duel = _dbService.ById<Duel>(duelId);
            // check if duel already has opponent
            if (duel == null || duel.Opponent != null)
            {
                return null;
            }
            
            // check image
            if (image.Length > 300) throw new ArgumentException("Image url is too long");
            // check if own
            if (duel.Creator.User.Id == userId) throw new InvalidOperationException("This is your own duel");
            
            // get current duel
            var currentDuel = _dbService.Collection<Duel>().FirstOrDefault(Duel.IsCurrentDuelOf(userId));
            if (currentDuel != null) throw new InvalidOperationException("There is current duel already");

            // create new duel object
            duel.Opponent = new Duellist
            {
                Image = image,
                Time = Utils.Now(),
                User = user.ToMeta(),
                Voters = new List<UserMeta>()
            };
            duel.Status = DuelStatus.Started;
            duel.TimeStart = Utils.Now();
            duel.TimeFinish = Utils.Now() + 24 * 3600000L;
            
            // notify creator
            _socialService.Notify(
                new[] {duel.Creator.User.Id},
                $"{user.Name} принял ваш вызов. К барьеру, господа! Зайдите, чтобы опубликовать голосование."
            );
            
            // write to db
            _dbService.UpdateAsync(duel);

            // return
            return duel;
        }

        public Duel MakePublic(string userId, string duelId)
        {
            // current duels
            var currentDuels = _dbService.Collection<Duel>().Where(Duel.IsCurrentDuelOf(userId)).ToList();
            var hasPublic = currentDuels.Count(d => d.IsPublic) > 0;
            if (hasPublic) throw new InvalidOperationException("There is already public duel");
            
            // load duel
            var duel = _dbService.ById<Duel>(duelId, false);

            // check if valid to set public
            if (duel.Status != DuelStatus.Created || duel.Creator.User.Id != userId) return null;
            if (duel.IsPublic) return duel;

            // set duel to public
            duel.IsPublic = true;
            _dbService.UpdateAsync(duel);
            return duel;
        }

        public Duel UpdateStory(string userId, string duelId, string storyUrl)
        {
            // load duel
            var duel = _dbService.ById<Duel>(duelId);
            if (duel == null || duel.Status != DuelStatus.Started) return null;
            
            if (duel.Creator.User.Id != userId && duel.Opponent.User.Id != userId)
            {
                throw new InvalidOperationException("This is not a participant of this duel");
            }
            
            // check what user
            if (duel.Creator.User.Id == userId) duel.Creator.Story = storyUrl;
            else duel.Opponent.Story = storyUrl;

            // update db
            _dbService.UpdateAsync(duel);
            return duel;
        }

        public void FinishDuels(List<Duel> duels)
        {
            foreach (var duel in duels)
            {
                duel.Status = DuelStatus.Finished;
                _dbService.UpdateAsync(duel);

                var dName = _contentService.GetChallengeName(duel.ChallengeId);
                var message = $"Господа, в дуэли \"{dName}\"";
                if (duel.Creator.Voters.Count > duel.Opponent.Voters.Count)
                {
                    // creator wins
                    message += $" побеждает {duel.Creator.User.Name}";
                } 
                else if (duel.Creator.Voters.Count < duel.Opponent.Voters.Count)
                {
                    // opponent wins
                    message += $" побеждает {duel.Opponent.User.Name}";
                } 
                else if (duel.Creator.Voters.Count > 0)
                {
                    // tie
                    message += " ничья";
                }
                else
                {
                    // not happened
                    message += " участники мирно разошлись";
                }

                // send messages
                var allIds = duel.Creator.Voters.Select(x => x.Id)
                    .Concat(duel.Opponent.Voters.Select(x => x.Id))
                    .Append(duel.Creator.User.Id)
                    .Append(duel.Opponent.User.Id);
                
                _socialService.Notify(allIds.ToArray(), message, duel.Id);
            }
        }

        public bool DeleteDuel(string userId, string duelId)
        {
            var duel = _dbService.ById<Duel>(duelId);
            if (duel == null || duel.Creator.User.Id != userId || duel.Status != DuelStatus.Created) return false;
            
            _dbService.DeleteAsync<Duel>(duel.Id);
            return true;
        }

        public bool ReportDuel(string userId, string duelId)
        {
            // load user
            var user = _dbService.ById<User>(userId, false);
            // load duel
            var duel = _dbService.ById<Duel>(duelId);
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
            _dbService.UpdateAsync(report);
            return true;
        }
    }
}