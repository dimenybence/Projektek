using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeQuizGame.DataAccess.Models;
using RealtimeQuizGame.DataAccess.Services;
using RealtimeQuizGame.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace RealtimeQuizGame.WebApi.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUsersService _usersService;

        public UsersController(IMapper mapper, IUsersService usersService)
        {
            _mapper = mapper;
            _usersService = usersService;
        }

        /// <summary>
        /// Új felhasználó regisztrációja.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] UserRequestDto userRequestDto)
        {
            var user = _mapper.Map<User>(userRequestDto);
            await _usersService.AddUserAsync(user, userRequestDto.Password);
            var response = _mapper.Map<UserResponseDto>(user);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Bejelentkezés e-maillel és jelszóval.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var (authToken, refreshToken, userId) = await _usersService.LoginAsync(loginRequestDto.Email, loginRequestDto.Password);
            return Ok(new LoginResponseDto
            {
                UserId = userId,
                AuthToken = authToken,
                RefreshToken = refreshToken,
            });
        }

        /// <summary>
        /// Kijelentkezés (JWT érvénytelenítése a kliens oldalon ajánlott).
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            await _usersService.LogoutAsync();
            return NoContent();
        }

        /// <summary>
        /// Refresh token felhasználása.
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var (authToken, newRefreshToken, userId) = await _usersService.RedeemRefreshTokenAsync(refreshToken);
            return Ok(new LoginResponseDto
            {
                UserId = userId,
                AuthToken = authToken,
                RefreshToken = newRefreshToken,
            });
        }

        /// <summary>
        /// Bejelentkezett felhasználó adatai.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _usersService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserResponseDto>(user));
        }

        /// <summary>
        /// Bejelentkezett felhasználó adatai (ID szerint, csak saját ID engedélyezett).
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser([FromRoute][Required] string id)
        {
            var user = await _usersService.GetUserByIdAsync(id);
            return Ok(_mapper.Map<UserResponseDto>(user));
        }
    }
}

