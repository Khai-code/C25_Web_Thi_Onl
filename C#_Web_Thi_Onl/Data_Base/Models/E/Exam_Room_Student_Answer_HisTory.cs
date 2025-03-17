using Data_Base.Models.A;
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
    public class Exam_Room_Student_Answer_HisTory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Answer_Id")]
        [JsonIgnore]
        public Answers Answers { get; set; }
        public int Answer_Id { get; set; }
        [ForeignKey("Exam_Room_Student_Id")]
        [JsonIgnore]
        public Exam_Room_Student Exam_Room_Students { get; set; }
        public int Exam_Room_Student_Id { get; set; }
    }
}
