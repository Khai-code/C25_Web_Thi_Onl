using Data_Base.Models.P;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.S
{
    public class Score
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Student_Id")]
        [JsonIgnore]
        public Student Students { get; set; }
        public int Student_Id { get; set; }
        [ForeignKey("Subject_Id")]
        [JsonIgnore]
        public Subject Subjects { get; set; }
        public int Subject_Id { get; set; }
        [ForeignKey("Point_Type_Id")]
        [JsonIgnore]
        public Point_Type Point_Types { get; set; }
        public int Point_Type_Id { get; set; }
        public double Point { get; set; }
        
        
        
    }
}
