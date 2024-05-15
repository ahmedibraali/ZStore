using Microsoft.AspNetCore.Mvc;

using ZStore.Core;
using ZStore.Application.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using ZStore.Utility;

namespace ZStore.PL.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id is null || id == 0)
            {
                return View(category);
            }
            else
            {
                category = unitOfWork.Category.Get(u => u.Id == id);
                return View(category);

            }
            // ViewBag.CategoryList = CategoryList;

        }
        [HttpPost]
        public IActionResult Upsert(Category obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    unitOfWork.Category.Add(obj);
                    TempData["success"] = "Category is created successfully";

                }
                else
                {
                    unitOfWork.Category.Update(obj);
                    TempData["success"] = "Category is updated successfully";
                }

                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {

                return View(obj);
            }
        }
        /*public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category newCategory)
        {
            if (newCategory.Name == newCategory.DisplayOrder.ToString())
                ModelState.AddModelError("name", "Display order can't exactly match the Name");
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Add(newCategory);
                unitOfWork.Save();
                TempData["success"] = "Category is created successfully";
                return RedirectToAction("Index");
            }
            return View(newCategory);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            Category categoryFromDb = unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
                return NotFound();
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category newCategory)
        {
            if (newCategory.Name == newCategory.DisplayOrder.ToString())
                ModelState.AddModelError("name", "Display order can't exactly match the Name");
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Update(newCategory);
                unitOfWork.Save();
                TempData["success"] = "Category is updated successfully";

                return RedirectToAction("Index");
            }
            return View(newCategory);
        }*/

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            Category categoryFromDb = unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
                return NotFound();
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? category = unitOfWork.Category.Get(u => u.Id == id);
            if (category == null)
                return NotFound();
            unitOfWork.Category.Remove(category);
            unitOfWork.Save();
            TempData["success"] = "Category is deleted successfully";

            return RedirectToAction("Index");

        }
    }
}
