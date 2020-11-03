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

namespace DataAccessLibrary.Services
{
    public class DbService
    {
        private static ObservableCollection<Ticket> _openTickets = ViewModel.OpenTickets;
        private static ObservableCollection<Ticket> _closedTickets = ViewModel.ClosedTickets;

        private static CosmosClient cosmosClient;
        private static Database database;
        private static Container container;

        #region Initialize Cosmos Database + Container

        private static async Task CreateDatabaseAsync()
            => database = await cosmosClient
                .CreateDatabaseIfNotExistsAsync(Config.DatabaseName, throughput: 400);

        private static async Task CreateContainerAsync()
            => container = await database
                .CreateContainerIfNotExistsAsync(Config.ContainerName, "/id", 400);

        public static async Task InitCosmosDbAsync()
        {
            // ConnectionMode.Gateway kringgår ev. brandväggsregler 
            cosmosClient = new CosmosClient(
                Config.EndpointUri, Config.PrimaryKey, 
                new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway }
                );

            await CreateDatabaseAsync();
            await CreateContainerAsync();
        }

        #endregion

        // Funktion för att vänta på InitCosmosDbAsync
        private static async Task WaitForDb()
        {
            while (container == null)
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

        public static async Task UpdateTicketStatusAsync(string id, Ticket.TicketStatus status)
        {
            var result = await container.ReadItemAsync<Ticket>(id, new PartitionKey(id));

            if (result != null)
            {
                var ticket = result.Resource;
                ticket.Status = status;

                await container.ReplaceItemAsync(ticket, ticket.Id, new PartitionKey(ticket.Id));
            }
        }

        public static async Task UpdateTicketList()
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
    }
}
