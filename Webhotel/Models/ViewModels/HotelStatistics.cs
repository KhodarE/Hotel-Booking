using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webhotel.Models.ViewModels
{
    public class HotelStatistics
    {
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }
        [Display(Name = "Number of Customers")]
        public int CustomerCount { get; set; }
        [Display(Name = "Room ID")]
        public int RoomID { get; set; }
        [Display(Name = "Number of Bookings")]
        public int BookingCount { get; set; }
    }
}
