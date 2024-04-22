namespace Bank_Branches_Project.Models
{
    public class BankBranch
    {
        
        public int Id { get; set; }
        public string LocationName { get; set; }
        public string LocationURL { get; set; }
        public string BranchManager {  get; set; }
        public int EmployeeCount { get; set; }
        public List<Employee> Employees { get; set;}
    }
}
