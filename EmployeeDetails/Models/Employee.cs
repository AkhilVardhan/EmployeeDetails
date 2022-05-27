using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeDetails.Models
{
    public class Employee
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("DateOfBirth")]
        public string DateOfBirth { get; set; }

        [JsonProperty("PhoneNo")]
        public string PhoneNo { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("EmployeeId")]
        public string EmployeeId { get; set; }

        [JsonProperty("id")]
        public string EmployeeGuid { get; set; }
    }
    public class Employees
    {
        public Employees()
        {
            LstEmployee = new List<Employee>();
        }
        public List<Employee> LstEmployee { get; set; }
    }
}
