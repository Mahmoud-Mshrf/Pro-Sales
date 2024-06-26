﻿using CRM.Core.Dtos;
using CRM.Core.Services.Implementations;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Security.Claims;

namespace CRM.Controllers
{
    [Authorize(Roles = "Marketing Moderator")] // later will be [Authorize(Roles ="MarketingModerator")]
    [Route("api/[controller]")]
    [ApiController]
    public class ModeratorController : ControllerBase
    {
        private readonly IModeratorService _moderatorService;
        private readonly ISharedService? _sharedService;
        public ModeratorController(IModeratorService moderatorService, ISharedService sharedService)
        {
            _moderatorService = moderatorService;
            _sharedService = sharedService;
        }
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
        public async Task<IActionResult> AddCustomer([FromBody] AddCustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var marketingModeratorEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _moderatorService.AddCustomer(customerDto, marketingModeratorEmail);
            if (!result.IsSuccess)
            {
                var errors = new {result.Errors};
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpPut("update-customer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] AddCustomerDto customerDto, int CustomerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var marketingModeratorEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _moderatorService.UpdateCustomer(customerDto, CustomerId);
            if (!result.IsSuccess)
            {
                var errors = new { result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpGet("get-last-week-customers")]
        public async Task<IActionResult> GetLastWeekCustomers(int page, int size)
        {
            var result = await _moderatorService.GetLastWeekCustomers(page, size);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result.Pages);
        }
        [HttpGet("get-customer/{customerId}")]
        public async Task<IActionResult> GetCustomer(int customerId)
        {
            var result = await _moderatorService.GetCustomer(customerId);
            if (!result.IsSuccess)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpDelete("delete-customer/{customerId}")]
        public async Task<IActionResult> DeletCustomer(int customerId)
        {
            var result = await _moderatorService.DeleteCustomer(customerId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("add-source")]
        public async Task<IActionResult> AddSource([FromBody] NameDto dto)
        {
            var result = await _moderatorService.AddSource(dto.Name);
            if (result.Id==0|| string.IsNullOrEmpty(result.Name))
            {
                var errors = new { errors = "Source already exist" };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpGet("get-customers")]
        public async Task<IActionResult> GetCustomers(int page , int size, [FromQuery] string? query = null)
        {
            if(query == null)
            {
                var result = await _moderatorService.GetAllCustomers(page, size);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }
                return Ok(result.Pages);
            }
            else
            {
                var result = await _moderatorService.Search(query, page, size);
                return Ok(result.Pages);
            }
        }
        [HttpGet("get-sales/{id}")]
        public async Task<IActionResult> GetSalesById(string id)
        {
            var result = await _moderatorService.GetSalesById(id);
            if (result == null)
            {
                var errors = new { errors = "Sales representative not found" };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpGet("get-last-action")]
        public async Task<IActionResult> LastAction(int id)
        {
            var result = await _sharedService.GetLastAction(id);
            return Ok(result);
        }
        [HttpGet("get-customer-actions/{customerId}")]
        public async Task<ActionResult<IEnumerable<ActionDto>>> GetAllActionsForCustomer(int customerId)
        {
            var actions = await _moderatorService.GetAllActionsForCustomer(customerId);
            if (actions == null)
            {
                var errors = new { errors = new List<string> { "Customer not found" } };

                return BadRequest(errors);
            }
            return Ok(actions);
        }
    }
}
