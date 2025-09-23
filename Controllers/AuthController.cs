using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TooLiRent.Services.DTOs;
using TooLiRent.Services.Interfaces;

namespace TooLiRent.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        /// <summary>Logga in och få en JWT</summary>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _auth.LoginAsync(dto);
            if (token is null) return Unauthorized("Ogiltig e-post eller lösenord.");
            return Ok(new { token });
        }

        [Authorize(Roles = "Admin")] // Se alla Admins
        [HttpGet("users/admins")]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _auth.GetAdminsAsync();
            return Ok(admins);
        }

        [Authorize(Roles = "Admin")] // Se alla Members
        [HttpGet("users/members")]
        public async Task<IActionResult> GetMembers()
        {
            var members = await _auth.GetMembersAsync();
            return Ok(members);
        }

        /// <summary>Registrera ny Admin-användare</summary>
        [Authorize(Roles = "Admin")] 
        [HttpPost("register-admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto dto)
        {
            var (ok, errors) = await _auth.RegisterAdminAsync(dto);
            if (!ok) return BadRequest(new { errors });
            return StatusCode(StatusCodes.Status201Created, "Admin created successfully");
        }

        /// <summary>Registrera ny Member-användare</summary>
        [AllowAnonymous]
        [HttpPost("register-member")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterMember([FromBody] RegisterDto dto)
        {
            var (ok, errors) = await _auth.RegisterMemberAsync(dto);
            if (!ok) return BadRequest(new { errors });
            return StatusCode(StatusCodes.Status201Created, "Member created successfully");
        }

        /// <summary>Ta bort en användare (endast Admin)</summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var (ok, errors) = await _auth.DeleteUserAsync(email);
            if (!ok) return BadRequest(new { errors });
            return Ok($"User {email} deleted successfully");
        }

        /// <summary>Info om inloggad användare</summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Me()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirst("email")?.Value;
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            return Ok(new { id, email, roles });
        }
    }

}
