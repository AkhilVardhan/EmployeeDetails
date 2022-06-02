using EmployeeDetails.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmployeeDetails.Common
{
    public class AzureCosmoDBActivity
    {
        private static readonly string EndpointUri = "https://localhost:8081";

        private static readonly string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        private CosmosClient cosmosClient;

        private Database database;

        private Container container;

        private string databaseId = "EmployeeAzureFunctions";

        private string containerId = "EmployeeDetails";
        internal Task<Employee> objEmployeeDetails;
        private static string emailString;

        public List<Employee> EmployeeId { get; private set; }
        public ItemResponse<Employee> EmployeeResponse { get; private set; }

        public async Task InitiateConnection()
        {
            // Create a new instance of the Cosmos Client 
            //configuring Azure Cosmosdb sql api details
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            await CreateDatabaseAsync();
            await CreateContainerAsync();
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }
        private async Task CreateContainerAsync()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/EmployeeId");
        }
        public async Task<ItemResponse<Employee>> SaveNewEmployeeItem(Employee objEmployee)
        {
            ItemResponse<Employee> employeeResponse = null;
            try
            {
                employeeResponse = await this.container.CreateItemAsync<Employee>(objEmployee, new PartitionKey(objEmployee.EmployeeId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return employeeResponse;
        }
        public async Task<ItemResponse<Employee>> ModifyEmployeeItem(Employee objEmployee)
        {
            ItemResponse<Employee> employeeResponse = null;
            try
            {
                EmployeeResponse = await this.container.ReplaceItemAsync<Employee>(objEmployee, objEmployee.EmployeeGuid, new PartitionKey(objEmployee.EmployeeId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return EmployeeResponse;
        }
        public async Task<ItemResponse<Employee>> GetEmployeeItem(string EmployeeId, string partitionKey)
        {
            ItemResponse<Employee> EmployeeResponse = null;
            try
            {
                EmployeeResponse = await this.container.ReadItemAsync<Employee>(EmployeeId, new PartitionKey(partitionKey));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return EmployeeResponse;
        }
        public static class PhoneNumber
        {
            // Regular expression used to validate a phone number.
            public const string motif = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";

            public static bool IsPhoneNbr(string number)
            {
                if (number != null) return Regex.IsMatch(number, motif);
                else return false;
            }
        }
        public async Task<List<Employee>> GetAllEmployees()
        {
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Employee> queryResultSetIterator = this.container.GetItemQueryIterator<Employee>(queryDefinition);

            List<Employee> lstEmployees = new List<Employee>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Employee> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                lstEmployees = currentResultSet.Select(r => new Employee()
                {
                    Name = r.Name,
                    DateOfBirth = r.DateOfBirth,
                    PhoneNo = r.PhoneNo,
                    Email = r.Email,
                    EmployeeGuid = r.EmployeeGuid,
                    EmployeeId = r.EmployeeId
                }).ToList();

            }
            return lstEmployees;
        }
        public static bool isValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
    }
}
