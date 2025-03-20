using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Data_Base.Models.S;

namespace Data_Base.Models.L
{
    public class Learning_Summary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double Attendance { get; set; }
        public double Point_15 { get; set; }
        public double Point_45 { get; set; }
        public double Point_Midterm { get; set; }
        public double Point_Final { get; set; }
        public double Point_Summary { get; set; }
        [ForeignKey("Subject_Id")]
        [JsonIgnore]
        public Subject? Subjects { get; set; }
        public int Subject_Id { get; set; }
        [ForeignKey("Student_Id")]
        [JsonIgnore]
        public Student? Students { get; set; }
        public int Student_Id { get; set; }
        [ForeignKey("Summary_ID")]
        [JsonIgnore]
        public Summary? Summaries { get; set; }
        public int Summary_ID { get; set; }
    }
}
