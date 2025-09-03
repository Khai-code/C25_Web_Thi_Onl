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

        public async Task<LoginResult> Login(string username, string password)
        {

            try
            {
                var loginRequest = new { User_Name = username, User_Pass = password };

                var response = await _httpClient.PostAsJsonAsync($"https://localhost:7187/api/Auth/login", loginRequest);
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Lỗi từ server: {errorContent}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResult>();

                    // 🔥 Giải mã token để lấy Role_Id
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(result.Token) as JwtSecurityToken;
                    var roleClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "Role");

                    int role = 0; // Mặc định nếu không tìm thấy role
                    if (roleClaim != null && int.TryParse(roleClaim.Value, out int parsedRole))
                    {
                        role = parsedRole;
                    }

                    return new LoginResult { Token = result.Token, Role = role };
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class LoginResult
    {
        public string Token { get; set; }
        public int Role { get; set; }
    }
}

