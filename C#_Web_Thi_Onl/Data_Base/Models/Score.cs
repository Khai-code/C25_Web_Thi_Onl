using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models
{
    public class Score
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Student_Id { get; set; }
        [ForeignKey("Id")]
        public int Subject_Id { get; set; }
        [ForeignKey("Id")]
        public int Point_Type_Id { get; set; }
        public double Point { get; set; }
        [JsonIgnore]
        public virtual Subject Subjects { get; set; }
        [JsonIgnore]
        public virtual Student Students { get; set; }
        [JsonIgnore]
        public virtual Point_Type Point_Types { get; set; }
    }
}
