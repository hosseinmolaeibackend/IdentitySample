using System.ComponentModel.DataAnnotations;

namespace IdentitySample.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; } 
        public string? Country { get; set; }
    }
}
