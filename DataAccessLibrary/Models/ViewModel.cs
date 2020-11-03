using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace DataAccessLibrary.Models
{
    public class ViewModel
    {
        public static ObservableCollection<Ticket> OpenTickets = new ObservableCollection<Ticket>();
        public static ObservableCollection<Ticket> ClosedTickets = new ObservableCollection<Ticket>();

        // Använder list då denna apps "customers" inte kommer förändras
        public static List<Customer> Customers = new List<Customer>() { new Customer("Anders", "Andersson") };

        public static Page mainPage;
    }
}
