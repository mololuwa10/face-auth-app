using Biometric.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Biometric.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(EnrollmentService enrollmentService) : ControllerBase
    {
        private readonly EnrollmentService _enrollmentService = enrollmentService;

        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll(
            [FromForm] string firstName,
            [FromForm] string lastName,
            [FromForm] string email,
            IFormFile file
        )
        {
            // Reliability - Basic Validation
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
                        user.Email,
                        Message = "Enrollment successful!",
                    }
                );
            }
            catch (Exception ex)
            {
                // PILLAR 7: Observability - Log this properly in production!
                return UnprocessableEntity(new { error = ex.Message });
            }
        }
    }
}
