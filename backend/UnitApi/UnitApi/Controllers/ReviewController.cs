using Dal.EF;
using Microsoft.AspNetCore.Mvc;
using UnitApi.dto.Review;
using UnitApi.Models;

namespace UnitApi.Controllers
{
    public class ReviewController : Controller
    {
        ApplicationDbContext db;
        public ReviewController(ApplicationDbContext context)
        {
            db = context;
        }
        /// <summary>
        /// Оставить отзыв для товара
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/item/{id}/review")]
        public IActionResult CreateReview(CreateReviewRequest dto, int id)
        {
            Review review = new Review
            {
                UserId = dto.UserId,
                ItemId = dto.ItemId,
                Text = dto.Text,
                Rating = dto.Rating,
            };
            db.Reviews.Add(review);
            var item = db.Items.Find(dto.ItemId);
            if (dto.Rating > 0)
                item.Rating++;
            else 
                item.Rating--;
            db.SaveChanges();
            return Ok(review);
        }
        /// <summary>
        /// Получить все отзывы для товара
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/item/{id}/review")]
        public IActionResult GetReviewsByItemId(int id)
        {
            var reviews = db.Reviews.Where(r => r.ItemId == id).Select(r => new ReviewDto
            {
                Id = r.Id,
                Text = r.Text,
                Rating = r.Rating,
                UserFullName = r.User.FullName
            }).ToArray();
            
            return Ok(reviews);
        }
    }
}
