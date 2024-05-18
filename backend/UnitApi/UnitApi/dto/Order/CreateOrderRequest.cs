using UnitDal.Models;

namespace UnitApi.dto.Order
{
    public class CreateOrderRequest
    {
        public int? UserId { get; set; }
        public int ItemId { get; set; }
        public string UserPhone { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
