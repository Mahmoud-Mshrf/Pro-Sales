using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("daily-report")]
        public async Task<IActionResult> DailyReport(int page, int size)
        {
            var result = await _reportingService.GetDailyReport(page, size);
            return Ok(result);
        }
        [HttpGet("global-statistics")]
        public async Task<IActionResult> GlobalStat()
        {
            var result = await _reportingService.GlobalStatAsync();
            return Ok(result);
        }
        [HttpGet("sales-reprot/{id}")]
        public async Task<IActionResult> SalesReport(string id)
        {
            var result = await _reportingService.SalesReport(id);
            if (result.IsSuccess == false)
            {
                var errors = new { result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
    }
}
