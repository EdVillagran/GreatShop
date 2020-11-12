using GreatShop.Data;
using GreatShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GreatShop.Controllers
{
    public class CategoryController : Controller
    {
        //Using Dependency Injection/ need an instance of ApplicationDbContext
        private readonly ApplicationDbContext _db;

        //Populate the property with a constructor 
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            //Retrieve objects from database

            IEnumerable<Category> objList = _db.Category;
            return View(objList);

        }
        //GET-- Create new category
        public IActionResult Create()
        {
            //Retrieve objects from database

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]  //Validate Post is valid and has not been tampered

        //Recieves object to add to database as argument
        public IActionResult Create(Category obj)
        {
            /*  Server Side Validation    */

            if (ModelState.IsValid)
            {
                //Add object to DB
                _db.Category.Add(obj);

                //Save changes to DB
                _db.SaveChanges();

                //Rather than returning back to view we want to redirect to Index Page
                return RedirectToAction("Index");
            }
            else
                return View(obj);

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
        public IActionResult Edit(int? id)
        {
            //Retrieve that category from database using id
            //then display it to user

            if (id == null || id == 0)
                return NotFound();

            else
            {
                var obj = _db.Category.Find(id);
                if (obj == null)
                    return NotFound();
                return View(obj);
            }



        }

        [HttpPost]
        [ValidateAntiForgeryToken]  //Validate Post is valid and has not been tampered
        //POST - EDIT
        public IActionResult Edit(Category obj)
        {
            /*  Server Side Validation    */

            if (ModelState.IsValid)
            {
                //Add object to DB
                _db.Category.Update(obj);

                //Save changes to DB
                _db.SaveChanges();

                //Rather than returning back to view we want to redirect to Index Page
                return RedirectToAction("Index");
            }
            else
                return View(obj);

        }

        //GET - Edit
        public IActionResult Delete(int? id)
        {
            //Retrieve that category from database using id
            //then display it to user

            if (id == null || id == 0)
                return NotFound();

            else
            {
                var obj = _db.Category.Find(id);
                if (obj == null)
                    return NotFound();
                return View(obj);
            }



        }

        [HttpPost]
        [ValidateAntiForgeryToken]  //Validate Post is valid and has not been tampered
        //POST - delete
        public IActionResult DeletePost(int? id)
        {
            /*  Server Side Validation    */
            //Get object from DB using id
            var obj = _db.Category.Find(id);

            if (obj != null)
            {

                //Add object to DB
                _db.Category.Remove(obj);

                //Save changes to DB
                _db.SaveChanges();

                //Rather than returning back to view we want to redirect to Index Page
                return RedirectToAction("Index");
            }
            else
                return View(obj);

        }

    }
}
