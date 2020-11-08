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
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace DataAccessLibrary.Services
{
    public class DbService
    {
        private static CosmosClient cosmosClient;
        private static Database database;
        private static Container container;

        /* -- INITIALIZE DATABASE -- */
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

                return true;
            }
            catch
            {
                return false;
            }
        }


        /* -- CREATE -- */
        public static async Task AddTicketAsync(Ticket ticket)
            => await container.CreateItemAsync(ticket);

        public static async Task AddCustomerAsync(Customer customer)
            => await container.CreateItemAsync(customer);


        /* -- READ -- */
        public static async Task<List<Ticket>> GetClosedTicketsAsync()
        {
            var take = ViewModel.ClientSettings.ClosedTicketTake;
            var tickets = new List<Ticket>();

            var query = new QueryDefinition("SELECT * FROM items"
                                            + " WHERE items.type = \"ticket\""
                                            + " AND items.status = 2"
                                            + " ORDER BY items.created DESC"
                                            + " OFFSET 0 LIMIT " + take.ToString());

            FeedIterator<Ticket> result = container.GetItemQueryIterator<Ticket>(query);

            while (result.HasMoreResults)
                foreach (var ticket in await result.ReadNextAsync())
                    tickets.Add(ticket);

            return tickets;
        }

        public static async Task<List<Ticket>> GetOpenTicketsAsync()
        {
            var tickets = new List<Ticket>();

            var query = new QueryDefinition("SELECT * FROM items"
                                            + " WHERE items.type = \"ticket\""
                                            + " AND items.status != 2"
                                            + " ORDER BY items.created DESC");

            FeedIterator<Ticket> result = container.GetItemQueryIterator<Ticket>(query);

            while (result.HasMoreResults)
                foreach (var ticket in await result.ReadNextAsync())
                    tickets.Add(ticket);

            return tickets;
        }

        public static async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customers = new List<Customer>();
            var query = new QueryDefinition("SELECT * FROM items WHERE items.type = \"customer\" ");
            FeedIterator<Customer> result = container.GetItemQueryIterator<Customer>(query);

            while (result.HasMoreResults)
                foreach (var customer in await result.ReadNextAsync())
                    customers.Add(customer);

            return customers;
        }


        /* -- UPDATE -- */
        public static async Task UpdateClosedTicketsAsync()
        {
            ViewModel.ClosedTickets.Clear();

            var tickets = await GetClosedTicketsAsync();
            foreach (var ticket in tickets)
                ViewModel.ClosedTickets.Add(ticket);
        }

        public static async Task UpdateOpenTicketsAsync()
        {
            ViewModel.OpenTickets.Clear();

            var tickets = await GetOpenTicketsAsync();
            foreach (var ticket in tickets)
                ViewModel.OpenTickets.Add(ticket);
        }

        public static async Task UpdateTicketListAsync()
        {
            await UpdateClosedTicketsAsync();
            await UpdateOpenTicketsAsync();
        }

        public static async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                await container.ReplaceItemAsync(ticket, ticket.Id, new PartitionKey(ticket.Id));
            }
            catch { Debug.WriteLine($"Ticket {ticket.Id} could not be found in Cosmos Db."); }
        }
    }
}
