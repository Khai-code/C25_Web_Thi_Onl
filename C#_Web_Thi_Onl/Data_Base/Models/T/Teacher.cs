using Data_Base.Models.E;
using Data_Base.Models.P;
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
        [ForeignKey("User_Id")]
        [JsonIgnore]
        public User? User { get; set; }
        public int User_Id { get; set; }
        [JsonIgnore]
        public ICollection<Exam_Room_Teacher> Exam_Room_Teachers { get; set; } = new List<Exam_Room_Teacher>();
        [JsonIgnore]
        public ICollection<Teacher_Subject> Teacher_Subject { get; set; } = new List<Teacher_Subject>();
    }
}
