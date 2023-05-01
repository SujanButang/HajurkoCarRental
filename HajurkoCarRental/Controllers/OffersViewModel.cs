namespace HajurkoCarRental.Controllers
{
    internal class OffersViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime Validity { get; set; }
    }
}