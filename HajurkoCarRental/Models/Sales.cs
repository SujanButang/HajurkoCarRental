using HajurkoCarRental.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HajurkoCarRental.Models
{
    public class Sales
    {
        public Guid Id { get; set; }

        public Guid CarId { get; set; }

        [ForeignKey("CarId")]
        public Car Car { get; set; }

        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int RentalDuration { get; set; }

        public int RentalCharges { get; set; }

        public string ApprovedBy { get; set; }
        [ForeignKey("ApprovedBy")]
        public HajurkoCarRentalUser HajurkoCarRentalUser { get; set; }
    }
}
