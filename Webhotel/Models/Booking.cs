using System;
using System.ComponentModel.DataAnnotations;

namespace WebHotel.Models
{
    public class Booking
    {
        public int ID { get; set; }
        public int RoomID { get; set; }
        public string CustomerEmail { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CheckIn { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CheckOut { get; set; }
        [DataType(DataType.Currency)]
        public double Cost { get; set; }
        public Room TheRoom { get; set; }
        public Customer TheCustomer { get; set; }
    }
}