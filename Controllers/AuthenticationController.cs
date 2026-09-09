using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Training.Angular.Api.Application.Dtos;
using Training.Angular.Api.Application.Interfaces;

namespace Training.Angular.Api.Controllers;

[ApiController]
[Route("v1/authenticate")]
[AllowAnonymous]
public sealed class AuthenticationController(IAuthenticateUserUseCase authenticate) : ControllerBase
{
    [HttpPost]
    [Consumes("application/json")]
    public Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request, CancellationToken ct)
        => HandleAsync(request.Username, request.Password, ct);

    [HttpPost]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IActionResult> Authenticate([FromForm] string? username, [FromForm] string? password, CancellationToken ct)
        => HandleAsync(username, password, ct);

    private async Task<IActionResult> HandleAsync(string? username, string? password, CancellationToken ct)
    {
        var result = await authenticate.ExecuteAsync(username, password, ct);
        if (result is null)
            return BadRequest(new { error = "Invalid username or password" });

        return Ok(new
        {
            token = result.Token,
            expireDate = (int)Math.Max(0, (result.Expiry - DateTime.UtcNow).TotalSeconds),
            username
        });
    }
}
