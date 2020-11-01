using DataAccessLibrary.Models;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DataAccessLibrary.Views
{

    public sealed partial class TicketListViewModel : Page
    {
        //private ViewModel viewModel = new ViewModel();

        public ObservableCollection<Ticket> OpenTickets = new ObservableCollection<Ticket>();
        public ObservableCollection<Ticket> ClosedTickets = new ObservableCollection<Ticket>();

        public static DataGrid ticketDataGrid;
        public static TextBlock ticketListHeader;

        public TicketListViewModel()
        {
            this.InitializeComponent();

            ticketDataGrid = dgTicketTable;
            ticketListHeader = tbListHeader;
            

            OpenTickets = new ObservableCollection<Ticket>() {
                new Ticket(1, "Error1", 1001, "abcdefhgihoaksdjlkaj sdk ajsld kasjd lasi djasidjasdl iajsd laijs dlasi jdals ijdals dijasld ijasdia jsdliaj sdlas d", (int)Ticket.TicketStatus.Open),
                new Ticket(2, "Error2", 1002, "Something went wrong", (int)Ticket.TicketStatus.Open),
                new Ticket(3, "Error3", 1003, "Something went wrong", (int)Ticket.TicketStatus.Active)
            };

            ClosedTickets = new ObservableCollection<Ticket>() {
                new Ticket(4, "Error4", 1004, "Something went wrong", (int)Ticket.TicketStatus.Closed),
                new Ticket(5, "Error5", 1005, "Something went wrong", (int)Ticket.TicketStatus.Closed),
                new Ticket(6, "Error6", 1006, "Something went wrong", (int)Ticket.TicketStatus.Closed)
            };
        }

        private void btnShowHideDetails_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
