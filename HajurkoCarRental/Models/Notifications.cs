using HajurkoCarRental.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HajurkoCarRental.Models
{
    public class Notifications
    {
        public Guid Id { get; set; }

        public string To { get; set; }
        [ForeignKey("To")]
        public HajurkoCarRentalUser Users { get; set; }

        public string NotificationType { get; set; }
    }
}
