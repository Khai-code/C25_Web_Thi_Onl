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
        [ForeignKey("Grade_Id")]
        [JsonIgnore]
        public Grade Grades { get; set; }
        public int Grade_Id { get; set; }
        [ForeignKey("Subject_Id")]
        [JsonIgnore]
        public Subject Subjects { get; set; }
        public int Subject_Id { get; set; }
    }
}
