using Microsoft.AspNetCore.Mvc;
using ZStore.Core;
using System.Diagnostics;
using ZStore.Application.Repository.IRepository;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ZStore.Utility;
using Microsoft.AspNetCore.Http;

namespace Store.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork _unitOfWork)
        {
            _logger = logger;
            unitOfWork = _unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = unitOfWork.Product.GetAll(includeProperties:"Category");
            return View(productList);
        }
        public IActionResult Details(int id)
        {
            ShopingCart cart = new()
            {
                Product = unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category"),
                Count =1,
                ProductId=id
        };
            
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShopingCart shopingCart)
        {
            shopingCart.Id = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shopingCart.ApplicationUserId = userId;

            ShopingCart cartFromDb = unitOfWork.ShopingCart.Get(u=>u.ApplicationUserId == userId &&
            u.ProductId==shopingCart.ProductId);
            if (cartFromDb != null)
            {
                //exist => update count
                cartFromDb.Count += shopingCart.Count;
                unitOfWork.ShopingCart.Update(cartFromDb);
                unitOfWork.Save();
            }
            else
            {
                //add cart record

                unitOfWork.ShopingCart.Add(shopingCart);
                unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                     unitOfWork.ShopingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Cart was updated successfuly";
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
