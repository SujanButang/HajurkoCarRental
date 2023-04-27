using HajurkoCarRental.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace HajurkoCarRental.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid CarId { get; set; }

        [ForeignKey("CarId")]
        public Car Car { get; set; }

        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public HajurkoCarRentalUser Users { get; set; }




        public DateTime OrderDate { get; set; }

        public DateTime ReturnDate { get; set; }


        public string Status { get; set; }

        public Order()
        {
            Status = "Available";
        }

    }
}
