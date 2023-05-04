namespace HajurkoCarRental.Controllers
{
    internal class CarHistoryModel
    {
        public Guid CarId { get; set; }
        public string CarMake { get; set; }
        public string CarModel { get; set; }
        public string CarPhoto { get; set; }
        public int NumOrders { get; set; }
    }
}