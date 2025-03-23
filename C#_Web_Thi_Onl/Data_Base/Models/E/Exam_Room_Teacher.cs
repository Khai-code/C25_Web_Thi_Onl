using Data_Base.Models.T;
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
    public class Exam_Room_Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Teacher_Id")]
        [JsonIgnore]
        public Teacher? Teachers { get; set; }
        public int Teacher_Id { get; set; }
        [ForeignKey("Exam_Room")]
        [JsonIgnore]
        public Exam_Room? Exam_Rooms { get; set; }
        public int Exam_Room { get; set; }
    }
}
