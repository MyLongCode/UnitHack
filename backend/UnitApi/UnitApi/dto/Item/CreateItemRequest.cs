namespace UnitApi.dto.Item
{
    public class CreateItemRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
    }
}
