using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * This is the Proper way to send data from controller to Views. While using a ViewBag or ViewData works
 * Fine, it is not a Strongly Typed View as it should be. To make it we can use a ViewModel.
 */
namespace GreatShop.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
    }
}
