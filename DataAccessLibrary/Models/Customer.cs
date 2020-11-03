using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Models
{
    public class Customer
    {
        public Customer(string firstName, string lastName)
        {
            Id = Guid.NewGuid().ToString();
            Type = "customer";
            
            FirstName = firstName;
            LastName = lastName;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; }


        public string FullName => $"{FirstName} {LastName}";

        public string CustomerDisplay => $"{FullName} ({Id})";
    }
}
