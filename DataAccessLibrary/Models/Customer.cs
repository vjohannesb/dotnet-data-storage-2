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

        [JsonConstructor]
        public Customer(string id, string firstName, string lastName, string type)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Type = type;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; }

        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";

        [JsonIgnore]
        public string CustomerDisplay => $"{FullName} ({Id})";
    }
}
