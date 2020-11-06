using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
using DataAccessLibrary.Settings;
using DataAccessLibrary.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private List<Customer> Customers => ViewModel.customers;
        private List<string> Categories => ViewModel.ticketCategories;

        private Ticket _ticket = new Ticket();
        private StorageFile _file;

        private readonly SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush resetBrush = new SolidColorBrush(Colors.Black);
        private readonly List<ComboBox> comboBoxes;

        public TicketCreationViewModel()
        {
            InitializeComponent();
            tbCreated.Text = DateTime.Now.ToString("g");

            redBrush.Opacity = 0.4;
            resetBrush.Opacity = 0.4;
            comboBoxes = new List<ComboBox>() { cbxCategory, cbxStatus, cbxCustomer };
        }

        private void ResetData()
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
        }

        private void btnAddComment_Click(object sender, RoutedEventArgs e)
        {
            // Om kommentar är tom eller bara whitespace, dra fokus till kommentarsbox
            if (tbxComment.Text.Trim().Length > 0)
                _ticket.Comments.Add(new Comment(tbxComment.Text));
            else
                tbxComment.Focus(FocusState.Programmatic);

            tbxComment.Text = string.Empty;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Kontrollera värden och fokusera på "första felaktiga"
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


        private async Task SaveTicketToDb()
        {
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;

            _ticket.Category = cbxCategory.SelectedItem.ToString();
            _ticket.Status = (Ticket.TicketStatus)cbxStatus.SelectedIndex;
            _ticket.CustomerId = cbxCustomer.SelectedValue.ToString();
            _ticket.Description = tbxDescription.Text;

            if (_ticket.HasAttachment)
            {
                await BlobService.StoreFileAsync(_file, _ticket.Id);
                _ticket.AttachmentExtension = _file.FileType;
            }
            await DbService.AddTicketAsync(_ticket);

            ResetData();

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
            => ResetData();

        private void cbx_GotFocus(object sender, RoutedEventArgs e)
            => ((ComboBox)sender).BorderBrush = resetBrush;

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

                _ticket.AttachmentExtension = null;
                _ticket.HasAttachment = false;
            }
        }
    }
}
