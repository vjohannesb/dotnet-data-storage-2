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
            var ticket = ((FrameworkElement)sender).DataContext as Ticket;

            // Uppdaterar XAML genom att reset:a template som visar bilden.
            // Behövs då bilden kan behöva skrivas över vilket inte går
            // om den används.
            ticket.AttachmentPath = null;
            dgTicketTable.ItemsSource = null;
            dgTicketTable.RowDetailsTemplate = null;
            dgTicketTable.RowDetailsTemplate = _dgTemplate;

            ViewModel.MainPage.DataContext = new TicketEditViewModel(ticket);
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
            // Lite hackig kod men gör det den ska... Visa/dölja bild
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

                // Uppdaterar rowdetails' utrymme genom att reset:a template
                dgTicketTable.RowDetailsTemplate = null;
                dgTicketTable.RowDetailsTemplate = _dgTemplate;
            }
        }
    }
}
