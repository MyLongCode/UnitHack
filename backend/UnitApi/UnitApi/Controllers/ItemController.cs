﻿
using EFCore;
using Logic.Restaurant.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using UnitApi.dto.Item;
using UnitDal.Models;

namespace UnitApi.Controllers
{
    [Route("/item")]
    public class ItemController : Controller
    {
        IWebHostEnvironment _appEnvironment;
        ApplicationDbContext db;
        public ItemController(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
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
        public IActionResult GetAllItems([FromQuery] string? category, int page = 1, Sort sort = Sort.IdAsc)
        {
            int pageSize = 9;
            if (page <= 0) page = 1;
            var items = db.Items.ToArray();
            if (category != null) items = items.Where(i => i.Category == category).ToArray();
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
            foreach(var item in items)
                item.Image = "http://unithack.somee.com" + "/wwwroot/Files/" + item.Image;
            return Ok(items.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }
        /// <summary>
        /// Создать новый товар
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/item")]
        public IActionResult CreateItem([FromBody] CreateItemRequest dto)
        {
            string path = "";
            IFormFile image = dto.Image;
            if (image != null)
            {
                // путь к папке Files
                path =image.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + "/Files/" + path, FileMode.Create))
                {
                    image.CopyToAsync(fileStream);
                }
            }
            Item item = new Item {
                Title = dto.Title,
                Description = dto.Description,
                Cost = dto.Cost,
                Image = path,
                Category = dto.Category,
                Rating = 0,
                InStock = true
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
            db.SaveChanges();
            return Ok("item deleted");
        }
        /// <summary>
        /// Получить товар по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/item/{id}")]
        public IActionResult GetItemById(int id)
        {
            Item item = db.Items.Find(id);
            if (item == null)
                return BadRequest("item undefined");
            item.Image = "http://unithack.somee.com" + "/wwwroot/Files/" + item.Image;
            return Ok(item);
        }

        /// <summary>
        /// Товара нет в наличии
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/item/{id}/absent")]
        public IActionResult AbsentItem(int id)
        {
            var item = db.Items.Find(id);
            if (item == null) return BadRequest("item is not found");
            item.InStock = false;
            db.SaveChanges();
            return Ok("now item not in stock");
        }
        /// <summary>
        /// Товар в наличии
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/item/{id}/stock")]
        public IActionResult StockItem(int id)
        {
            var item = db.Items.Find(id);
            if (item == null) return BadRequest("item is not found");
            item.InStock = true;
            db.SaveChanges();
            return Ok("now item in stock");
        }
    }
}
