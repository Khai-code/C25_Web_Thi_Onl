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
        [ForeignKey("Id")]
        public int Answer_Id { get; set; }
        [ForeignKey("Id")]
        public int Exam_Room_Student_Id { get; set; }
        [JsonIgnore]
        public virtual Answers Answers { get; set; }
        [JsonIgnore]
        public virtual Exam_Room_Student Exam_Room_Students { get; set; }
    }
}
