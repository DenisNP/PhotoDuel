using System;
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
    }
}