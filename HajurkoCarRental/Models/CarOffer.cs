using System.ComponentModel.DataAnnotations.Schema;

namespace HajurkoCarRental.Models
{
    public class CarOffer
    {
        public Guid id { get; set; }

        public Guid CarID { get; set; }
        [ForeignKey("CarID")]
        public Car Car { get; set; }

        public Guid Offer { get; set; }
        [ForeignKey("Offer")]
        public Offers Offers { get; set; }
    }
}
