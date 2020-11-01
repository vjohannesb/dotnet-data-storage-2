using DataAccessLibrary;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UwpApp.Views;
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

        DataGrid ticketDataGrid = TicketListViewModel.ticketDataGrid;
        TextBlock ticketListHeader = TicketListViewModel.ticketListHeader;


        public MainPage()
        {
            this.InitializeComponent();

        }

        private void btnOpenTickets_Click(object sender, RoutedEventArgs e)
        {
            ticketListHeader.Text = "Open tickets";
            ticketDataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding { Source = ticketListViewModel.OpenTickets });
            DataContext = ticketListViewModel;
        }

        private void btnClosedTickets_Click(object sender, RoutedEventArgs e)
        {
            ticketListHeader.Text = "Closed tickets";
            ticketDataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding { Source = ticketListViewModel.ClosedTickets });
            DataContext = ticketListViewModel;
        }

        private void btnCreateTicket_Click(object sender, RoutedEventArgs e)
        {
            DataContext = ticketCreationViewModel;
        }
    }
}
