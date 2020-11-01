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

namespace UwpApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TicketCreationViewModel : Page
    {
        public TicketCreationViewModel()
        {
            this.InitializeComponent();
            // tbxTicketId.Text = Guid.NewGuid().ToString();
            tbxCreated.Text = DateTime.Now.ToString("g");
        }

        private void btnSaveTicket_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancelNewTicket_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddComment_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
