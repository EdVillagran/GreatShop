using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GreatShop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]

        public string Name { get; set; }

       
        public string Description { get; set; }
        [Required]

        public int Quantity { get; set; }

        [Required]
        public double Cost { get; set; }

        //Going to try to first store the path of image
        public string Image { get; set; }


        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }

        //Maps Product.CategoryId to Category.Id
        [ForeignKey("CategoryId")]
        //This creates the Foreign Key relation between tables
        public virtual Category Category { get; set; }





    }
}
