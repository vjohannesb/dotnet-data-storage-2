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
using Windows.Storage;
using Windows.Storage.Pickers;
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

        private Ticket _ticket;
        private Customer _ticketCustomer;
        private StorageFile _file;

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

            if (_ticket.HasAttachment)
            {
                btnAttach.Content = "Remove attachment";
                tbAttachment.Text = _ticket.AttachmentFileName;
            }
            else
            {
                btnAttach.Content = "Add attachment";
                tbAttachment.Text = string.Empty;
            }
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

            if (_ticket.HasAttachment)
            {
                await BlobService.StoreFileAsync(_file, _ticket.Id);
                _ticket.AttachmentExtension = _file.FileType;
            }
            else
            {
                await BlobService.DeleteFileIfExistAsync(_ticket.Id);
                _ticket.AttachmentExtension = null;
            }

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

        private async void btnAttach_Click(object sender, RoutedEventArgs e)
        {
            if (!_ticket.HasAttachment)
            {
                FileOpenPicker picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".gif");
                picker.FileTypeFilter.Add(".bmp");
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.ViewMode = PickerViewMode.Thumbnail;

                _file = await picker.PickSingleFileAsync();

                if (_file != null)
                {
                    btnAttach.Content = "Remove attachment";
                    tbAttachment.Text = _file.Name;

                    _ticket.HasAttachment = true;
                }
            }
            else
            {
                btnAttach.Content = "Add attachment";
                tbAttachment.Text = string.Empty;

                _ticket.HasAttachment = false;
            }
        }
    }
}
