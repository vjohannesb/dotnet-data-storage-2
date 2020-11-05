using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
using DataAccessLibrary.Settings;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DataAccessLibrary.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TicketEditViewModel : Page
    {
        private List<Customer> _customers => ViewModel.customers;
        private List<string> Categories => ViewModel.ticketCategories;

        private readonly Ticket _ticket;
        private Customer _ticketCustomer;

        public TicketEditViewModel(Ticket ticket)
        {
            InitializeComponent();
            _ticket = ticket;

            SetUpEdit();

        }

        public void SetUpEdit()
        {
            _ticketCustomer = _customers.First(c => c.Id == _ticket.CustomerId);

            cbxCategory.SelectedItem = _ticket.Category;
            cbxStatus.SelectedIndex = (int)_ticket.Status;
            cbxCustomer.SelectedItem = _ticketCustomer;
        }

        private void btnAddComment_Click(object sender, RoutedEventArgs e)
        {
            // Om kommentar är tom eller bara whitespace, dra fokus till kommentarsbox
            if (tbxComment.Text.Trim().Length > 0)
            {
                _ticket.Comments.Add(new Comment(tbxComment.Text));
                tbxComment.Text = string.Empty;
            }
            else
            {
                tbxComment.Text = string.Empty;
                tbxComment.Focus(FocusState.Programmatic);
            }
        }

        private async void btnSaveEdit_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();

            _ticket.Category = cbxCategory.SelectedItem.ToString();
            _ticket.Status = (Ticket.TicketStatus)cbxStatus.SelectedIndex;
            _ticket.CustomerId = cbxCustomer.SelectedValue.ToString();

            await DbService.UpdateTicketAsync(_ticket);

            EnableButtons();
        }

        private void btnCancelEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DisableButtons()
        {
            btnSaveEdit.IsEnabled = false;
            btnCancelEdit.IsEnabled = false;
        }

        private void EnableButtons()
        {
            btnSaveEdit.IsEnabled = true;
            btnCancelEdit.IsEnabled = true;
        }
    }
}
