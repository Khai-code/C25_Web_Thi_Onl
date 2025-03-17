using ASP.NET.Controllers.A;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Blazor_Server.Services
{
    public class AuthSerrvice
    {
        private readonly HttpClient _httpClient;

        public AuthSerrvice(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> Login(string username, string password)
        {
            var loginRequest = new { User_Name = username, User_Pass = password };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                // 🔥 Giải mã token để lấy Role_Id
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(result.Token) as JwtSecurityToken;
                var roleClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "Role");

                if (roleClaim != null && int.TryParse(roleClaim.Value, out int roleId))
                {
                    // 👉 Điều hướng dựa trên Role_Id
                    if (roleId == 1)
                    {
                        return "student"; // Học sinh
                    }
                    else if (roleId == 2)
                    {
                        return "teacher"; // Giáo viên
                    }
                }

                return "unknown"; // Không có role
            }

            return null;
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }
}

