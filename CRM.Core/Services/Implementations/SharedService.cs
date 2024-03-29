﻿using System;
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

        public SharedService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // will be used after adding Manager module
        public async Task<ReturnInterstsDto> GetAllInterests()
        {
            var interests = await _unitOfWork.Interests.GetAllAsync();
            if (interests == null)
            {
                return new ReturnInterstsDto
                {
                    IsSuccess = false,
                    Errors = ["No interests found"]
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
                Interests = Interests
            };
        }



        // will be used after adding Manager module
        public async Task<ReturnSourcesDto> GetAllSources()
        {
            var sources = await _unitOfWork.Sources.GetAllAsync();
            if (sources == null)
            {
                return new ReturnSourcesDto
                {
                    IsSuccess = false,
                    Errors = ["No sources found"]
                };
            }
            var Sources = new List<SourceDto>();
            foreach (var source in sources)
            {
                var sourceDto = new SourceDto
                {
                    SourceName = source.SourceName,
                    SourceId = source.SourceId
                };
                Sources.Add(sourceDto);
            }
            return new ReturnSourcesDto
            {
                IsSuccess = true,
                Sources = Sources
            };
        }



    }
}
