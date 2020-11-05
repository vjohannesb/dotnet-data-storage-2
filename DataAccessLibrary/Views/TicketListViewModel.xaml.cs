using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DataAccessLibrary.Views
{

    public sealed partial class TicketListViewModel : Page
    {
        private List<Customer> _customers => ViewModel.customers;

        public DataGrid ticketDataGrid => dgTicketTable;
        public TextBlock ticketListHeader => tbListHeader;

        public TicketListViewModel()
        {
            InitializeComponent();
        }

        private void btnEditTicket_Click(object sender, RoutedEventArgs e)
        {
            var ticket = ((FrameworkElement)sender).DataContext as Ticket;
            ViewModel.mainPage.DataContext = new TicketEditViewModel(ticket);
        }

        private async void btnRefreshDb_Click(object sender, RoutedEventArgs e)
        {
            btnRefreshDb.IsEnabled = false;
            tbUpdate.Visibility = Visibility.Visible;

            await DbService.UpdateTicketListAsync();

            tbUpdate.Visibility = Visibility.Collapsed;
            btnRefreshDb.IsEnabled = true;
        }
    }
}
