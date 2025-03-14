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
        public async Task<IActionResult> Login([FromBody] User loginModel)
        {
            var user = await AuthenticateUser(loginModel);

            if (user != null)
            {
                var tokenString = GenerateToken(user);

                // Kiểm tra Role_Id để lấy mã sinh viên hoặc giáo viên
                string? studentCode = null;
                string? teacherCode = null;

                if (user.Role_Id == 1) // học sinh
                {
                    studentCode = user.Students?.FirstOrDefault()?.Student_Code;
                }
                else if (user.Role_Id == 2) // giáo viên
                {
                    teacherCode = user.Teachers?.FirstOrDefault()?.Teacher_Code;
                }

                return Ok(new
                {
                    Token = tokenString,
                    Id = user.Id,
                    Full_Name = user.Full_Name,
                    Role_Id = user.Role_Id,
                    Student_Code = studentCode,
                    Teacher_Code = teacherCode
                });
            }

            return Unauthorized();
        }

        private async Task<User> AuthenticateUser(User loginModel)
        {
            var apiUrl = "https://localhost:7187/api/User/Get"; 

            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<User>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return users?.FirstOrDefault(u => u.User_Name == loginModel.User_Name && u.User_Pass == loginModel.User_Pass);
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

            // Kiểm tra Role_Id để lấy Student_Code hoặc Teacher_Code
            if (user.Role_Id == 1) // Học sinh
            {
                var studentCode = user.Students?.FirstOrDefault()?.Student_Code;
                if (!string.IsNullOrEmpty(studentCode))
                {
                    claims.Add(new Claim("Student_Code", studentCode));
                }
            }
            else if (user.Role_Id == 2) // Giáo viên
            {
                var teacherCode = user.Teachers?.FirstOrDefault()?.Teacher_Code;
                if (!string.IsNullOrEmpty(teacherCode))
                {
                    claims.Add(new Claim("Teacher_Code", teacherCode));
                }
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
    }
}
