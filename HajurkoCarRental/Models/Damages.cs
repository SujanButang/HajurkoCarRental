using HajurkoCarRental.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HajurkoCarRental.Models
{
    public class Damages
    {
        public Guid Id { get; set; }

        public Guid CarId { get; set; }
        [ForeignKey("CarId")]
        public Car Car { get; set; }

        public string UserId { get; set; }

        public string DamageDesc { get; set; }

        public int RepairCost { get; set; }

        public string Status { get; set; }

        public string PaymentStatus { get; set; }
        public Damages()
        {
            RepairCost= 0;
            Status = "Pending";
            PaymentStatus = "Pending";
        }

        
    }
}
