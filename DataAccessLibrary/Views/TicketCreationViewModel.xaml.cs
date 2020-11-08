using Azure;
using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
using DataAccessLibrary.Settings;
using DataAccessLibrary.Views;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DataAccessLibrary.Views
{
    public sealed partial class TicketCreationViewModel : Page
    {
        private List<Customer> Customers => ViewModel.Customers;
        private List<string> Categories => ViewModel.ClientSettings.Categories;

        private Ticket _ticket = new Ticket();
        private StorageFile _file;

        private readonly SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush resetBrush = new SolidColorBrush(Colors.Black);
        private readonly List<ComboBox> comboBoxes;

        private static ContentDialog errorDialog = new ContentDialog()
        {
            Title = "An error occured while updating the ticket",
            CloseButtonText = "Ok"
        };
        

        public TicketCreationViewModel()
        {
            InitializeComponent();
            tbCreated.Text = DateTime.Now.ToString("g");

            redBrush.Opacity = 0.4;
            resetBrush.Opacity = 0.5;
            comboBoxes = new List<ComboBox>() { cbxCategory, cbxStatus, cbxCustomer };
        }


        /* -- HELPERS (DRY) -- */
        private void ResetInput()
        {
            _ticket = new Ticket();
            lvComments.ItemsSource = _ticket.Comments;

            tbxComment.Text = string.Empty;
            tbxDescription.Text = string.Empty;

            foreach (var cbx in comboBoxes)
            {
                cbx.SelectedIndex = -1;
                cbx.BorderBrush = resetBrush;
            }

            AttachmentRemoved();
        }

        private void EnableButtons()
        {
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
        }

        private void DisableButtons()
        {
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
        }
        

        /* -- ATTACHMENTS -- */
        private async void btnAttach_Click(object sender, RoutedEventArgs e)
        {
            if (!_ticket.HasAttachment)
                await AddAttachment();
            else
                AttachmentRemoved();
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


        /* -- COMMENTS -- */
        private void btnAddComment_Click(object sender, RoutedEventArgs e)
        {
            // Om kommentar är tom eller bara whitespace, dra fokus till kommentarsbox
            if (tbxComment.Text.Trim().Length > 0)
                _ticket.Comments.Add(new Comment(tbxComment.Text));
            else
                tbxComment.Focus(FocusState.Programmatic);

            tbxComment.Text = string.Empty;
        }
        
        private void btnRemoveComment_Click(object sender, RoutedEventArgs e)
            => _ticket.Comments.Remove(((FrameworkElement)sender).DataContext as Comment);


        /* -- DATABASE / SAVE FUNCTIONS -- */
        private async Task<bool> TryStoreFileAsync()
        {
            try
            {
                if (_ticket.HasAttachment)
                {
                    await BlobService.StoreFileAsync(_file, _ticket.Id);
                    _ticket.AttachmentExtension = _file.FileType;
                }
                return true;
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

            return false;
        }

        private async Task TryAddTicketAsync()
        {
            try
            {
                await DbService.AddTicketAsync(_ticket);
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
        }

        private async Task SaveTicketToDb()
        {
            DisableButtons();

            _ticket.Category = cbxCategory.SelectedItem.ToString();
            _ticket.Status = (Ticket.TicketStatus)cbxStatus.SelectedIndex;
            _ticket.CustomerId = cbxCustomer.SelectedValue.ToString();
            _ticket.Description = tbxDescription.Text;

            try
            {
                if (await TryStoreFileAsync())
                    await TryAddTicketAsync();

                ResetInput();
            }
            catch (Exception ex)
            {
                errorDialog.Content = ex.Message;
                await errorDialog.ShowAsync();
            }

            EnableButtons();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Kontrollera värden och "fokusera" på "första felaktiga"
            if (cbxCategory.SelectedIndex == -1)
                cbxCategory.BorderBrush = redBrush;
            else if (cbxStatus.SelectedIndex == -1)
                cbxStatus.BorderBrush = redBrush;
            else if (cbxCustomer.SelectedIndex == -1)
                cbxCustomer.BorderBrush = redBrush;
            else if (tbxDescription.Text.Trim().Length < 1)
                tbxDescription.Focus(FocusState.Programmatic);
            else
                await SaveTicketToDb();
        }


        // CANCEL + COMBOBOX CHECK
        private void btnCancel_Click(object sender, RoutedEventArgs e)
            => ResetInput();
        
        private void cbx_GotFocus(object sender, RoutedEventArgs e)
            => ((ComboBox)sender).BorderBrush = resetBrush;











    }
}
