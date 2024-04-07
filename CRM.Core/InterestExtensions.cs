using CRM.Core.Dtos;
using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core
{
    public static class InterestExtensions
    {
        public static InterestDto ToInterestDto(this Interest interest)
        {
            if (interest == null)
                return null;

            return new InterestDto
            {
                Id = interest.InterestID,
                Name = interest.InterestName
                
            };
        }
    }
}
