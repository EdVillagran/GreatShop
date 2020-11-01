using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GreatShop.Models
{
    public class Cart
    {
       
        public Product PID { get; set; }

        [Required]

        public Product Name { get; set; }


        [Required]
        public int Quantity { get; set; }
    }
}
