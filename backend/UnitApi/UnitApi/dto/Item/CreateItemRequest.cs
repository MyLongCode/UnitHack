namespace UnitApi.dto.Item
{
    public class CreateItemRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public IFormFile Image { get; set; }
        public string Category { get; set; }
    }
}
