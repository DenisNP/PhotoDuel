using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotoDuel.Models;
using PhotoDuel.Services.Abstract;

namespace PhotoDuel.Services
{
    public class CheckDuelsService : IHostedService
    {
        private readonly ILogger<CheckDuelsService> _logger;
        private readonly IDbService _dbService;
        private readonly DuelService _duelService;
        private readonly ConcurrencyService _concurrencyService;
        private Timer _timer;

        public CheckDuelsService(
            ILogger<CheckDuelsService> logger,
            IDbService dbService,
            DuelService duelService,
            ConcurrencyService concurrencyService
        )
        {
            _logger = logger;
            _dbService = dbService;
            _duelService = duelService;
            _concurrencyService = concurrencyService;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ToBackgroundJob, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            _logger.LogInformation("Background check started");
            return Task.CompletedTask;
        }

        private void ToBackgroundJob(object state = null)
        {
            CheckDuels();
            _concurrencyService.CleanOld();
        }
        
        private void CheckDuels()
        {
            var now = Utils.Now();
            var duels = _dbService.Collection<Duel>()
                .Where(d => d.Status == DuelStatus.Started && d.TimeFinish <= now)
                .ToList();
            
            _duelService.FinishDuels(duels);
            
            // TODO clear old challenges
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogInformation("Background check stopped");
            return Task.CompletedTask;
        }
    }
}