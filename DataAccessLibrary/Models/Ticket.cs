using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        public Ticket(string category, string description, string customerId, TicketStatus status)
        {
            Id = Guid.NewGuid().ToString();
            Created = DateTime.Now.ToString("g");
            Comments = new ObservableCollection<Comment>();
            Type = "ticket";

            Category = category;
            Description = description;
            CustomerId = customerId;
            Status = status;

            //var listItem = Comments.Single(c => c.Id == Id);
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; }

        [JsonProperty(PropertyName = "created")]
        public string Created { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        // Spara Enumvärdet som string när det serialiseras
        [JsonProperty(PropertyName = "status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TicketStatus Status { get; set; }

        public string StatusString => Status.ToString();

        [JsonProperty(PropertyName = "customerid")]
        public string CustomerId { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; }

        [JsonProperty(PropertyName = "comments")]
        public ObservableCollection<Comment> Comments { get; set; }

    }
}
