using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebHotel.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required, Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z'-]{2,20}", ErrorMessage = "Surname must only consist of English letters, hyphens, apostrophe's and must be between 2 and 20 characters")]
        public string Surname { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z'-]{2,20}", ErrorMessage = "Family Name must only consist of English letters, hyphens, apostrophe's and must be between 2 and 20 characters")]
        public string Givenname { get; set; }
        [Required]
        [RegularExpression(@"^[0-8]{1}[0-9]{3}", ErrorMessage = "Post Code should be 4 digits e.g. 2164 and not begin with 9")]
        public string Postcode { get; set; }
        public ICollection<Booking> TheBookings { get; set; }
    }
}