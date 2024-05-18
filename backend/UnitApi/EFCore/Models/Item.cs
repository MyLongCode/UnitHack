namespace UnitDal.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public double Rating { get; set; }
        public bool InStock { get; set; }
    }
}
