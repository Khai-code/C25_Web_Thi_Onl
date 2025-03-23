using Data_Base.Models.U;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace ASP.NET.Controllers.A
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        public AuthController(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await AuthenticateUser(request.User_Name, request.User_Pass);

            if (user != null)
            {
                var tokenString = GenerateToken(user);

                return Ok(new
                {
                    Token = tokenString,
                    Id = user.Id,
                    Full_Name = user.Full_Name,
                    Role_Id = user.Role_Id,
                    Student_Code = user.Role_Id == 1 ? user.Students?.FirstOrDefault()?.Student_Code : null,
                    Teacher_Code = user.Role_Id == 2 ? user.Teachers?.FirstOrDefault()?.Teacher_Code : null
                });
            }

            return Unauthorized(new { Message = "Tên đăng nhập hoặc mật khẩu không chính xác!" });
        }

        private async Task<User?> AuthenticateUser(string User_Name, string User_Pass)
        {
            try
            {
                var apiUrl = "https://localhost:7187/api/User/Get";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<User>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return users?.FirstOrDefault(u => u.User_Name == User_Name && u.User_Pass == User_Pass);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi API: {ex.Message}");
                return null;
            }
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Full_Name", user.Full_Name),
                new Claim(JwtRegisteredClaimNames.Sub, user.User_Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Status", user.Status.ToString()),
                new Claim("Role", user.Role_Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (user.Role_Id == 1 && user.Students?.FirstOrDefault() is { } student)
            {
                claims.Add(new Claim("Student_Code", student.Student_Code));
            }
            else if (user.Role_Id == 2 && user.Teachers?.FirstOrDefault() is { } teacher)
            {
                claims.Add(new Claim("Teacher_Code", teacher.Teacher_Code));
            }

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("get-all-token-info")]
        public IActionResult GetAllTokenInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var claims = identity.Claims.Select(c => new { c.Type, c.Value }).ToList();
                return Ok(claims);
            }

            return Unauthorized();
        }

        public class LoginRequest
        {
            public string User_Name { get; set; }
            public string User_Pass { get; set; }
        }
    }
}
