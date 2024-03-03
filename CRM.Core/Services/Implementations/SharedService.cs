using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Core.Dtos;
using CRM.Core.Services.Interfaces;

namespace CRM.Core.Services.Implementations
{
    public class SharedService : ISharedService
    {
        private readonly IUnitOfWork _unitOfWork;
        // will be used after adding Manager module
        public async Task<ReturnInterstsDto> GetAllInterests()
        {
            var interests = await _unitOfWork.Interests.GetAllAsync();
            if (interests == null)
            {
                return new ReturnInterstsDto
                {
                    IsSuccess = false,
                    Message = "No interests found"
                };
            }
            var Interests = new List<InterestDto>();
            foreach (var interest in interests)
            {
                var interestDto = new InterestDto
                {
                    InterestID = interest.InterestID,
                    InterestName = interest.InterestName
                };
                Interests.Add(interestDto);
            }
            return new ReturnInterstsDto
            {
                IsSuccess = true,
                Message = "Interests found",
                Interests = Interests
            };
        }




    }
}
