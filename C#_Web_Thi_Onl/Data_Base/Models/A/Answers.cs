using Data_Base.Models.E;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.U;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.A
{
    public class Answers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Answers_Name { get; set; }
        public int? Right_Answer { get; set; }
        [ForeignKey("Question_Id")]
        [JsonIgnore]
        public Question? Questions { get; set; }
        public int Question_Id { get; set; }
        [JsonIgnore]
        public ICollection<Exam_Room_Student_Answer_HisTory> Exam_Room_Student_Answer_HisTories { get; set; } = new List<Exam_Room_Student_Answer_HisTory>();
    }
}
