﻿using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace DataAccessLibrary.Views
{

    public sealed partial class TicketListViewModel : Page
    {
        private List<Customer> _customers => ViewModel.customers;

        public DataGrid ticketDataGrid => dgTicketTable;
        public TextBlock ticketListHeader => tbListHeader;

        public bool ShowAttachment { get; private set; } = false;

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

        private void btnShowImage_Click(object sender, RoutedEventArgs e)
        {
            var ticket = ((FrameworkElement)sender).DataContext as Ticket;
            ticket.ShowAttachment = true;
            ((FrameworkElement))
            //FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
