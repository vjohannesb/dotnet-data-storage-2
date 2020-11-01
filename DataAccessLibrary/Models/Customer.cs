using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Tickets = new HashSet<Ticket>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
