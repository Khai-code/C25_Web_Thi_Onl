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
        [ForeignKey("Id")]
        public int Summary_ID { get; set; }
        [ForeignKey("Id")]
        public int Student_Id { get; set; }
        [ForeignKey("Id")]
        public int Subject_Id { get; set; }
        [JsonIgnore]
        public virtual Subject Subjects { get; set; }
        [JsonIgnore]
        public virtual Student Students { get; set; }
        [JsonIgnore]
        public virtual Summary Summaries { get; set; }
    }
}
