using Data_Base.Models.E;
using Data_Base.Models.L;
using Data_Base.Models.P;
using Data_Base.Models.T;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace Data_Base.Models.S
{
    public class Subject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(20, ErrorMessage = "Tên môn không quá 20 ký tự")]
        public string Subject_Name { get; set; }
        [JsonIgnore]
        public ICollection<Package> Packages { get; set; } = new List<Package>();
        [JsonIgnore]
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        [JsonIgnore]
        public ICollection<Score> Scores { get; set; } = new List<Score>();
        [JsonIgnore]
        public ICollection<Learning_Summary> Learning_Summaries { get; set; } = new List<Learning_Summary>();
        [JsonIgnore]
        public ICollection<Point_Type_Subject> PointType_Subjects { get; set; } = new List<Point_Type_Subject>();
    }
}
