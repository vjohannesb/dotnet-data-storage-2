using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Models
{
    public partial class Comment
    {
        [Key]
        public int Id { get; set; }
        public int TicketId { get; set; }
        [Required]
        [StringLength(20)]
        public string Created { get; set; }
        [Required]
        public string Text { get; set; }

        [ForeignKey(nameof(TicketId))]
        [InverseProperty(nameof(Models.Ticket.Comments))]
        public virtual Ticket Ticket { get; set; }
    }
}
