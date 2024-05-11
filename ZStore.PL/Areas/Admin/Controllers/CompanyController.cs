using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZStore.Application.Repository.IRepository;
using ZStore.Core;
using ZStore.Core.ViewModels;
using ZStore.Utility;

namespace ZStore.PL.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CompanyController(IUnitOfWork _unitOfWork, IWebHostEnvironment _webHostEnvironment)
        {
            unitOfWork = _unitOfWork;
            webHostEnvironment = _webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Company> objCategoryList = unitOfWork.Company.GetAll().ToList();

            return View(objCategoryList);
        }
        public IActionResult Upsert(int? id)
        {
           
            if (id is null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                Company companyObj = unitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);

            }
            // ViewBag.CategoryList = CategoryList;

        }
        [HttpPost]
        public IActionResult Upsert(Company companyObj)
        {
            if (ModelState.IsValid)
            {
                
                if (companyObj.Id == 0)
                {
                    unitOfWork.Company.Add(companyObj);
                    TempData["success"] = "Company is created successfully";

                }
                else
                {
                    unitOfWork.Company.Update(companyObj);
                    TempData["success"] = "Company is updated successfully";

                }
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View(companyObj);
            }
        }




        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCategoryList = unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCategoryList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = unitOfWork.Company.Get(u => u.Id == id); 
            if(companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            
            unitOfWork.Company.Remove(companyToBeDeleted);
            unitOfWork.Save();


            return Json(new { success = true, message = "Deleting Successful" });

        }
        #endregion
    }
}
