using Biometric.Application.Services;
using Biometric.Infrastructure.Persistence;
using Biometric.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biometric.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        EnrollmentService enrollmentService,
        AuthenticationService authenticationService
    ) : ControllerBase
    {
        private readonly EnrollmentService _enrollmentService = enrollmentService;
        private readonly AuthenticationService _authenticationService = authenticationService;

        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll(
            [FromForm] string firstName,
            [FromForm] string lastName,
            [FromForm] string email,
            IFormFile file
        )
        {
            // Basic Validation
            if (file == null || file.Length == 0)
                return BadRequest("No image uploaded.");

            try
            {
                using var stream = file.OpenReadStream();
                var user = await _enrollmentService.EnrollUserAsync(
                    firstName,
                    lastName,
                    email,
                    stream,
                    file.FileName
                );

                return Ok(
                    new
                    {
                        user.UserId,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        Message = "Enrollment successful!",
                    }
                );
            }
            catch (Exception ex)
            {
                // Log this properly in production!
                return UnprocessableEntity(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        [Authorize]
        public async Task<IActionResult> Login(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No image uploaded.");

            try
            {
                using var stream = file.OpenReadStream();

                var token = await _authenticationService.LoginWithFaceAsync(stream, file.FileName);

                return Ok(new { Token = token, Message = "Login successful!" });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}
