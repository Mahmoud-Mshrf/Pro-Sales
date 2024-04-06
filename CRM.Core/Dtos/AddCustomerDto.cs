using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class AddCustomerDto
    {
            [Required]
            [MaxLength(50)]
            public string FirstName { get; set; }

            [Required]
            [MaxLength(50)]
            public string LastName { get; set; }

            [Phone][Required]
            public string Phone { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            public int Age { get; set; }

            [EnumDataType(typeof(Gender))]
            public Gender Gender { get; set; } = Gender.None;

            [MaxLength(50)]
            public string City { get; set; }
            [Required]
            public string SalesRepresntativeId { get; set; }
            [Required]
            public string Source { get; set; }
            [Required]
            //public IList<UserInterestDto> UserInterests { get; set; }
            public IEnumerable<AddInterestDto> Interests { get; set; }
        }
}
