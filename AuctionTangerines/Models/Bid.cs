using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionTangerines.Models
{
    public class Bid
    {
        public int Id { get; set; }
        public int MandarinId { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }

        public virtual Mandarin Mandarin { get; set; }
        public virtual IdentityUser User { get; set; }

    }
}
