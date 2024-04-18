using Bank_Branches_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Branches_Project.Controllers
{
    public class BankController : Controller
    {
        private static List<BankBranch> branches = new List<BankBranch>();
        public IActionResult Index()
        {
            return View(branches);
        }

        public IActionResult AddBranch()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBranch(NewBranchForm form)
        {
            if(ModelState.IsValid )
            {
                BankBranch branch = new BankBranch()
                {
                    Id = branches.Count + 1,
                    LocationName = form.LocationName,
                    LocationURL = form.LocationURL,
                    BranchManager = form.BranchManager,
                    EmployeeCount = form.EmployeeCount
                };
                branches.Add(branch);
                return RedirectToAction("Index");
            } 
            return View(form);
        }

        public IActionResult Details(int Id) 
        {
            var branch = branches.SingleOrDefault(b => b.Id == Id);
            if(branch == null)
            {
                return View("Index");
            }
            return View(branch);
        }
    }
}
