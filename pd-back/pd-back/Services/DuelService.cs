﻿using System;
using System.Collections.Generic;
using System.Linq;
using PhotoDuel.Models;

namespace PhotoDuel.Services
{
    public class DuelService
    {
        private readonly IDbService _dbService;
        private readonly ContentService _contentService;

        public DuelService(IDbService dbService, ContentService contentService)
        {
            _dbService = dbService;
            _contentService = contentService;
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

        public Duel CreateDuel(string userId, string image, DuelType type, int challengeId)
        {
            var user = _dbService.ById<User>(userId, false);

            // load current duel if it exists
            var currentDuel = _dbService.Collection<Duel>()
                .FirstOrDefault(
                    d => d.Status != DuelStatus.Finished
                         && (
                             d.Creator.User.Id == user.Id
                             || d.Opponent != null && d.Opponent.User.Id == user.Id
                         )
                );
            
            if (currentDuel != null) throw new InvalidOperationException("There is current duel already");
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
                Type = type,
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
            // TODO notification
            
            // write to db
            _dbService.UpdateAsync(duel);

            // return
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