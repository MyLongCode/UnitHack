namespace UnitDal.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ItemId { get; set; }
        public Item Item{ get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public int IsModerated { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
