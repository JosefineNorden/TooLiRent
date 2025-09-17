using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TooLiRent.Services.DTOs;
using TooLiRent.Services.Interfaces;

namespace TooLiRent.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        /// <summary>
        /// Get all rentals
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RentalDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RentalDto>>> GetAll()
        {
            var rentals = await _rentalService.GetAllAsync();
            return Ok(rentals);
        }

        /// <summary>
        /// Get rental by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RentalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RentalDto>> GetById(int id)
        {
            var rental = await _rentalService.GetByIdAsync(id);
            if (rental == null)
                return NotFound($"Rental with ID {id} not found");

            return Ok(rental);
        }

        /// <summary>
        /// Create a new rental
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RentalDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RentalDto>> Create(RentalCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _rentalService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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

            var updated = await _rentalService.UpdateAsync(dto);
            if (updated == null)
            {
                return NotFound($"Rental with ID {id} not found");
            }

            return Ok(updated);
        }

        /// <summary>
        /// Mark a rental as returned
        /// </summary>
        [HttpPost("{id}/return")]
        [ProducesResponseType(typeof(RentalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReturnRental(int id)
        {
            var returned = await _rentalService.ReturnAsync(id);
            if (returned == null)
                return NotFound($"Rental with ID {id} not found.");

            return Ok(returned);
        }

        /// <summary>
        /// Delete a rental
        /// </summary>
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
