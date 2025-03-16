using Data_Base.Models.L;
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

namespace Data_Base.Models.P
{
    public class Point_Type
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Summary_Id { get; set; }
        [StringLength(20, ErrorMessage = "Tên kiêu điểm không quá 20 ký tự")]
        public string Point_Type_Name { get; set; }
        [JsonIgnore]
        public virtual Summary Summaries { get; set; }
        [JsonIgnore]
        public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
        [JsonIgnore]
        public virtual ICollection<Learning_Summary> Learning_Summaries { get; set; } = new List<Learning_Summary>();
        [JsonIgnore]
        public virtual ICollection<Point_Type_Subject> Point_Type_Subjects { get; set; } = new List<Point_Type_Subject>();
        [JsonIgnore]
        public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    }
}
