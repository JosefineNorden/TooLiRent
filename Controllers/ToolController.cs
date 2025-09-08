using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TooLiRent.Services.DTOs;
using TooLiRent.Services.Interfaces;

namespace TooLiRent.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolController : ControllerBase
    {

        private readonly IToolService _toolService;

        public ToolController(IToolService toolService)
        {
            _toolService = toolService;
        }

        /// <summary>
        /// Get all tools
        /// </summary>

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ToolDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ToolDto>>> GetAll()
        {
            var tools = await _toolService.GetAllAsync();
            return Ok(tools);
        }

        /// <summary>
        /// Get a specific tool by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ToolDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ToolDto>> GetById(int id)
        {
            var tool = await _toolService.GetByIdAsync(id);
            if (tool == null)
                return NotFound($"Tool with ID {id} not found");

            return Ok(tool);
        }

        /// <summary>
        /// Create a new tool (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(ToolDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ToolDto>> CreateTool(ToolCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _toolService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update an existing tool (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTool(int id, ToolUpdateDto dto)
        {
            var success = await _toolService.UpdateAsync(id, dto);
            if (!success)
                return NotFound($"Tool with ID {id} not found");

            return NoContent();
        }

        /// <summary>
        /// Delete a tool (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTool(int id)
        {
            var success = await _toolService.DeleteAsync(id);
            if (!success)
                return NotFound($"Tool with ID {id} not found");

            return NoContent();
        }

        /// <summary>
        /// Get all available tools (Stock > 0)
        /// </summary>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<ToolDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ToolDto>>> GetAvailable()
        {
            var availableTools = await _toolService.GetAvailableAsync();
            return Ok(availableTools);
        }

        /// <summary>
        /// Get filtered tools by category, status, and availability
        /// </summary>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(IEnumerable<ToolDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ToolDto>>> FilterTools(
            [FromQuery] string? category,
            [FromQuery] string? status,
            [FromQuery] bool? onlyAvailable)
        {
            var filtered = await _toolService.FilterAsync(category, status, onlyAvailable);
            return Ok(filtered);
        }


    }
}
