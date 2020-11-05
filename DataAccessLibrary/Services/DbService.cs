using DataAccessLibrary.Models;
using DataAccessLibrary.Views;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace DataAccessLibrary.Services
{
    public class DbService
    {
        private static CosmosClient cosmosClient;
        private static Database database;
        private static Container container;

        private static bool _dbLoaded = false;

        #region Initialize Cosmos Database + Container

        private static async Task CreateDatabaseAsync()
            => database = await cosmosClient
                .CreateDatabaseIfNotExistsAsync(Config.DatabaseName, throughput: 400);

        private static async Task CreateContainerAsync()
            => container = await database
                .CreateContainerIfNotExistsAsync(Config.ContainerName, "/id", 400);

        public static async Task<bool> InitCosmosDbAsync()
        {
            try
            {
                // ConnectionMode.Gateway kringgår ev. brandväggsregler 
                cosmosClient = new CosmosClient(
                    Config.EndpointUri, Config.PrimaryKey, 
                    new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway }
                    );

                await CreateDatabaseAsync();
                await CreateContainerAsync();
                _dbLoaded = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        // Funktion för att kolla så att cosmos db är laddad
        private static async Task WaitForDb()
        {
            while (!_dbLoaded)
                await Task.Delay(1000);
        }

        public static async Task<List<Customer>> GetAllCustomersAsync()
        {
            await WaitForDb();

            var customers = new List<Customer>();
            var query = new QueryDefinition("SELECT * FROM items WHERE items.type = \"customer\" ");
            FeedIterator<Customer> result = container.GetItemQueryIterator<Customer>(query);

            while (result.HasMoreResults)
                foreach (var customer in await result.ReadNextAsync())
                    customers.Add(customer);

            return customers;
        }

        public static async Task AddTicketAsync(Ticket ticket) 
            => await container.CreateItemAsync(ticket);

        public static async Task<List<Ticket>> GetAllTicketsAsync()
        {
            await WaitForDb();

            var tickets = new List<Ticket>();

            var query = new QueryDefinition("SELECT * FROM items WHERE items.type = \"ticket\" ");
            FeedIterator<Ticket> result = container.GetItemQueryIterator<Ticket>(query);

            while (result.HasMoreResults)
                foreach (var ticket in await result.ReadNextAsync())
                    tickets.Add(ticket);

            return tickets;
        }

        public static async Task<Ticket> GetTicketAsync(string id)
        {
            var result = await container.ReadItemAsync<Ticket>(id, new PartitionKey(id));
            return result.Resource;
        }

        public static async Task UpdateTicketAsync(Ticket ticket)
        {
            var result = await container.ReadItemAsync<Ticket>(ticket.Id, new PartitionKey(ticket.Id));

            if (result != null)
                await container.ReplaceItemAsync(ticket, ticket.Id, new PartitionKey(ticket.Id));
        }

        public static async Task UpdateTicketListAsync()
        {
            ViewModel.OpenTickets.Clear();
            ViewModel.ClosedTickets.Clear();

            var tickets = await GetAllTicketsAsync();

            foreach (var ticket in tickets)
            {
                if (ticket.StatusString == "Closed")
                    ViewModel.ClosedTickets.Add(ticket);
                else
                    ViewModel.OpenTickets.Add(ticket);
            }
        }

        public static async Task AddCustomerAsync(Customer customer)
            => await container.CreateItemAsync(customer);
    }
}
