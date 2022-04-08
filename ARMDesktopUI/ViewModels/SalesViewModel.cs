using ARMDesktopUI.Library.Api;
using ARMDesktopUI.Library.Helpers;
using ARMDesktopUI.Library.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndpoint _productEndpoint;
        IConfigHelper _configHelper;
        ISaleEndpoint _saleEndpoint;

        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, ISaleEndpoint saleEndpoint)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }


        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }

        private BindingList<ProductModel> _products;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }


        private int _itemQuentity = 1;

        public int ItemQuentitiy
        {
            get { return _itemQuentity; }
            set
            {
                _itemQuentity = value;
                NotifyOfPropertyChange(() => ItemQuentitiy);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }


        public string SubTotal
        {
            get
            {
                return CalculateSubTotal().ToString("C");
            }
        }

        private decimal CalculateSubTotal()
        {
            decimal subTotal = Cart.Sum(x => x.Product.RetailPrice * x.QuentitiyInCart);
            return subTotal;
        }

        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();
                return total.ToString("C");
            }
        }

        public string Tax
        {
            get
            {
                return CalculateTax().ToString("C");
            }
        }

        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate()/100;
            taxAmount = Cart
                .Where(x => x.Product.IsTaxable)
                .Sum(x => x.Product.RetailPrice * x.QuentitiyInCart * taxRate);
            return taxAmount;
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                if (ItemQuentitiy > 0 && SelectedProduct?.QuentityInStock >= ItemQuentitiy)
                {
                    output = true;
                }
                // Make sure something is selected
                // Make sure there is an item quentity
                return output;
            }
        }

        public void AddToCart()
        {
            CartItemModel existingCartItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

            if (existingCartItem != null)
            {
                existingCartItem.QuentitiyInCart += ItemQuentitiy;
                // HACK - There should be a better way of refreshing the cart display
                Cart.Remove(existingCartItem);
                Cart.Add(existingCartItem);
            }
            else
            {
                CartItemModel cart = new CartItemModel
                {
                    Product = SelectedProduct,
                    QuentitiyInCart = ItemQuentitiy
                };
                Cart.Add(cart);
            }

            SelectedProduct.QuentityInStock -= ItemQuentitiy;
            ItemQuentitiy = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;
                // Make sure something is selected
                return output;
            }
        }

        public void RemoveFromCart()
        {
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);

        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;
                // Make sure something is the cart
                if (Cart.Count > 0)
                {
                    output = true;
                }
                return output;
            }
        }

        public async Task CheckOut()
        {
            // Create a SaleModel and post to the API
            SaleModel sale = new SaleModel();

            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantitiy = item.QuentitiyInCart
                });
            }
            await _saleEndpoint.PostSale(sale);
        }
        
    }
}
