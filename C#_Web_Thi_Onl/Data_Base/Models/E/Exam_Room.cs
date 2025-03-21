using Data_Base.Models.R;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.E
{
    public class Exam_Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Ngày tháng năm không được để trống")]
        [Range(19000101000000, 20991231235959, ErrorMessage = "Ngày tháng năm không hợp lệ")]
        public long Start_Time { get; set; }
        [Required(ErrorMessage = "Ngày tháng năm không được để trống")]
        [Range(19000101000000, 20991231235959, ErrorMessage = "Ngày tháng năm không hợp lệ")]
        public long End_Time { get; set; }
        [ForeignKey("Room_Id")]
        [JsonIgnore]
        public Room? Rooms { get; set; }
        public int Room_Id { get; set; }
        [ForeignKey("Exam_Id")]
        [JsonIgnore]
        public Exam? Exams { get; set; }
        public int Exam_Id { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room_Teacher> Exam_Room_Teachers { get; set; } = new List<Exam_Room_Teacher>();
        [JsonIgnore]
        public virtual ICollection<Exam_Room_Package> Exam_Room_Packages { get; set; } = new List<Exam_Room_Package>();
    }
}
