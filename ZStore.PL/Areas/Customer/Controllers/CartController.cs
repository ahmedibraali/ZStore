using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using ZStore.Application.Repository.IRepository;
using ZStore.Core;
using ZStore.Core.ViewModels;
using ZStore.Utility;

namespace ZStore.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpContextAccessor httpContextAccessor;

        [BindProperty]
        public ShopingCartVM shopingCartVM { get; set; }

        public CartController(IUnitOfWork _unitOfWork, IHttpContextAccessor _httpContextAccessor)
        {
            unitOfWork = _unitOfWork;
            httpContextAccessor = _httpContextAccessor;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shopingCartVM = new()
            {
                ShopingCartList = unitOfWork.ShopingCart.GetAll(u=>u.ApplicationUserId == userId ,
                includeProperties:"Product"),
                OrderHeader = new()
            };
            foreach (var cart in shopingCartVM.ShopingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shopingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(shopingCartVM);
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shopingCartVM = new()
            {
                ShopingCartList = unitOfWork.ShopingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };
            shopingCartVM.OrderHeader.ApplicationUser = unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            shopingCartVM.OrderHeader.Name = shopingCartVM.OrderHeader.ApplicationUser.Name;
            shopingCartVM.OrderHeader.PhoneNumber = shopingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shopingCartVM.OrderHeader.StreetAddress = shopingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shopingCartVM.OrderHeader.City = shopingCartVM.OrderHeader.ApplicationUser.City;
            shopingCartVM.OrderHeader.State = shopingCartVM.OrderHeader.ApplicationUser.State;
            shopingCartVM.OrderHeader.PostalCode = shopingCartVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in shopingCartVM.ShopingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shopingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(shopingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			shopingCartVM.ShopingCartList = unitOfWork.ShopingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product");
			shopingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            shopingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = unitOfWork.ApplicationUser.Get(u => u.Id == userId);
			foreach (var cart in shopingCartVM.ShopingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				shopingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
				// Regular customer account and we need to capture payment
				//// Cycle                         OrderStatus    |   PaymentStatus 
				/// 1- Make Payment                Pending        |   Pending
				/// 2- Order Confirmation          Approved       |   Approved
				/// 3- Processing                  Proccessing    |   Approved
				/// 4- Shipped                     Shipped        |   Approved
                shopingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
				shopingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			}
            else
            {
				// Company user account
				//// Cycle                         OrderStatus    |   PaymentStatus 
				/// 1- Order Confirmation          Approved       |   ApprovedForDelayedPayment
				/// 2- Processing                  Proccessing    |   ApprovedForDelayedPayment
				/// 3- Shipped                     Shipped        |   ApprovedForDelayedPayment
				/// 4- Make Payment                Shipped        |   Approved        
				/// (Company Has 30 days to make payment after order is shipped)
				shopingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
				shopingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
			}
            unitOfWork.OrderHeader.Add(shopingCartVM.OrderHeader);
            unitOfWork.Save();
			foreach(var cart in shopingCartVM.ShopingCartList)
			{
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shopingCartVM.OrderHeader.Id,
                    Price=cart.Price,
                    Count = cart.Count

				};
                unitOfWork.OrderDetail.Add(orderDetail);
                unitOfWork.Save();
			}
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // Regular customer account and we need to capture payment
                //stripe Logic
                var domain = httpContextAccessor.HttpContext?.Request.BaseUrl();
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shopingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment"
                };
                
                foreach (var item in shopingCartVM.ShopingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price*100), //$15.25 => 1525
                            Currency="usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title,

                            }
                        },
                        Quantity=item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new Stripe.Checkout.SessionService();
                Session session = service.Create(options);
                unitOfWork.OrderHeader.UpdateStipePaymentID(shopingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return RedirectToAction(nameof(OrderConfirmation), new {id=shopingCartVM.OrderHeader.Id});
		}
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeader.Get(u=>u.Id==id , includeProperties:"ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // this order by customer
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower()=="paid")
                {
                    unitOfWork.OrderHeader.UpdateStipePaymentID(id, session.Id, session.PaymentIntentId);
                    unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    unitOfWork.Save();
                }
            }
            List<ShopingCart> shopingCarts = unitOfWork.ShopingCart
                .GetAll(u=>u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            unitOfWork.ShopingCart.RemoveRange(shopingCarts);
            unitOfWork.Save();

            return View(id);
        }
		public IActionResult Plus(int CartId)
        {
            var cartFromDb = unitOfWork.ShopingCart.Get(u => u.Id == CartId);
            cartFromDb.Count += 1;
            unitOfWork.ShopingCart.Update(cartFromDb);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int CartId)
        {
            var cartFromDb = unitOfWork.ShopingCart.Get(u => u.Id == CartId);
            if (cartFromDb.Count <= 1)
            {
                //Remove 
                unitOfWork.ShopingCart.Remove(cartFromDb);
            }
            else
            {
            cartFromDb.Count -= 1;
            unitOfWork.ShopingCart.Update(cartFromDb);

            }
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int CartId)
        {
            var cartFromDb = unitOfWork.ShopingCart.Get(u => u.Id == CartId);
            unitOfWork.ShopingCart.Remove(cartFromDb);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        private double GetPriceBasedOnQuantity(ShopingCart shopingCart)
        {
            if (shopingCart.Count <= 50)
                return shopingCart.Product.Price;
            else
            { if (shopingCart.Count<=100)
                    return shopingCart.Product.Price50;
                else
                    return shopingCart.Product.Price100;
            }
        }
    }
}
