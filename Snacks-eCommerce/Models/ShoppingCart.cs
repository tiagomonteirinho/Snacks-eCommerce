using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snacks_eCommerce.Models
{
    public class ShoppingCart
    {
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Total { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }
    }
}
