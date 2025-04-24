namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime DOB { get; set; }
        public required string ContactAddress { get; set; }
        public List<Qualification> Qualifications { get; set; } = new List<Qualification>();
    }
}