namespace Bank_Branches_Project.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CivilId { get; set; }
        public string Position { get; set; }
        public BankBranch BankBranch { get; set; }
    }
}
