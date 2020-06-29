﻿using System;
using System.Collections.Generic;
using System.Linq;
using PhotoDuel.Models;
using PhotoDuel.Models.Web;

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
            if (vote == Models.Web.Vote.None) throw new InvalidOperationException();
            // user can only vote for running duel
            if (duel.Status != DuelStatus.Started) return false;
            // user cannot vote if he participates is duel
            if (duel.Creator.User.Id == voter.Id || duel.Opponent.User.Id == voter.Id) return false;
            // user cannot vote if he already voted
            if (duel.Creator.Voters.Concat(duel.Opponent.Voters).Any(v => v.Id == voter.Id)) return false;

            // vote for creator
            if (vote == Models.Web.Vote.Creator)
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
            if (duel == null) throw new InvalidOperationException("Duel does not exist");

            // check if duel already has opponent
            if (duel.Opponent != null)
            {
                return new DuelResponse
                {
                    Duel = null
                };
            }

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
    }
}