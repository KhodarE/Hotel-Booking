using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models
{
    public class Room
    {
        public int ID { get; set; }
        [Required]
        [RegularExpression(@"^[G1-3]{1}$", ErrorMessage = "Exactly one character of 'G','1', '2' or '3'")]
        public string Level { get; set; }
        [RegularExpression(@"^[G1-3]{1}$", ErrorMessage = "Number of bed should be'1', '2' or '3'")]
        public int BedCount { get; set; }
        [Range(50, 300)]
        public double Price { get; set; }
        public ICollection<Booking> TheBookings { get; set; }
    }
}
