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

/* Ärendehanteringssystem
 * Meny till vänster
 *  - Navigera mellan lista & skapa nytt ärende
 *  
 * Information i ärende:
 *  Tidpunkt
 *  Kunden
 *  Kategorisering av ärende
 *  Ärendets status
 *  
 * Läsa inställningsfil i json
 *  Hur många avslutade ärenden som ska visas i listan?
 *  Ej påbörjade, aktiv, avslutad
 * 
 * Tryck på ärende => få upp detaljer
 * 
 * Sparas i en databaslösning
 *  SQL?
 *  
 * Gå att uppdatera ett ärende!
 * Ett sparat ärende ska inte kunna tas bort från systemet
 *  
 *  - VG -
 * Kommentera ärenden
 * Ladda upp bilder
 *  associera med ärendet
 */


namespace UwpApp
{
    public sealed partial class MainPage : Page
    {
        TicketCreationViewModel ticketCreationViewModel = new TicketCreationViewModel();
        TicketListViewModel ticketListViewModel = new TicketListViewModel();

        DataGrid ticketDataGrid => ticketListViewModel.ticketDataGrid;
        TextBlock ticketListHeader => ticketListViewModel.ticketListHeader;

        private bool _dbConnected = false;

        public MainPage()
        {
            InitializeComponent();
            ViewModel.mainPage = this;

            InitDbAsync().GetAwaiter();
        }

        private async Task InitDbAsync()
        {
            if (await DbService.InitCosmosDbAsync())
            {
                ViewModel.customers = await DbService.GetAllCustomersAsync();
                await DbService.UpdateTicketListAsync(); 

                tbLoadingCosmos.Visibility = Visibility.Collapsed;
                tbLoadingBlob.Visibility = Visibility.Visible;
            }
            else
                tbLoadingCosmos.Text = "Could not connect to Cosmos DB!";

            // Kör denna en gång för att skapa customers i CosmosDb
            // await CreateMockCustomers();

            if (await BlobService.InitStorageAsync())
            {
                tbLoadingBlob.Visibility = Visibility.Collapsed;
                _dbConnected = true;
            }
            else
                tbLoadingBlob.Text = "Could not connect to Azure Blob Storage!";
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
