using Data_Base.Models.S;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.E
{
    public class Exam_Room_Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Ngày tháng năm không được để trống")]
        [Range(19000101000000, 20991231235959, ErrorMessage = "Ngày tháng năm không hợp lệ")]
        public long Check_Time { get; set; }
        [ForeignKey("Exam_Room_Package_Id")]
        [JsonIgnore]
        public Exam_Room_Package? Exam_Room_Packages { get; set; }
        public int Exam_Room_Package_Id { get; set; }
        [ForeignKey("Student_Id")]
        [JsonIgnore]
        public Student? Students { get; set; }
        public int Student_Id { get; set; }
        [JsonIgnore]
        public ICollection<Exam_Room_Student_Answer_HisTory> Exam_Room_Student_Answer_HisTories { get; set; } = new List<Exam_Room_Student_Answer_HisTory>();
        [JsonIgnore]
        public ICollection<Exam_HisTory> Exam_HisTories { get; set; } = new List<Exam_HisTory>();
    }
}
