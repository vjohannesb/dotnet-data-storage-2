using Azure;
using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DataAccessLibrary.Views
{
    public sealed partial class TicketEditViewModel : Page
    {
        private List<Customer> Customers => ViewModel.Customers;
        private List<string> Categories => ViewModel.ClientSettings.Categories;

        private Ticket _ticket;
        private StorageFile _file;

        private static ContentDialog errorDialog = new ContentDialog()
        {
            Title = "An error occured while updating the ticket",
            CloseButtonText = "Ok"
        };

        private bool _attachmentUpdated = false;

        // CTOR
        public TicketEditViewModel(Ticket ticket)
        {
            InitializeComponent();
            _ticket = ticket;

            SetUpEdit();
        }


        /* -- HELPERS (DRY) -- */
        public void SetUpEdit()
        {
            // Sätter programmatiskt för enklare konvertering från enum till int
            cbxStatus.SelectedIndex = (int)_ticket.Status;

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

        private void DisableButtons()
        {
            btnSaveEdit.IsEnabled = false;
            btnAttach.IsEnabled = false;
        }

        private void EnableButtons()
        {
            btnSaveEdit.IsEnabled = true;
            btnAttach.IsEnabled = true;
        }


        /* -- COMMENTS -- */
        private void btnAddComment_Click(object sender, RoutedEventArgs e)
        {
            // Förhindra tomma kommentarer
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


        /* -- DATABASE/SAVE FUNCTIONS -- */

        private async Task TryStoreFileAsync()
        {
            try
            {
                _ticket.AttachmentExtension = _file.FileType;
                await BlobService.StoreFileAsync(_file, _ticket.Id);
            }
            catch (FileLoadException flEx)
            {
                Debug.WriteLine($"File could not be loaded. {flEx}");
                tbAttachment.Text = "File access error. Try again or try another file.";

                errorDialog.Content = flEx.Message;
                await errorDialog.ShowAsync();
            }
            catch (UnauthorizedAccessException uaEx)
            {
                Debug.WriteLine($"File could not be loaded. {uaEx}");
                tbAttachment.Text = "File access error. Try again or try another file.";

                errorDialog.Content = uaEx.Message;
                await errorDialog.ShowAsync();
            }
        }

        private async Task TryDeleteFileAsync()
        {
            try
            {
                await BlobService.DeleteFileIfExists(_ticket.AttachmentFileName);
                _ticket.AttachmentExtension = null;
            }
            catch (FileLoadException flEx)
            {
                Debug.WriteLine($"File could not be loaded. {flEx}");
                tbAttachment.Text = "File access error. Try again or try another file.";

                errorDialog.Content = flEx.Message;
                await errorDialog.ShowAsync();
            }
            catch (UnauthorizedAccessException uaEx)
            {
                Debug.WriteLine($"File could not be loaded. {uaEx}");
                tbAttachment.Text = "File access error. Try again or try another file.";

                errorDialog.Content = uaEx.Message;
                await errorDialog.ShowAsync();
            }
        }

        private async void btnSaveEdit_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();

            _ticket.Category = cbxCategory.SelectedItem.ToString();
            _ticket.Status = (Ticket.TicketStatus)cbxStatus.SelectedIndex;
            _ticket.CustomerId = cbxCustomer.SelectedValue.ToString();

            if (_attachmentUpdated)
            {
                if (_ticket.HasAttachment)
                {
                    try
                    {
                        await TryStoreFileAsync();
                    }
                    catch (Exception ex)
                    {
                        errorDialog.Content = ex.Message;
                        await errorDialog.ShowAsync();
                    }
                }
                else
                {
                    try
                    {
                        await TryDeleteFileAsync();
                    }
                    catch (Exception ex)
                    {
                        errorDialog.Content = ex.Message;
                        await errorDialog.ShowAsync();
                    }
                }
            }

            try
            {
                await DbService.UpdateTicketAsync(_ticket);
            }
            catch (RequestFailedException rEx)
            {
                Debug.WriteLine($"Request failed - {rEx}");

                errorDialog.Content = rEx.Message;
                await errorDialog.ShowAsync();
            }
            catch (CosmosException cEx)
            {
                Debug.WriteLine($"Ticket could not be updated. {cEx}");

                errorDialog.Content = cEx.Message;
                await errorDialog.ShowAsync();
            }

            EnableButtons();
        }


        /* -- ATTACHMENTS -- */
        private async void btnAttach_Click(object sender, RoutedEventArgs e)
        {
            _attachmentUpdated = true;

            DisableButtons();

            if (!_ticket.HasAttachment)
                await AddAttachment();
            else
                AttachmentRemoved();

            EnableButtons();
        }

        private async Task AddAttachment()
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                FileTypeFilter = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" },
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail
            };

            _file = await picker.PickSingleFileAsync();

            if (_file != null)
            {
                var properties = await _file.GetBasicPropertiesAsync();
                if (properties.Size > 4 * 1024 * 1024)
                    tbAttachment.Text = "File size exceeds limit (4MB).";
                else
                    AttachmentAdded();
            }
        }

        private void AttachmentAdded()
        {
            btnAttach.Content = "Remove attachment";
            tbAttachment.Text = _file.Name;

            _ticket.HasAttachment = true;
        }

        private void AttachmentRemoved()
        {
            btnAttach.Content = "Add attachment";
            tbAttachment.Text = string.Empty;

            _ticket.HasAttachment = false;
        }

    }
}
