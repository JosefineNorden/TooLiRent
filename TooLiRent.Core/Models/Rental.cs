﻿using TooLiRent.Core.Models;

namespace TooLiRent.Models
{
    public class Rental : BaseEntity
    {
        public int CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Customer Customer { get; set; } = null!;
        public ICollection<RentalDetail> RentalDetails { get; set; } = new List<RentalDetail>();
        public bool IsReturned { get; set; } = false;
    }
}

