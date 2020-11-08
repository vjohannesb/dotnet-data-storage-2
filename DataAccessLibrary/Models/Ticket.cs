using DataAccessLibrary.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
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

            // Om ärendet ex. hämtas från Azure & bifogad fil inte finns lagrad lokalt
            if (AttachmentExtension != null)
                DownloadAttachmentIfNotExist().GetAwaiter();
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

        // Hämta Customer när CustomerId sätts istället
        // för att lagra en hel Customer i CosmosDB
        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId 
        {
            get { return _customerId; }
            set
            {
                _customerId = value;
                TicketCustomer = ViewModel.Customers.First(c => c.Id == _customerId);
            } 
        }

        // För att bara behöva lagra en variabel i Cosmos DB
        // som indikation på om ärendet har en attachment eller inte
        // och samtidigt kunna använda Cosmos' JsonSerializer
        [JsonProperty(PropertyName = "attachmentExtension")]
        public string AttachmentExtension
        {
            get { return _attachmentExtension; }
            set
            {
                _attachmentExtension = value;
                if (_attachmentExtension != null)
                {
                    AttachmentPath = "ms-appdata:///local/" + AttachmentFileName;
                    HasAttachment = true;
                }
                else
                {
                    AttachmentPath = string.Empty;
                    HasAttachment = false;
                }
            }
        }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; }

        [JsonProperty(PropertyName = "comments")]
        public ObservableCollection<Comment> Comments { get; set; }

        // -- JsonIgnores --

        [JsonIgnore]
        public string AttachmentFileName => 
            AttachmentExtension != null 
                                ? Id + AttachmentExtension 
                                : null;

        // Visibility i detaljvy + om fil ska laddas upp/ner eller ej
        [JsonIgnore]
        public bool HasAttachment { get; set; } = false;

        // Lokal path till bifogad fil, sätts initiellt av SetAttachmentExtension
        [JsonIgnore]
        public string AttachmentPath { get; set; }

        // För att visa status i XAML
        [JsonIgnore]
        public string StatusString => Status.ToString();

        // För att lagra Customer lokalt istället för på Cosmos DB
        [JsonIgnore]
        public Customer TicketCustomer { get; private set; }

        // -- Fields --
        [JsonIgnore]
        private string _customerId;

        [JsonIgnore]
        private string _attachmentExtension;

        // -- Methods --
        private async Task DownloadAttachmentIfNotExist()
            => await BlobService.DownloadFileIfNotExistAsync(AttachmentFileName);
    }
}
