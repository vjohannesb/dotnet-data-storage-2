using Azure.Storage.Blobs.Models;
using DataAccessLibrary.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace DataAccessLibrary.Models
{
    // Använder ViewModel som samlingsfil för allmänt accessbara objekt
    public static class ViewModel
    {
        public static ObservableCollection<Ticket> OpenTickets { get; set; } = new ObservableCollection<Ticket>();
        public static ObservableCollection<Ticket> ClosedTickets { get; set; } = new ObservableCollection<Ticket>();

        public static List<Customer> Customers { get; set; } = new List<Customer>();

        public static ClientSettings ClientSettings { get; set; } = new ClientSettings();

        // Referens till huvudsidan för navigation inifrån contentcontrol
        public static Page MainPage { get; set; }

    }
}
