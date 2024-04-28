using Bank_Branches_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Bank_Branches_Project.Controllers
{
    public class BankController : Controller
    {
        private readonly BankContext _bankContext;
       
        public BankController(BankContext bankContext)
        {
            _bankContext = bankContext; 
        }
        public async Task<IActionResult> Index(string search = "")
        {
            using (_bankContext)
            {
                var viewModel = new BankDashboardViewModel();
                if(search != "")
                {
                    viewModel.BranchList = await _bankContext.BankBranches.Where(b => b.LocationName.StartsWith(search)).Include(a => a.Employees).ToListAsync();
                    viewModel.TotalBranches = viewModel.BranchList.Count;
                    var EmployeeCount = 0;
                    foreach(var branch in viewModel.BranchList)
                    {
                        if (viewModel.BranchWithMostEmployees == null || (viewModel.BranchWithMostEmployees.Employees != null && branch.Employees.Count > viewModel.BranchWithMostEmployees.Employees.Count))
                        {
                            viewModel.BranchWithMostEmployees = branch;
                        }
                        if (branch.Employees != null)
                        {
                            EmployeeCount += branch.Employees.Count;
                        }
                    }
                    viewModel.TotalEmployees = EmployeeCount;
                    return View(viewModel);
                } else
                {
                    viewModel.BranchList = await _bankContext.BankBranches.Include(a => a.Employees).ToListAsync();
                    viewModel.TotalBranches = viewModel.BranchList.Count;
                    var EmployeeCount = 0;
                    foreach (var branch in viewModel.BranchList)
                    {
                        if (viewModel.BranchWithMostEmployees == null || (viewModel.BranchWithMostEmployees.Employees != null && branch.Employees.Count > viewModel.BranchWithMostEmployees.Employees.Count))
                        {
                            viewModel.BranchWithMostEmployees = branch;
                        }
                        if(branch.Employees != null)
                        {
                            EmployeeCount += branch.Employees.Count;
                        }                  
                    }
                    viewModel.TotalEmployees = EmployeeCount;

                    return View(viewModel);
                }
            } 
        }

        public IActionResult AddBranch()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBranch(NewBranchForm form)
        {
            if(ModelState.IsValid )
            {
                using (_bankContext)
                {
                    BankBranch branch = new BankBranch()
                    {
                        LocationName = form.LocationName,
                        LocationURL = form.LocationURL,
                        BranchManager = form.BranchManager,
                        EmployeeCount = form.EmployeeCount
                    };
                    await _bankContext.BankBranches.AddAsync(branch);
                    await _bankContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                } 
            } 
            return View(form);
        }

        public async Task<IActionResult> Details(int Id) 
        {
            using (_bankContext)
            {
                var branch = await _bankContext.BankBranches.Include(r=> r.Employees).SingleOrDefaultAsync(b => b.Id == Id);
                if(branch == null)
                {
                    return View("Index");
                }
                return View(branch);
            } 
        }

        public async Task<IActionResult> Edit(int Id)
        {
            using (_bankContext)
            {
                var branch = await _bankContext.BankBranches.SingleOrDefaultAsync(b => b.Id == Id);
                if(branch != null) 
                {
                    var form = new NewBranchForm() { LocationName = branch.LocationName, LocationURL = branch.LocationURL, BranchManager = branch.BranchManager, EmployeeCount = branch.EmployeeCount };
                    return View(form);
                }
            } 
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewBranchForm form)
        {
            using (_bankContext)
            {
                var branch = await _bankContext.BankBranches.FindAsync(id);
                if(branch != null)
                {
                    branch.LocationName = form.LocationName;
                    branch.LocationURL = form.LocationURL;
                    branch.BranchManager = form.BranchManager;
                    branch.EmployeeCount = form.EmployeeCount;
                    await _bankContext.SaveChangesAsync();
                
                    return RedirectToAction("Index");
                }
            } 
            return View(form);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            using (_bankContext)
            {
                var branch = await _bankContext.BankBranches.SingleOrDefaultAsync(b =>b.Id == Id);
                if(branch != null)
                {
                    _bankContext.BankBranches.Remove(branch);
                    await _bankContext.SaveChangesAsync();
                }
            } 
            return RedirectToAction("Index");
        }

        public IActionResult AddEmployee(int id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(int id,[FromForm] AddEmployeeForm form)
        {           
            if(ModelState.IsValid)
            {
                try
                {
                    using (_bankContext)
                    {
                        var branch = await _bankContext.BankBranches.FindAsync(id);

                        if(branch == null)
                        {
                            return RedirectToAction("Index");
                        }

                        Employee employee = new Employee() { Name = form.Name, CivilId = form.CivilId, Position = form.Position, BankBranch = branch };

                        //branch.Employees.Add(employee);

                        await _bankContext.Employees.AddAsync(employee);
                        await _bankContext.SaveChangesAsync();
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
