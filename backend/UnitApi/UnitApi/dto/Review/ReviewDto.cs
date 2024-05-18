using UnitDal.Models;

namespace UnitApi.dto.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string UserFullName { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
