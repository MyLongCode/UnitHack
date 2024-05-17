using Dal.EF;
using Logic.Restaurant.Models;
using Microsoft.AspNetCore.Mvc;
using UnitApi.dto.Item;
using UnitApi.Models;

namespace UnitApi.Controllers
{
    [Route("/item")]
    public class ItemController : Controller
    {
        ApplicationDbContext db;
        public ItemController(ApplicationDbContext context)
        {
            db = context;
        }
        /// <summary>
        /// Получить все товары
        /// </summary>
        /// <remarks>
        ///
        ///
        ///     Сортировки 
        ///     {
        ///        "NameAsc" : По названию по возрастанию алфавита, 
        ///        "NameDesc" : По названию по убыванию алфавита,
        ///        "IdAsc" : Сначала старые,
        ///        "IdDesc": Сначала новые,
        ///        "RatingAsc" : Сначала плохие, 
        ///        "RatingDesc" : Сначала хорошие,
        ///        "CostAsc" : Сначала дешёвые,
        ///        "CostDesc": Сначала дорогие
        ///     }
        /// 
        /// page - страничка для пагинации (вроде выдаёт по 10 :) )
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("/item")]
        public IActionResult GetAllItems([FromQuery] int page = 1, Sort sort = Sort.IdAsc)
        {
            int pageSize = 10;
            if (page <= 0) page = 1;
            var items = db.Items.ToArray();
            items = sort switch
            {
                Sort.NameAsc => items.OrderBy(r => r.Title).ToArray(),
                Sort.NameDesc => items.OrderByDescending(r => r.Title).ToArray(),
                Sort.IdAsc => items.OrderBy(r => r.Id).ToArray(),
                Sort.IdDesc => items.OrderByDescending(r => r.Id).ToArray(),
                Sort.RatingAsc => items.OrderBy(r => r.Rating).ToArray(),
                Sort.RatingDesc => items.OrderByDescending(r => r.Rating).ToArray(),
                Sort.CostAsc => items.OrderBy(r => r.Rating).ToArray(),
                Sort.CostDesc => items.OrderByDescending(r => r.Rating).ToArray(),
            };
            return Ok(items.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }
        /// <summary>
        /// Создать новый товар
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/item")]
        public IActionResult CreateItem(CreateItemRequest dto)
        {
            Item item = new Item {
                Title = dto.Title,
                Description = dto.Description,
                Cost = dto.Cost,
                Image = dto.Image,
                Category = dto.Category,
                Rating = 0
            };
            db.Items.Add(item);
            db.SaveChanges();
            return Ok("Item created");
        }
        /// <summary>
        /// Удалить товар по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("/item")]
        public IActionResult DeleteItem(int id)
        {
            Item item= db.Items.Find(id);
            if (item== null)
                return BadRequest("item undefined");
            db.Items.Remove(item);
            return Ok("item deleted");
        }
        /// <summary>
        /// Получить товар по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/item")]
        public IActionResult GetItemById(int id)
        {
            Item item = db.Items.Find(id);
            if (item == null)
                return BadRequest("item undefined");
            return Ok(item);
        }
    }
}
