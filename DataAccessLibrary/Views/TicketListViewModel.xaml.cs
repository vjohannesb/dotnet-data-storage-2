using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace DataAccessLibrary.Views
{

    public sealed partial class TicketListViewModel : Page
    {
        private List<Customer> _customers => ViewModel.Customers;

        public DataGrid ticketDataGrid => dgTicketTable;
        public TextBlock ticketListHeader => tbListHeader;

        private DataTemplate _dgTemplate;

        public TicketListViewModel()
        {
            InitializeComponent();

            _dgTemplate = dgTicketTable.RowDetailsTemplate;
        }

        private void btnEditTicket_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.previousView = DataContext as ObservableCollection<Ticket>;

            var ticket = ((FrameworkElement)sender).DataContext as Ticket;

            // Halvdan lösning men för att kunna uppdatera en tickets attachment
            // så behöver den "frigöras" från detaljvyn först, då jag bara sparar en
            // attachment (som jag vill skriva över) och döper den efter ticket.Id
            ticket.AttachmentPath = null;
            dgTicketTable.ItemsSource = null;
            dgTicketTable.RowDetailsTemplate = null;
            dgTicketTable.RowDetailsTemplate = _dgTemplate;

            ViewModel.mainPage.DataContext = new TicketEditViewModel(ticket);
        }

        private async void btnRefreshDb_Click(object sender, RoutedEventArgs e)
        {
            btnRefreshDb.IsEnabled = false;
            tbUpdate.Visibility = Visibility.Visible;

            if (dgTicketTable.ItemsSource == ViewModel.OpenTickets)
                await DbService.UpdateOpenTicketsAsync();
            else
                await DbService.UpdateClosedTicketsAsync();

            tbUpdate.Visibility = Visibility.Collapsed;
            btnRefreshDb.IsEnabled = true;
        }

        private void btnShowImage_Click(object sender, RoutedEventArgs e)
        {
            var _button = ((FrameworkElement)sender) as HyperlinkButton;
            var _ticket = ((FrameworkElement)sender).DataContext as Ticket;

            var _image = (((FrameworkElement)sender).Tag) as Image;
            if (_image.Visibility == Visibility.Collapsed)
            {
                _image.Visibility = Visibility.Visible;
                _button.Content = "Close";
            }
            else
            {
                _image.Visibility = Visibility.Collapsed;
                _button.Content = _ticket.AttachmentFileName;

                // Halvdan kod men uppdaterar rowdetails' utrymme genom att återställa template
                dgTicketTable.RowDetailsTemplate = null;
                dgTicketTable.RowDetailsTemplate = _dgTemplate;
            }

            //FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
