namespace UnitDal.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public string Status { get; set; }
        public string UserPhone { get; set; }
        public string UserFullName { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
