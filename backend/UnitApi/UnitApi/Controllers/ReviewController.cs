
using Microsoft.AspNetCore.Mvc;
using UnitApi.dto.Review;
using UnitDal.Models;
using EFCore;
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
        public IActionResult CreateReview([FromBody] CreateReviewRequest dto, int id)
        {
            Review review = new Review
            {
                UserId = dto.UserId,
                ItemId = id,
                Text = dto.Text,
                Rating = dto.Rating,
                IsModerated = 0,
                CreatedTime = DateTime.Now,
            };
            db.Reviews.Add(review);
            var item = db.Items.Find(id);
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
            var reviews = db.Reviews.Where(r => r.ItemId == id && r.IsModerated == 1).Select(r => new ReviewDto
            {
                Id = r.Id,
                Text = r.Text,
                Rating = r.Rating,
                UserFullName = r.User.FullName,
                CreatedTime = r.CreatedTime
            }).ToArray();
            
            return Ok(reviews);
        }
        /// <summary>
        /// Получить все неотмодерированные отзывы
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/review/moderate")]
        public IActionResult GetReviewsForModerate()
        {
            var reviews = db.Reviews.Where(r => r.IsModerated == 0).Select(r => new ReviewDto
            {
                Id = r.Id,
                Text = r.Text,
                Rating = r.Rating,
                UserFullName = r.User.FullName,
                CreatedTime = r.CreatedTime

            }).ToArray();
            return Ok(reviews);
        }

        /// <summary>
        /// Принять отзыв по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/review/{id}/verify")]
        public IActionResult VerifiedReview(int id)
        {
            var review = db.Reviews.Find(id);
            review.IsModerated = 1;
            db.SaveChanges();
            return Ok("review is verify");
        }
        /// <summary>
        /// Отклонить отзыв по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/review/{id}/reject")]
        public IActionResult RejectReview(int id)
        {
            var review = db.Reviews.Find(id);
            review.IsModerated = -1;
            db.SaveChanges();
            return Ok("review is reject");
        }
    }
}
