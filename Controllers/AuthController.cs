using LdapAuthApi.Models;
using LdapAuthApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly LdapAuthenticationService _ldapService;  
    private readonly JwtService _jwtService;   
    private readonly LoginService _loginService; 
    public AuthController(LdapAuthenticationService ldapService, JwtService jwtService, LoginService loginService)
    {
        _ldapService = ldapService;  
        _jwtService = jwtService;   
        _loginService = loginService;
    }

    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] AuthRequest request)
    {
        var isAuthenticated = _ldapService.AuthenticateUser(request.Username, request.Password);

        if (isAuthenticated)
        {
            var token = _jwtService.GenerateJwtToken(request.Username);

            var user = new
            {
                username = request.Username,
                email = $"{request.Username}@example.com",  
                displayName = "John Doe", 
                department = "IT",         
                title = "Software Engineer"
            };

            _loginService.SaveLoginResponse(user.username, user.email, user.displayName,user.department, user.title);
            return Ok(new
            {
                status = "success",
                user = user,
                token = token 
            });
        }

        return Unauthorized(new
        {
            status = "error",
            message = "Invalid credentials or user not found."
        });
    }
}
