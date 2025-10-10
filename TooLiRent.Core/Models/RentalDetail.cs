﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Models;

namespace TooLiRent.Core.Models
{
    public class RentalDetail : BaseEntity
    {
        [Required]
        public int RentalId { get; set; }

        [ForeignKey("RentalId")]
        public Rental Rental { get; set; } = null!;

        [Required]
        public int ToolId { get; set; }

        [ForeignKey("ToolId")]
        public Tool Tool { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }
    }
}
