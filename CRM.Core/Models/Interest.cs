﻿using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Models
{
    public class Interest
    {
        [Key]
        public int InterestID { get; set; }

        [Required]
        [MaxLength(50)]
        public string InterestName { get; set; }
        public bool IsDisabled { get; set; }
        public ICollection<Customer> Customers { get; set; }=new List<Customer>();
        public ICollection<Deal> deals { get; set; } = new List<Deal>();
    }
}