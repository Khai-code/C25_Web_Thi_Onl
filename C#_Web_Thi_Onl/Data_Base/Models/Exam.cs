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
    public class Exam // đề thì 
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Subject_Id { get; set; }
        [StringLength(50, ErrorMessage = "Tên bài thi không quá 50 ký tự")]
        public string Exam_Name { get; set; }
        [StringLength(8, ErrorMessage = "Mã bài thi không quá 8 ký tự")]
        public string Exam_Code { get; set; }
        [JsonIgnore]
        public virtual Subject Subjects { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room> Exam_Rooms { get; set; }

    }
}
