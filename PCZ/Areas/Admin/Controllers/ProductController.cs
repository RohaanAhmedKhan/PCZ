using PCZ.Models;
using PCZ.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PCZ.Areas.Admin.Controllers
{
    public class ProductController : MasterController
    {
        PCZDbContext db = new PCZDbContext();
        // GET: Admin/Product
        [CheckAuthorization(Roles = "Admin")]
        public ActionResult Index()
        {
            ProductViewModel model = new ProductViewModel()
            {
                CategoryList = GetCategoriesSelectList(null),
                _color = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "Black", Value = "Black" },
                    new SelectListItem { Selected = false, Text = "Red", Value = "Red" },
                    new SelectListItem { Selected = false, Text = "Blue", Value = "Blue" },
                    new SelectListItem { Selected = false, Text = "White", Value = "White" },
                    new SelectListItem { Selected = false, Text = "Fawn", Value = "Fawn" },
                    new SelectListItem { Selected = false, Text = "Brown", Value = "Brown" },
                    new SelectListItem { Selected = false, Text = "Orange", Value = "Orange" },
                    new SelectListItem { Selected = false, Text = "Purple", Value = "Purple" },
                    new SelectListItem { Selected = false, Text = "Green", Value = "Green" },
                    new SelectListItem { Selected = false, Text = "Grey", Value = "Grey" },
                    new SelectListItem { Selected = false, Text = "Yellow", Value = "Yellow" }
                }, "Value", "Text"),
                _condition = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "New", Value = "New" },
                    new SelectListItem { Selected = false, Text = "Like New", Value = "Like New" },
                    new SelectListItem { Selected = false, Text = "Good", Value = "Good" },
                    new SelectListItem { Selected = false, Text = "Used", Value = "Used" },
                    new SelectListItem { Selected = false, Text = "Fair", Value = "Fair" },

                }, "Value", "Text"),

                Action = 2,
                products = db.Product.ToList()
            };

           
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProductViewModel model, HttpPostedFileBase Image)
        {
            Product product = new Product
            {
                Name = model.Name,
                Color = model.color,
                Category = db.DeviceType.Where(x=>x.Id == model.Category).Select(x=> x.Name).FirstOrDefault(),
                Condition = model.condition,
                Description = model.Description,
                ImagePath = saveImage(Image, "Product-" + RandomString(16)),
                Size = model.size,
                Price = model.Price,
                Quantity = model.Quantity
        };
            db.Product.Add(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpGet]
        public ActionResult Edit(int productID)
        {
            Product product = db.Product.Where(x => x.ID == productID).FirstOrDefault();
            ProductViewModel model = new ProductViewModel
            {
                ID = productID,
                Name = product.Name,
                CategoryList = GetCategoriesSelectList(null),
                Category = db.DeviceType.Where(x => x.Name == product.Category).Select(x => x.Id).FirstOrDefault(),
                _color = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "Black", Value = "Black" },
                    new SelectListItem { Selected = false, Text = "Red", Value = "Red" },
                    new SelectListItem { Selected = false, Text = "Blue", Value = "Blue" },
                    new SelectListItem { Selected = false, Text = "White", Value = "White" },
                    new SelectListItem { Selected = false, Text = "Fawn", Value = "Fawn" },
                    new SelectListItem { Selected = false, Text = "Brown", Value = "Brown" },
                    new SelectListItem { Selected = false, Text = "Orange", Value = "Orange" },
                    new SelectListItem { Selected = false, Text = "Purple", Value = "Purple" },
                    new SelectListItem { Selected = false, Text = "Green", Value = "Green" },
                      new SelectListItem { Selected = false, Text = "Grey", Value = "Grey" },
                    new SelectListItem { Selected = false, Text = "Yellow", Value = "Yellow" }
                }, "Value", "Text"),
                _condition = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "New", Value = "New" },
                    new SelectListItem { Selected = false, Text = "Like New", Value = "Like New" },
                    new SelectListItem { Selected = false, Text = "Good", Value = "Good" },
                    new SelectListItem { Selected = false, Text = "Used", Value = "Used" },
                    new SelectListItem { Selected = false, Text = "Fair", Value = "Fair" },

                }, "Value", "Text"),
                CategoryName = product.Category,
                color = product.Color,
                condition = product.Condition,
                Price = product.Price,
                Quantity = product.Quantity,
                Description = product.Description,
                size = product.Size,
                products = db.Product.ToList(),
                ImagePath = product.ImagePath,
                Action =1
            };
            return View("Index",model);
        }

        [HttpPost]
        public ActionResult Edit(ProductViewModel model , HttpPostedFileBase Image)
        {
            Product product = db.Product.Where(x => x.ID == model.ID).FirstOrDefault();
            product.Name = model.Name;
            product.Description = model.Description;
            product.Color = model.color;
            product.Condition = model.condition;
            product.Category = db.DeviceType.Where(x => x.Id == model.Category).Select(x => x.Name).FirstOrDefault();
            product.Quantity = model.Quantity;
            product.Price = model.Price;
            product.Size = model.size;
            product.ImagePath = model.ImagePath;
            if (Image != null)
            {
                string filePath = Server.MapPath("~" + model.ImagePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                product.ImagePath = saveImage(Image, "Product-" + RandomString(16));
            }
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int prdID)
        {
            Product product = db.Product.Where(x => x.ID == prdID).FirstOrDefault();
            if (product.ImagePath != null)
            {
                string filePath = Server.MapPath("~" + product.ImagePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            db.Product.Remove(product);
            db.SaveChanges();
            string dtl = "/Admin/Product/Index";
            return Redirect(dtl);
        }
    }
}