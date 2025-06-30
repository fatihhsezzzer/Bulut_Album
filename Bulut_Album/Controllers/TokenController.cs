using Bulut_Album.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly TokenService _tokenService;

    public TokenController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpGet("{customerId}")]
    public IActionResult GetToken(string customerId)
    {
        var token = _tokenService.GenerateCustomerUploadToken(customerId);
        return Ok(new { token });
    }
}
