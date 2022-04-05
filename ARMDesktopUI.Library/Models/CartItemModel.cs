using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMDesktopUI.Library.Models
{
    public class CartItemModel
    {
        public ProductModel Product { get; set; }
        public int QuentitiyInCart { get; set; }

        public string DisplayText
        {
            get
            {
                return $"{Product.ProductName} ({QuentitiyInCart})";
            }
        }
    }
}
