using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZStore.Application.Repository.IRepository;
using ZStore.Utility;

namespace ZStore.PL.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                HttpContext.Session.SetInt32(SD.SessionCart, unitOfWork.ShopingCart
                    .GetAll(u=>u.ApplicationUserId==claim.Value).Count());

                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
