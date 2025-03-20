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
    public class Exam_HisTory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public double Score { get; set; }
        [Required(ErrorMessage = "Ngày tháng năm không được để trống")]
        [Range(19000101000000, 20991231235959, ErrorMessage = "Ngày tháng năm không hợp lệ")]
        public long Create_Time { get; set; }
        [ForeignKey("Exam_Room_Student_Id")]
        [JsonIgnore]
        public Exam_Room_Student? Exam_Room_Students { get; set; }
        public int Exam_Room_Student_Id { get; set; }
    }
}
