using Data_Base.Models.S;
using Data_Base.Models.U;

namespace Blazor_Server.Services
{
    public class Inforservice
    {
        private readonly HttpClient _httpClient;
        public Inforservice(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<listInforStudent?> GetInforStudent(int id)
        {
            try
            {
                var user = await _httpClient.GetFromJsonAsync<User>($"/api/User/GetBy/{id}");
                var student = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
                var data = student?.FirstOrDefault(x => x.User_Id == id);
                if (user == null)
                {
                    Console.WriteLine("Không tìm thấy dữ liệu User.");
                    return null;
                }

                return new listInforStudent
                {
                    Full_Name = user.Full_Name,
                    Email = user.Email,
                    Picture = user.Avatar,
                    DateofBirt = ConvertLongToDate(user.Data_Of_Birth).ToString("dd/MM/yyyy"),
                    NumberPhone = user.Phone_Number,
                    Status = user.Status,
                    codestudent = data?.Student_Code ?? "N/A",
                    Adrees = user?.Address ?? "N/A"
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin học sinh: {ex.Message}");
                return null;
            }
        }
        public static DateTime ConvertLongToDate(long dateLong)
        {
            string dateStr = dateLong.ToString();
            if (dateStr.Length != 8)
                throw new ArgumentException("Invalid date format. Expected ddMMyyyy.");
            int day = int.Parse(dateStr.Substring(0, 2)); 
            int month = int.Parse(dateStr.Substring(2, 2)); 
            int year = int.Parse(dateStr.Substring(4, 4)); 
            return new DateTime(year, month, day);
        }
        public class listInforStudent
        {
            public string Full_Name { get; set; }
            public string Email { get; set; }
            public string Picture { get; set; }
            public string DateofBirt { get; set; }
            public string NumberPhone { get; set; }
            public string Adrees { get; set; }
            public string codestudent { get; set; }
            public int Status { get; set; }
        }
    }
}
