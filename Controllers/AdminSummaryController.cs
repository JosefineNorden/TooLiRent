using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TooLiRent.Services.DTOs;
using TooLiRent.Services.Interfaces;

namespace TooLiRent.WebAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminSummaryController : ControllerBase
    {
        private readonly IAdminSummaryService _stats;

        public AdminSummaryController(IAdminSummaryService stats)
        {
            _stats = stats;
        }


        [HttpGet("summary")]
        [ProducesResponseType(typeof(AdminSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Summary()
        {
            var dto = await _stats.GetSummaryAsync();
            return Ok(dto);
        }

        [HttpGet("top-tools")]
        [ProducesResponseType(typeof(IEnumerable<TopToolDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> TopTools([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int take = 5)
        {
            if (take <= 0) take = 5;
            var list = await _stats.GetTopToolsAsync(from, to, take);
            return Ok(list);
        }
    }
}
