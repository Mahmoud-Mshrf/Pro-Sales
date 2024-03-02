using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class CustomerDto
    {

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Range(18, 100)]
        public int Age { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }

        [MaxLength(50)]
        public string City { get; set; }
        [Required]
        public string SalesRepresntativeId { get; set; }
        [Required]
        public int sourceId { get; set; }
        [Required]
        public IEnumerable<UserInterestDto> UserInterests { get; set; }
    }
}
