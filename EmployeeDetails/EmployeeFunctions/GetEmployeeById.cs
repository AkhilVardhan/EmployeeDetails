using EmployeeDetails.Common;
using EmployeeDetails.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDetails.EmployeeFunctions
{
    public static class GetEmployeeById
    {
        [FunctionName("GetEmployeeById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetEmployeeById/{employeeGuid}/{partitionkey}")] HttpRequest req,
            string employeeGuid, string partitionkey,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function - GetEmployeeById.");

            ItemResponse<Employee> objEmployeeResponse = null;
            log.LogInformation("Calling Azure Function -- GetEmployeeById");
            AzureCosmoDBActivity objCosmosDBActivitiy = new AzureCosmoDBActivity();
            await objCosmosDBActivitiy.InitiateConnection();
            objEmployeeResponse = await objCosmosDBActivitiy.GetEmployeeItem(employeeGuid, partitionkey);
            return new OkObjectResult(objEmployeeResponse.Resource);
        }
    }
}
