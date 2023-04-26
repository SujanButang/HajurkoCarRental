using System.ComponentModel.DataAnnotations.Schema;

namespace HajurkoCarRental.Models
{
    public class Car
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Model { get; set; }

        public string Color { get; set; }

        public string Year { get; set; }

        public int RegistrationNo { get; set; }

        public int DailyRent { get; set; }

        public string Status { get; set; }

        public string PhotoUrl { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
