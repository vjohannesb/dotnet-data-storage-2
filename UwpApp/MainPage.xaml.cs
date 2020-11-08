using DataAccessLibrary;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DataAccessLibrary.Models;
using DataAccessLibrary.Views;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Storage;
using DataAccessLibrary.Services;
using DataAccessLibrary.Settings;
using Windows.Storage.AccessCache;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;
using Newtonsoft.Json;
using Windows.ApplicationModel;
using System.Linq.Expressions;


namespace UwpApp
{
    public sealed partial class MainPage : Page
    {
        private TicketCreationViewModel ticketCreationViewModel = new TicketCreationViewModel();
        private TicketListViewModel ticketListViewModel = new TicketListViewModel();

        private DataGrid ticketDataGrid => ticketListViewModel.ticketDataGrid;
        private TextBlock ticketListHeader => ticketListViewModel.ticketListHeader;

        private StorageFolder _installedFolder = Package.Current.InstalledLocation;
        private bool _dbConnected = false;

        public MainPage()
        {
            InitializeComponent();
            ViewModel.mainPage = this;

            InitSettingsAsync().GetAwaiter();
            InitDbAsync().GetAwaiter();
        }

        private async Task InitSettingsAsync()
        {
            try
            {
                ViewModel.ClientSettings = JsonConvert
                    .DeserializeObject<ClientSettings>
                    (await FileIO.ReadTextAsync
                    (await _installedFolder
                    .GetFileAsync("settings.json")));
            }
            catch
            {
                Debug.WriteLine("Settings file could not be read. Using standard settings.");
            }
        }

        private async Task InitDbAsync()
        {
            // Azure Blob Storage (måste vara först pga Tickets DownloadAttachmentIfNotExist)
            if (await BlobService.InitStorageAsync())
            {
                tbLoadingBlob.Visibility = Visibility.Collapsed;
                tbLoadingCosmos.Visibility = Visibility.Visible;
            }
            else
                tbLoadingBlob.Text = "Could not connect to Azure Blob Storage!";


            // Azure Cosmos DB
            if (await DbService.InitCosmosDbAsync())
            {
                ViewModel.Customers = await DbService.GetAllCustomersAsync();

                if (ViewModel.Customers.Count < 3)
                {
                    await CreateMockCustomers();
                    ViewModel.Customers = await DbService.GetAllCustomersAsync();
                }

                await DbService.UpdateTicketListAsync(); 

                tbLoadingCosmos.Visibility = Visibility.Collapsed;
                _dbConnected = true;
            }
            else
                tbLoadingCosmos.Text = "Could not connect to Cosmos DB!";

        }

        private static async Task CreateMockCustomers()
        {
            await DbService.AddCustomerAsync(new Customer("Anders", "Andersson"));
            await DbService.AddCustomerAsync(new Customer("Bertil", "Bertilsson"));
            await DbService.AddCustomerAsync(new Customer("Carl", "Carlsson"));
        }

        private void btnOpenTickets_Click(object sender, RoutedEventArgs e)
        {
            if (_dbConnected)
            {
                ticketListHeader.Text = "Open tickets";
                ticketDataGrid.ItemsSource = ViewModel.OpenTickets;
                DataContext = ticketListViewModel;
            }
        }

        private void btnClosedTickets_Click(object sender, RoutedEventArgs e)
        {
            if (_dbConnected)
            {
                ticketListHeader.Text = "Closed tickets";
                ticketDataGrid.ItemsSource = ViewModel.ClosedTickets;
                DataContext = ticketListViewModel;
            }
        }

        private void btnCreateTicket_Click(object sender, RoutedEventArgs e)
        {
            if (_dbConnected)
                DataContext = ticketCreationViewModel;
        }
    }
}
