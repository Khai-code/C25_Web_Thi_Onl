using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.R;
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

namespace Data_Base.Models.T
{
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Teacher_Code { get; set; }
        public int Position { get; set; } //chưc vụ 0 admin 1 giáo viên
        [ForeignKey("User_Id")]
        [JsonIgnore]
        public User? User { get; set; }
        public int User_Id { get; set; }
        [ForeignKey("Subject_Id")]
        [JsonIgnore]
        public Subject? Subjects { get; set; }
        public int? Subject_Id { get; set; } // môn dạy
        [JsonIgnore]
        public ICollection<Exam_Room_Teacher> Exam_Room_Teachers { get; set; } = new List<Exam_Room_Teacher>();
        [JsonIgnore]
        public ICollection<Review_Test> Review_Tests { get; set; } = new List<Review_Test>();
        [JsonIgnore]
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}
