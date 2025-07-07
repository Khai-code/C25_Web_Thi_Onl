using Data_Base.Models.E;
using Data_Base.Models.L;
using Data_Base.Models.R;
using Data_Base.Models.U;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Data_Base.Models.S
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Student_Code { get; set; }
        [ForeignKey("User_Id")]
        [JsonIgnore]
        public User? Users { get; set; }
        public int User_Id { get; set; }
        [JsonIgnore]
        public ICollection<Student_Class> Student_Classes { get; set; } = new List<Student_Class>();
        [JsonIgnore]
        public ICollection<Exam_Room_Student> Exam_Room_Students { get; set; } = new List<Exam_Room_Student>();
        [JsonIgnore]
        public ICollection<Learning_Summary> Learning_Summaries { get; set; } = new List<Learning_Summary>();
        [JsonIgnore]
        public ICollection<Score> Scores { get; set; } = new List<Score>();
        [JsonIgnore]
        public ICollection<Review_Test> Review_Tests { get; set; } = new List<Review_Test>();
    }
}
