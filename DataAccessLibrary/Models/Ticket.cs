using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace DataAccessLibrary.Models
{
    public class Ticket
    {
        public enum TicketStatus
        {
            Open = 0,
            Active,
            Closed
        }

        public Ticket()
        {
            Id = Guid.NewGuid().ToString();
            Created = DateTime.Now.ToString("g");
            Comments = new ObservableCollection<Comment>();
            Type = "ticket";
        }

        [JsonConstructor]
        public Ticket(string id, string created, string category, 
                      string description, TicketStatus status, string customerId,
                      string attachmentExtension, string type, 
                      ObservableCollection<Comment> comments)
        {
            Id = id;
            Created = created;
            Category = category;
            Description = description;
            Status = status;
            CustomerId = customerId;
            AttachmentExtension = attachmentExtension;
            Type = type;
            Comments = comments;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; }

        [JsonProperty(PropertyName = "created")]
        public string Created { get; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "status")]
        public TicketStatus Status { get; set; }

        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId 
        {
            get { return _customerId; }
            set
            {
                _customerId = value;
                TicketCustomer = ViewModel.customers.First(c => c.Id == _customerId);
            } 
        }

        [JsonProperty(PropertyName = "attachmentExtension")]
        public string? AttachmentExtension
        {
            get
            {
                return _attachmentExtension;
            }

            set
            {
                _attachmentExtension = value;
                try
                {
                    _attachedImage = new BitmapImage(new Uri($"{ApplicationData.Current.LocalFolder}\\{Id}{_attachmentExtension}"));
                    await ApplicationData.Current.LocalFolder.GetFileAsync($"{Id}{AttachmentExtension}")
                }
                catch
                {
                    _attachedImage = null;
                }
            }
        }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; }

        [JsonProperty(PropertyName = "comments")]
        public ObservableCollection<Comment> Comments { get; set; }


        [JsonIgnore]
        public string StatusString => Status.ToString();

        [JsonIgnore]
        public Customer TicketCustomer { get; private set; }

        [JsonIgnore]
        private string _customerId;

        [JsonIgnore]
        private string _attachmentExtension;

        [JsonIgnore]
        public string _attachedFileName => $"{Id}{AttachmentExtension}";

        [JsonIgnore]
        public BitmapImage _attachedImage { get; private set; }

    }
}
