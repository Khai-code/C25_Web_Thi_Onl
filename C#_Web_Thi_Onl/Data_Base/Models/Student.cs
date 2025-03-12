using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Data_Base.Models
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(14, ErrorMessage = "Mã học sinh không hợp lệ")]
        public string Student_Code { get; set; }
        [ForeignKey("Id")]
        public int User_Id { get; set; }
        [JsonIgnore]
        public virtual User Users { get; set; }
        [JsonIgnore]
        public virtual ICollection<Student_Class> Student_Classes { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room_Student> Exam_Room_Students { get; set; }
        [JsonIgnore]
        public virtual ICollection<Learning_Summary> Learning_Summaries { get; set; }
        [JsonIgnore]
        public virtual ICollection<Score> Scores { get; set; }
    }
}
