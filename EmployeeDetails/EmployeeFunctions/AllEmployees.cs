using EmployeeDetails.Common;
using EmployeeDetails.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDetails.EmployeeFunctions
{
    public static class AllEmployees
    {
        [FunctionName("AllEmployee")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "AllEmployees")] HttpRequest req,
            ILogger log)
        {
            List<Employee> lstEmployees = null;
            log.LogInformation("Calling Azure Function -- EmployeeGetId");
            AzureCosmoDBActivity objCosmosDBActivitiy = new AzureCosmoDBActivity();
            await objCosmosDBActivitiy.InitiateConnection();
            lstEmployees = await objCosmosDBActivitiy.GetAllEmployees();
            return new OkObjectResult(lstEmployees);
        }
    }
}
