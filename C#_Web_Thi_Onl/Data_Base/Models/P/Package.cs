using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using Microsoft.Data.SqlClient.DataClassification;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.P
{
    public class Package // gói để
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Package_Code { get; set; }
        [StringLength(50, ErrorMessage = "Tên gói đề không quá 50 ký tự")]
        public string Package_Name { get; set; }
        [Required(ErrorMessage = "Ngày tháng năm không được để trống")]
        [Range(19000101000000, 20991231235959, ErrorMessage = "Ngày tháng năm không hợp lệ")]
        public long Create_Time { get; set; }
        public int Number_Of_Questions { get; set; }
        public int ExecutionTime { get; set; }
        public int Um_Lock { get; set; } = 0;
        public int Status { get; set; } = 0;
        [ForeignKey("Subject_Id")]
        [JsonIgnore]
        public Subject? Subjects { get; set; }
        public int Subject_Id { get; set; }
        [ForeignKey("Class_Id")]
        [JsonIgnore]
        public Class? Classes { get; set; }
        public int Class_Id { get; set; }
        [ForeignKey("Point_Type_Id")]
        [JsonIgnore]
        public Point_Type? Point_Types { get; set; }
        public int Point_Type_Id { get; set; }
        [ForeignKey("Package_Type_Id")]
        [JsonIgnore]
        public Package_Type? Package_Types { get; set; }
        public int Package_Type_Id { get; set; }
        [ForeignKey("Teacher_Id")]
        [JsonIgnore]
        public Teacher? Teachers { get; set; }
        public int Teacher_Id { get; set; }// giáo viên gia đề
        [JsonIgnore]
        public ICollection<Question>? Questions { get; set; } = new List<Question>();
        [JsonIgnore]
        public ICollection<Exam_Room_Package>? Exam_Room_Packages { get; set; } = new List<Exam_Room_Package>();
    }
}
