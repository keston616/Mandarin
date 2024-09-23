using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionTangerines.Models
{
    public class Mandarin
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime AuctionEndTime { get; set; }
        public bool IsSold { get; set; } = false;
        public string WinnerEmail { get; set; } 

    }
}
