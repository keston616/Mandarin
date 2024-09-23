using AuctionTangerines.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionTangerines
{
    public class AuctionCheckService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ApplicationDbContext _context;

        public AuctionCheckService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckAuctions, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void CheckAuctions(object state)
        {
            await CheckAuctionEnd();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();

        public async Task CheckAuctionEnd()
        {
            var tangerines = await _context.Mandarins
                .Where(t => t.AuctionEndTime <= DateTime.Now && string.IsNullOrEmpty(t.WinnerEmail))
                .ToListAsync();

            foreach (var tangerine in tangerines)
            {

                if (tangerine.CurrentPrice > 0)
                {
                    tangerine.WinnerEmail = ClaimTypes.Email; 
                    await _context.SaveChangesAsync();

                    EmailService emailService = new EmailService();
                    await emailService.SendWinNotification(tangerine.WinnerEmail, tangerine);
                    emailService.GenerateReceipt(tangerine);
                }
            }
        }
    }
}
