using Identity.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;



namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto) 
    {
        try 
        {
            return Ok(await _authService.RegisterAsync(dto));
        
        
        }
        catch (Exception ex) { return BadRequest(ex.Message); }


    
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto) 
    {
        try
        {
            return Ok(await _authService.LoginAsync(dto));
        
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }





}
