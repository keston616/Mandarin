using AuctionTangerines.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionTangerines.Service
{
    public class CleanupMandarinsService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ApplicationDbContext _context;

        public CleanupMandarinsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CleanupMandarins, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private void CleanupMandarins(object state)
        {
            var expiredMandarins = _context.Mandarins.Where(m => m.AuctionEndTime < DateTime.Now && !m.IsSold).ToList();

            foreach (var mandarin in expiredMandarins)
            {
                _context.Mandarins.Remove(mandarin);
            }

            _context.SaveChanges();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
