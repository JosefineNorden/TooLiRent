using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TooLiRent.Services.DTOs;
using TooLiRent.Services.Interfaces;

namespace TooLiRent.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        // Hämtar vem som ringer (email + adminflagga)
        private (string? email, bool isAdmin) Caller()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                        ?? User.FindFirst("email")?.Value;
            return (email, User.IsInRole("Admin"));
        }

        /// <summary>
        /// Get all rentals (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RentalDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RentalDto>>> GetAll()
        {
            var rentals = await _rentalService.GetAllAsync();
            return Ok(rentals);
        }

        /// <summary>
        /// Get rental by ID (ägarkoll i service)
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RentalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RentalDto>> GetById(int id)
        {
            var (email, isAdmin) = Caller();
            try
            {
                var rental = await _rentalService.GetByIdAsync(id, email, isAdmin);
                if (rental == null)
                    return NotFound($"Rental with ID {id} not found");
                return Ok(rental);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Create a new rental (Member skapar åt sig själv, Admin kan skapa åt valfri kund)
        /// </summary>
        [Authorize(Roles = "Member,Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(RentalDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RentalDto>> Create(RentalCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (email, isAdmin) = Caller();
            try
            {
                var created = await _rentalService.CreateAsync(dto, email, isAdmin);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Update an existing rental 
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, RentalUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID i URL matchar inte ID i objektet.");

            var (email, isAdmin) = Caller();
            try
            {
                var updated = await _rentalService.UpdateAsync(dto, email, isAdmin);
                if (updated == null)
                    return NotFound($"Rental with ID {id} not found");

                return Ok(updated);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Mark a rental as returned (ägarkoll i service)
        /// </summary>
        [Authorize(Roles = "Member,Admin")]
        [HttpPost("{id}/return")]
        [ProducesResponseType(typeof(RentalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReturnRental(int id)
        {
            var (email, isAdmin) = Caller();
            try
            {
                var returned = await _rentalService.ReturnAsync(id, email, isAdmin);
                if (returned == null)
                    return NotFound($"Rental with ID {id} not found.");

                return Ok(returned);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Delete a rental (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _rentalService.DeleteAsync(id);
            if (!success)
                return NotFound($"Rental with ID {id} not found");

            return NoContent();
        }

    }
}
