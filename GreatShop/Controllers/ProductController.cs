using GreatShop.Data;
using GreatShop.Models;
using GreatShop.Models.ViewModels;
using GreatShop.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GreatShop.Controllers
{
    public class ProductController : Controller
    {
        //Using Dependency Injection to ge instance of ApplicationDbContext & Webhostenviorment
        private readonly ApplicationDbContext _db;

        //
        private readonly IWebHostEnvironment _webHostEnvironment;

        //Populate the property with a constructor 
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult AdminIndex()
        {
            //Retrieve objects from database

            IEnumerable<Product> objList = _db.Product;



            return View(objList);

        }

        public IActionResult Index()
        {
            CategoryVM categoryVM = new CategoryVM()
            {
                Products = _db.Product.Include(u => u.Category),
                Category = _db.Category
            };
            return View(categoryVM);
        }

        //GET--  Upsert-Edit product by taking in current obj.id

        public IActionResult Upsert(int? id)
        {
            /*  For Refference
             *  ViewData or View Bags are used if you want to pass data from controller 
             *  to the view
            
            IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            {
                Text=i.Name,
                Value = i.Id.ToString()
            });
            //To create a viewBag do ViewBag.Name=Name;
            ViewBag.CategoryDropDown = CategoryDropDown;

            */
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };



            if (id == null)
            {
                //this is for create - upsert()
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Product.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();

                }
                return View(productVM);
            }




        }
        //POST
        //Recieves object to add to database as argument
        [HttpPost]

        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    //Creating
                    string upload = webRootPath + WebConstants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                    _db.Product.Add(productVM.Product);
                }
                else
                {
                    //updating
                    var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);

                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WebConstants.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _db.Product.Update(productVM.Product);
                }


                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            productVM.CategorySelectList = _db.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return View(productVM);

        }

        //Recieves id as paramater because we used asp-route-id    Name=id

        //int? means it can also be Null


        /* To Add Edit Feature
         * 1. Add Edit button on index.cshtml
         * 2. On Edit add: asp-controller="Category" asp-route-Id="@obj.Id" asp-action="Edit"
         * 3. In CategoryController add an IActionResult fuction named Edit taking in Id as argu.
         * 4. Check for errors, null or invalid and return Not-Found 
         * 5. If found return that object with View(obj)
         * 6. Create Edit.cshtml in Views
         * 7. On Edit.cshtml add features to edit product
         
         */



        //GET - Edit
        public IActionResult Delete(int? id)
        {


            if (id == null || id == 0)
            {
                return NotFound();

            }

            //Eager Loading, When it loads product loads all related entities as part of query
            Product product = _db.Product.Include(u => u.Category).FirstOrDefault(u => u.Id == id);

            if (product == null)
            {
                return NotFound();

            }
            return View(product);




        }

        [HttpPost]
        [ValidateAntiForgeryToken]  //Validate Post is valid and has not been tampered
        //POST - delete
        public IActionResult DeletePost(int? id)
        {
            /*  Server Side Validation    */
            //Get object from DB using id
            var obj = _db.Product.Find(id);

            if (obj != null)
            {
                string upload = _webHostEnvironment.WebRootPath + WebConstants.ImagePath;
                var oldFile = Path.Combine(upload, obj.Image);

                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }
                //Add object to DB
                _db.Product.Remove(obj);

                //Save changes to DB
                _db.SaveChanges();

                //Rather than returning back to view we want to redirect to Index Page
                return RedirectToAction("Index");
            }
            else
                return NotFound();

        }
        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }



            DetailsVM DetailsVM = new DetailsVM()
            {
                Product = _db.Product.Include(u => u.Category)
                .Where(u => u.Id == id).FirstOrDefault(),
                ExistInCart = false
            };


            foreach (var item in shoppingCartList)
            {
                if (item.ProductId == id)
                {
                    DetailsVM.ExistInCart = true;
                }
            }

            return View(DetailsVM);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart { ProductId = id });

            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
    }
}
