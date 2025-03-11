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
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        [StringLength(8, ErrorMessage = "Mã giáo viên không hợp lệ")]
        public string Teacher_Code { get; set; }
        [ForeignKey("Id")]
        public int User_Id { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
        [JsonIgnore]
        public virtual ICollection<Teacher_Class> Teacher_Classes { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room_Teacher> Exam_Room_Teachers { get; set; }
        [JsonIgnore]
        public virtual ICollection<Teacher_Subject> Teacher_Subject { get; set; }
    }
}
