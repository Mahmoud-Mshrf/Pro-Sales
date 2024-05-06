using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace CRM.Controllers
{
    [Authorize(Roles = "Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportingService _reportingService;

        public ReportsController(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }
        [HttpGet("main-report")]
        public async Task<IActionResult> DailyReport(int page, int size, string? within)
        {
            //if (within == "Daily" || within == "Monthly" || within == "Weekly")
            //{
            //    var result = await _reportingService.MainReport(page, size, within);
            //    return Ok(result);
            //}
            //else
            //{
            //    var error = new List<string>();
            //    error = ["Not a vaild time period"];
            //    var errors = new { error };
            //    return BadRequest(errors);
            //}
           var result = await _reportingService.MainReport(page, size, within);
           return Ok(result);
            
        }
        [HttpGet("global-statistics")]
        public async Task<IActionResult> GlobalStat()
        {
            var result = await _reportingService.GlobalStatAsync();
            return Ok(result);
        }
        [HttpGet("sales-reprot/{id}")]
        public async Task<IActionResult> SalesReport(string id, string? within)
        {
            //if (within == "Daily" || within == "Monthly" || within == "Weekly")
            //{
            //    var result = await _reportingService.SalesReport(id, within);
            //    if (!result.IsSuccess)
            //    {
            //        var errors = new { result.Errors };
            //        return BadRequest(errors);
            //    }
            //    return Ok(result);
            //}
            //else
            //{
            //    var error = new List<string>();
            //    error = ["Not a vaild time period"];
            //    var errors = new { error };
            //    return BadRequest(errors);
            //}
            var result = await _reportingService.SalesReport(id, within);
            if (!result.IsSuccess)
            {
                var errors = new { result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
            
        }
    }
}
