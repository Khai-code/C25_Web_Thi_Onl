using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace Data_Base.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        [StringLength(20, ErrorMessage = "Tên mông không quá 20 ký tự")]
        public string Subject_Name { get; set; }
        [StringLength(8, ErrorMessage = "Mã môn không quá 8 ký tự")]
        public string Subject_Code { get; set; }
        [JsonIgnore]
        public virtual ICollection<Subject_Grade> Subject_Grade { get; set; }
        [JsonIgnore]
        public virtual ICollection<Package> Packages { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam> Exams { get; set; }
        [JsonIgnore]
        public virtual ICollection<Teacher_Subject> Teacher_Subject { get; set; }
        [JsonIgnore]
        public virtual ICollection<Score> Scores { get; set; }
        [JsonIgnore]
        public virtual ICollection<Learning_Summary> Learning_Summaries { get; set; }
        [JsonIgnore]
        public virtual ICollection<Point_Type_Subject> PointType_Subjects { get; set; }
    }
}
