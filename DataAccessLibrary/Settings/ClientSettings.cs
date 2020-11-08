using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Settings
{
    public class ClientSettings
    {
        /// <summary>
        /// An empty constructor for initializing with standard settings.
        /// </summary>
        public ClientSettings() {}

        [JsonConstructor]
        public ClientSettings(List<string> categories, int closedTicketTake)
        {
            Categories = categories;
            ClosedTicketTake = closedTicketTake;
        }

        [JsonProperty(PropertyName = "categories")]
        public List<string> Categories { get; set; } = new List<string>() { "Error T1", "Error T2", "Error T3" };

        [JsonProperty(PropertyName = "closedTicketTake")]
        public int ClosedTicketTake { get; set; } = 5;
    }
}
