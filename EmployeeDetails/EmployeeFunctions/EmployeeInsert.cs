using EmployeeDetails.Common;
using EmployeeDetails.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDetails.EmployeeFunctions
{
    public static class  EmployeeInsert
    {
        private static object objEmployeeDetails;
        [FunctionName("EmployeeInsert")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "EmployeeInsert")] HttpRequest req,
            ILogger log)
        {
            string requestBody = null;
            Employee objEmployeeDetails = null;
            MyAzureFunctionResponse objResponse = new MyAzureFunctionResponse();
            ItemResponse<Employee> objInsertResponse = null;
            log.LogInformation("Calling Azure Function -- EmployeeInsert");
            requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            objEmployeeDetails = JsonConvert.DeserializeObject<Employee>(requestBody);
            if(objEmployeeDetails!=null)
            {
                AzureCosmoDBActivity objCosmosDBActivitiy = new AzureCosmoDBActivity();
                await objCosmosDBActivitiy.InitiateConnection();
                objEmployeeDetails.EmployeeGuid = Guid.NewGuid().ToString();
                objInsertResponse = await objCosmosDBActivitiy.SaveNewEmployeeItem(objEmployeeDetails);
                if (objInsertResponse == null)
                {
                    objResponse.ErrorCode = 100;
                    objResponse.Message = $"Error occured while inserting information of employee- {objEmployeeDetails.Name}, {objEmployeeDetails.PhoneNo}";
                    log.LogInformation(objResponse.Message + "Error:" + objInsertResponse.StatusCode);
                }
                else
                {
                    objResponse.ErrorCode = 0;
                    objResponse.Message = "Successfully inserted information.";
                }
            }
            else
            {
                objResponse.ErrorCode = 100;
                objResponse.Message = "Failed to read or extract Employee information from Request body due to bad data.";
                log.LogInformation("Failed to read or extract Employee information from Request body due to bad data.");
            }
            return new OkObjectResult(objResponse);
        }
    }
}
