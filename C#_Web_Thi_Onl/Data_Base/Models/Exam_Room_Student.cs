using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models
{
    public class Exam_Room_Student
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Student_Id { get; set; }
        [ForeignKey("Id")]
        public int Exam_Room_Package_Id { get; set; }
        [StringLength(14, ErrorMessage = "Không đúng định dạng thời gian")]
        public int Check_Time { get; set; }
        [JsonIgnore]
        public virtual Exam_Room_Package Exam_Room_Packages { get; set; }
        [JsonIgnore]
        public virtual Student Students { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room_Student_Answer_HisTory> Exam_Room_Student_Answer_HisTories { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_HisTory> Exam_HisTories { get; set; }
    }
}
