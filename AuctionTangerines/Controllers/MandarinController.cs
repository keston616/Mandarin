using AuctionTangerines.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuctionTangerines.Controllers
{
    [Authorize]
    public class MandarinController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MandarinController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mandarins
        public IActionResult Index()
        {
            var mandarins = _context.Mandarins.ToList();
            return View(mandarins);
        }

        // GET: Mandarins/Details/5
        public IActionResult Details(int id)
        {
            var mandarin = _context.Mandarins.Find(id);
            if (mandarin == null) return NotFound();
            return View(mandarin);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Bid(int mandarinId, decimal amount)
        {
            var mandarin = await _context.Mandarins.FindAsync(mandarinId);

            if (mandarin == null || amount <= mandarin.CurrentPrice)
            {
        ModelState.AddModelError("", "Ставка должна быть выше текущей цены.");
                return RedirectToAction("Index");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bid = new Bid
            {
                MandarinId = mandarinId,
                UserId = userId,
                Amount = amount,
                BidTime = DateTime.Now
            };

            mandarin.CurrentPrice = amount;

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();


            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(ClaimTypes.Email, "Аукцион", "Вашу ставку перебили!");

            return RedirectToAction("Index");
        }

    }
}
