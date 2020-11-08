using Azure.Storage.Blobs.Models;
using DataAccessLibrary.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace DataAccessLibrary.Models
{
    public class ViewModel
    {
        public static ObservableCollection<Ticket> OpenTickets { get; set; } = new ObservableCollection<Ticket>();
        public static ObservableCollection<Ticket> ClosedTickets { get; set; } = new ObservableCollection<Ticket>();

        public static List<Customer> Customers { get; set; } = new List<Customer>();

        public static ClientSettings ClientSettings { get; set; } = new ClientSettings();

        public static ObservableCollection<Ticket> previousView { get; set; }

        public static Page mainPage;

    }
}
