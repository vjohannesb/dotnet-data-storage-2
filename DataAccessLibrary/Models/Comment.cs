using Newtonsoft.Json;
using System;

namespace DataAccessLibrary.Models
{
    public class Comment
    {
        public Comment(string content)
        {
            Id = Guid.NewGuid().ToString();
            Created = DateTime.Now.ToString("g");

            Content = content;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; }

        [JsonProperty(PropertyName = "created")]
        public string Created { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }
}
