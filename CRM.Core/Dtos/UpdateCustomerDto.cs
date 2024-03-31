using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class UpdateCustomerDto
    {

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Phone]
        [Required]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public int Age { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; } = Gender.None;

        [MaxLength(50)]
        public string City { get; set; }
        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SalesRepresntativeId { get; set; }
        [Required]
        public string Source { get; set; }
        [Required]
        public IList<AddInterestDto> Interests { get; set; }
    }
}
