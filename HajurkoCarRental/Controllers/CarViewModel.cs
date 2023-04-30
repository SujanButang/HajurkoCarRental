using HajurkoCarRental.Models;

namespace HajurkoCarRental.Controllers
{
    public class CarViewModel
    {
        public List<Car> CarsInOffers { get; set; }
        public List<Car> CarsNotInOffers { get; set; }
    }
}