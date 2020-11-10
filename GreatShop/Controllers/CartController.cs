using GreatShop.Data;
using GreatShop.Models;
using GreatShop.Models.ViewModels;
using GreatShop.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GreatShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty] //So you dont have to take it as a paramater
        public ProductUserVM ProductUserVM { get; set; }

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }


        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;


        }
        //GET

        public IActionResult Index()
        {

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();



            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                //Items Exist so 
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).ToList();
            }
            //Set Quantity

            //This only passes the product ID. I need ProductID + Quantity
            List<int> productInCart = shoppingCartList.Select(i => i.ProductId).ToList();


            //Create IENumerable of product list where id matches id in product cart
            IEnumerable<Product> productList = _db.Product.Where(u => productInCart.Contains(u.Id));


            //Have to find a way to return product list + quantity in shopping cart
            //OOOORRR Be able to  have Product Quantity

            var quantities = new ArrayList();
            for (int i = 0; i < productList.Count(); i++)
            {
                quantities.Add(1);
            }

            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                //Populate using eager loading 
                Product = new Product(),
                Products = productList,
                Quantities=quantities

        };

            return View(shoppingCartVM);
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Index")]
    public IActionResult IndexPost()
    {

        return RedirectToAction(nameof(Summary));
    }


    //Add the Quantity Here
    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;

        //Claim is coming null if no one loggs in
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        //var userId = User.FindFirstValue(ClaimTypes.Name); //Anyther way of doing it

        List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
        if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
            && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
        {
            //session exsits
            shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
        }

        List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
        IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));

        if (claim != null)
        {


            ProductUserVM = new ProductUserVM()
            {
                AppUser = _db.AppUser.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = prodList.ToList()
            };


            return View(ProductUserVM);
        }
        return View();
    }

    public IActionResult Remove(int id)
    {
        List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
        if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
            && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
        {
            //Items Exist so 
            shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).ToList();
        }
        //Remove Item from Cart
        shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));

        HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);


        return RedirectToAction(nameof(Index));
    }

    //POST- For Submit
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Summary")]

    public async Task<IActionResult> SummaryPostAsync()  // Dont need (ProductUserVM productUserVM) because of [BindProperty]
    {
        //1 Send Email, then comfirmation page

        //Access template
        var PathtoTemplates = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
            + "Templates" + Path.DirectorySeparatorChar.ToString() + "OrderInquiry.html";

        var subject = "Order Summary";
        string HtmlBody = "";
        using (StreamReader sr = System.IO.File.OpenText(PathtoTemplates))
        {
            HtmlBody = sr.ReadToEnd();
        }
        //Name: { 0}
        //Email: { 1}
        //Phone: { 2}
        //Products: {3}

        //To build the body of messaage
        StringBuilder productListSB = new StringBuilder();

        foreach (var prod in ProductUserVM.ProductList)
        {
            productListSB.Append($" - Name: { prod.Name} <span style='font-size:14px;'> (ID: {prod.Id})</span><br />");
        }

        string messageBody = string.Format(HtmlBody,
            ProductUserVM.AppUser.FullName,
            ProductUserVM.AppUser.Email,
            ProductUserVM.AppUser.PhoneNumber,
            productListSB.ToString());
            

        //SendEmailAsync takes in RecievingEmail, Subject, Message as paramaters
        await _emailSender.SendEmailAsync(WebConstants.EmailAdmin, subject, messageBody);
        //Add Another to User

        return RedirectToAction(nameof(OrderConfirmation));
    }
    public IActionResult OrderConfirmation()
    {
        HttpContext.Session.Clear();
        return View();
    }


}
}
