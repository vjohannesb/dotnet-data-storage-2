using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Models
{
    public partial class Ticket
    {
        public enum TicketStatus
        {
            Open = 0,
            Active,
            Closed
        }

        public Ticket()
        {
            Comments = new ObservableCollection<Comment>();
        }
        public Ticket(int ticketId, string category, int customerId, string description, int status)
        {
            Id = ticketId;
            Category = category;
            CustomerId = customerId;
            Description = description;
            Status = status;
            Created = DateTime.Now.ToString("g");
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string Created { get; set; }
        [Required]
        [StringLength(50)]
        public string Category { get; set; }
        [Required]
        public string Description { get; set; }
        public int Status { get; set; }
        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty(nameof(Models.Customer.Tickets))]
        public virtual Customer Customer { get; set; }
        [InverseProperty("Ticket")]
        public virtual ObservableCollection<Comment> Comments { get; set; }

    }
}
