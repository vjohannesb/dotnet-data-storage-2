using DataAccessLibrary.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace DataAccessLibrary.Models
{
    public class ViewModel
    {
        public static ObservableCollection<Ticket> OpenTickets = new ObservableCollection<Ticket>();
        public static ObservableCollection<Ticket> ClosedTickets = new ObservableCollection<Ticket>();

        public static List<string> ticketCategories = new List<string>() { "Error T1", "Error T2", "Error T3" };

        // Använder list då denna apps "customers" inte kommer förändras
        public static List<Customer> customers = new List<Customer>();

        public static Page mainPage;
    }
}
