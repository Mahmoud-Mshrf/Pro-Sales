﻿using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class AddCallDto
    {
        [EnumDataType(typeof(CallStatus))]
        [Required]
        public CallStatus? status { get; set; }
        [MaxLength(500)]
        [Required]
        public string summary { get; set; }
        [Required]
        public DateTime date { get; set; }
        public DateTime? followUp { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? CustomerId { get; set; }
    }
}
