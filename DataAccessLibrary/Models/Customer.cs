using Newtonsoft.Json;
using System;

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
        public string DisplayName => $"{FirstName} {LastName}";

        [JsonIgnore]
        public string CustomerDisplay => $"{DisplayName} ({Id})";
    }
}
