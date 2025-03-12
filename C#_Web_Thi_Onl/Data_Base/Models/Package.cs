using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models
{
    public class Package // gói để
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Class_Id { get; set; }
        [ForeignKey("Id")]
        public int Subject_Id { get; set; }
        [ForeignKey("Id")]
        public int Point_Type_Id { get; set; }
        [StringLength(8, ErrorMessage = "Mã gói đề không quá 8 ký tự")]
        public int Package_Code { get; set; }
        [StringLength(50, ErrorMessage = "Tên gói đề không quá 50 ký tự")]
        public string Package_Name { get; set; }
        [StringLength(14, ErrorMessage = "Sai định dạng thời gian")]
        public int Create_Time { get; set; }
        public int Status { get; set; }
        [JsonIgnore]
        public virtual Subject Subjects { get; set; }
        [JsonIgnore]
        public virtual Class Classes { get; set; }
        [JsonIgnore]
        public virtual Point_Type Point_Types { get; set; }
        [JsonIgnore]
        public virtual ICollection<Question> Questions { get; set; }
        [JsonIgnore]
        public virtual ICollection<Package> Packages { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room_Package> Exam_Room_Packages { get; set; }
    }
}
