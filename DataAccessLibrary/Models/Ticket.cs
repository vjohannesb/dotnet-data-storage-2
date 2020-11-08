using DataAccessLibrary.Services;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
                try
                {
                    TicketCustomer = ViewModel.Customers.First(c => c.Id == _customerId);
                } 
                catch (Exception ex) 
                { 
                    Debug.WriteLine($"Could not find customer {_customerId} in customer-list. {ex.Message}"); 
                }
            } 
        }

        // Lite dumsnålt kanske men lagrar bara filtypen som indikation
        // på huruvida ärendet har en tillhörande fil eller ej.
        // Minskar lagring + anslutningar. Filen döps efter ticket-id.
        // Settern sätter användbara properties som används lokalt i appen
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

        // Per Microsofts egna dokumentation kring när data ska samlagras
        // fann jag det lämpligast att lagra kommentarer direkt i ärendet
        [JsonProperty(PropertyName = "comments")]
        public ObservableCollection<Comment> Comments { get; set; }

        // -- JsonIgnores --

        [JsonIgnore]
        public string AttachmentFileName => 
            AttachmentExtension != null 
                                ? Id + AttachmentExtension 
                                : null;

        // Används bl.a. i XAML
        [JsonIgnore]
        public bool HasAttachment { get; set; } = false;

        // Sätts av SetAttachmentExtension, används i XAML för att visa bilden
        [JsonIgnore]
        public string AttachmentPath { get; set; }

        // För att visa status i XAML utan egna konverterare
        [JsonIgnore]
        public string StatusString => Status.ToString();

        // För att lagra Customer lokalt istället för på Cosmos DB
        // Hämtas vid konstruktion genom CustomerId som lagras på cosmos
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
