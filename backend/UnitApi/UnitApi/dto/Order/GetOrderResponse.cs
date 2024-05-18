using UnitDal.Models;

namespace UnitApi.dto.Order
{
    public class GetOrderResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public List<ItemDto> Items { get; set; }
        public int TotalCost { get; set; }
        public string Status { get; set; }
        public string UserPhone { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime CreatedTime { get; set; }
    }
    public class ItemDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public double Rating { get; set; }
    }
}
