using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Qualification
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Course is required.")]
        public required string Course { get; set; }
        [Range(1900, 2100, ErrorMessage = "Year Passed must be between 1900 and 2100.")]
        public int YearPassed { get; set; }
        [Range(0, 100, ErrorMessage = "Marks Percentage must be between 0 and 100.")]
        public decimal MarksPercentage { get; set; }
        public Employee? Employee { get; set; }
    }
}