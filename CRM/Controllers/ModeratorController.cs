﻿using CRM.Core.Dtos;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM.Controllers
{
    [Authorize(Roles ="Marketing Moderator")] // later will be [Authorize(Roles ="MarketingModerator")]
    [Route("api/[controller]")]
    [ApiController]
    public class ModeratorController : ControllerBase
    {
        private readonly IModeratorService _moderatorService;
        public ModeratorController(IModeratorService moderatorService)
        {
            _moderatorService = moderatorService;
        }

        // Will be used after adding Manager module
        [HttpGet("get-all-sales")]
        public async Task<IActionResult> GetAllSalesRepresentatives()
        {
            var result = await _moderatorService.GetAllSalesRepresentatives();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result.Users);
        }

        [HttpPost("add-customer")]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var marketingModeratorEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _moderatorService.AddCustomer(customerDto,marketingModeratorEmail);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("update-customer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDto customerDto,int CustomerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var marketingModeratorEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _moderatorService.UpdateCustomer(customerDto, CustomerId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("get-all-customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var moderatorEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _moderatorService.GetAllCustomers(moderatorEmail);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result.Customers);
        }
        [HttpGet("get-customer/{CustomerId}")]
        public async Task<IActionResult> GetCustomer(int CustomerId)
        {
            var moderatorEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _moderatorService.GetCustomer(CustomerId, moderatorEmail);
            if (!result.IsSuccess)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
    }
}
