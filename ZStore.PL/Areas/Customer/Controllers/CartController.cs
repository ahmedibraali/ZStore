using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZStore.Application.Repository.IRepository;
using ZStore.Core;
using ZStore.Core.ViewModels;

namespace ZStore.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ShopingCartVM shopingCartVM { get; set; }

        public CartController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shopingCartVM = new()
            {
                ShopingCartList = unitOfWork.ShopingCart.GetAll(u=>u.ApplicationUserId == userId ,
                includeProperties:"Product")

            };
            foreach (var cart in shopingCartVM.ShopingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shopingCartVM.OrderTotal += (cart.Price * cart.Count);
            }
            return View(shopingCartVM);
        }
        public IActionResult Summary()
        {
            return View();
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
