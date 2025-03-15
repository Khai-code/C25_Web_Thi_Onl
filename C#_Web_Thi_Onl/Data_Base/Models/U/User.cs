using Data_Base.Models.R;
using Data_Base.Models.S;
using Data_Base.Models.T;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.U
{
    public class User
    {
        [Key]
        [StringLength(8, ErrorMessage = "Mã gói đề không quá 8 ký tự")]
        public int Id { get; set; }
        [StringLength(100, ErrorMessage = "Tên người dùng không được quá 100 ký tự")]
        public string Full_Name { get; set; }
        [StringLength(50, ErrorMessage = "User name không được quá 50 ký tự")]
        public string User_Name { get; set; }
        [StringLength(50, ErrorMessage = "User pass không được quá 50 ký tự")]
        public string User_Pass { get; set; }
        [StringLength(256, ErrorMessage = "Email không được quá 256 ký tự")]
        [EmailAddress(ErrorMessage = "Email ko đúng định dạng")]
        public string Email { get; set; }
        [StringLength(10, ErrorMessage = "Số điện thoại không được quá 10 ký tự")]
        public string Phone_Number { get; set; }
        [StringLength(14, ErrorMessage = "Ngày tháng năm không hợp lệ")]
        public int Data_Of_Birth { get; set; }
        [StringLength(14, ErrorMessage = "Ngày tháng năm không hợp lệ")]
        public int Create_Time { get; set; }
        [StringLength(14, ErrorMessage = "Ngày tháng năm không hợp lệ")]
        public int Last_Mordification_Time { get; set; }
        public string? Avatar { get; set; }
        public int Status { get; set; }
        [ForeignKey("Id")]
        public int Role_Id { get; set; }
        [JsonIgnore]
        public virtual Role Roles { get; set; }
        [JsonIgnore]
        public virtual ICollection<Student> Students { get; set; }
        [JsonIgnore]
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}
