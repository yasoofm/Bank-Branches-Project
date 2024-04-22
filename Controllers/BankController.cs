using Bank_Branches_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bank_Branches_Project.Controllers
{
    public class BankController : Controller
    {
        //private static List<BankBranch> branches = new List<BankBranch>();
        public IActionResult Index(string search = "")
        {
            using (BankContext db = new BankContext())
            {
                if(search != "")
                {
                    var branches = db.BankBranches.Where(b => b.LocationName.StartsWith(search)).ToList();
                    return View(branches);
                } else
                {
                    var branches = db.BankBranches.ToList();
                    return View(branches);
                }
            } 
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
                using (BankContext db = new BankContext())
                {
                    BankBranch branch = new BankBranch()
                    {
                        LocationName = form.LocationName,
                        LocationURL = form.LocationURL,
                        BranchManager = form.BranchManager,
                        EmployeeCount = form.EmployeeCount
                    };
                    db.BankBranches.Add(branch);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                } 
            } 
            return View(form);
        }

        public IActionResult Details(int Id) 
        {
            using (BankContext db = new BankContext())
            {
                var branch = db.BankBranches.Include(r=> r.Employees).SingleOrDefault(b => b.Id == Id);
                if(branch == null)
                {
                    return View("Index");
                }
                return View(branch);
            } 
        }

        public IActionResult Edit(int Id)
        {
            using (BankContext db = new BankContext())
            {
                var branch = db.BankBranches.SingleOrDefault(b => b.Id == Id);
                if(branch != null) 
                {
                    var form = new NewBranchForm() { LocationName = branch.LocationName, LocationURL = branch.LocationURL, BranchManager = branch.BranchManager, EmployeeCount = branch.EmployeeCount };
                    return View(form);
                }
            } 
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(int id, NewBranchForm form)
        {
            using (BankContext db = new BankContext())
            {
                var branch = db.BankBranches.Find(id);
                if(branch != null)
                {
                    branch.LocationName = form.LocationName;
                    branch.LocationURL = form.LocationURL;
                    branch.BranchManager = form.BranchManager;
                    branch.EmployeeCount = form.EmployeeCount;
                    db.SaveChanges();
                
                    return RedirectToAction("Index");
                }
            } 
            return View(form);
        }

        public IActionResult Delete(int Id)
        {
            using (BankContext db = new BankContext())
            {
                var branch = db.BankBranches.SingleOrDefault(b =>b.Id == Id);
                if(branch != null)
                {
                    db.BankBranches.Remove(branch);
                    db.SaveChanges();
                }
            } 
            return RedirectToAction("Index");
        }

        public IActionResult AddEmployee(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        public IActionResult AddEmployee(int id,[FromForm] AddEmployeeForm form)
        {
            
            if(ModelState.IsValid)
            {
                try
                {
                    using (BankContext db = new BankContext())
                    {
                        var branch = db.BankBranches.Find(id);
                        Employee employee = new Employee() { Name = form.Name, CivilId = form.CivilId, Position = form.Position, BankBranch = branch};
                
                        //branch.Employees.Add(employee);

                        db.Employees.Add(employee);
                        db.SaveChanges();
                    } 
                    return RedirectToAction("Index");                 
                } catch (DbUpdateException)
                {
                    ModelState.AddModelError("CivilId", "This Civil ID is already in the system.");
                    return View(form);
                }
            }
            return View(form);
        }
    }
}
