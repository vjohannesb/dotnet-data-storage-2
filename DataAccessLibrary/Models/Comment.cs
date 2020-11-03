using DataAccessLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Models
{
    public class Comment
    {
        public Comment(string ticketId, string content)
        {
            Id = Guid.NewGuid().ToString();
            Created = DateTime.Now.ToString("g");

            TicketId = ticketId;
            Content = content;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; }

        [JsonProperty(PropertyName = "ticketId")]
        public string TicketId { get; set; }

        [JsonProperty(PropertyName = "created")]
        public string Created { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }
}
