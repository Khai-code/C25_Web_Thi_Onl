using Data_Base.Models.G;
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
    public class Subject_Grade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Id")]
        public int Grade_Id { get; set; }
        [ForeignKey("Id")]
        public int Subject_Id { get; set; }
        [JsonIgnore]
        public virtual Subject Subjects { get; set; }
        [JsonIgnore]
        public virtual Grade Grades { get; set; }
    }
}
