using Data_Base.Models.T;
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
    public class Student_Class
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Student_Id")]
        [JsonIgnore]
        public Student Students { get; set; }
        public int Student_Id { get; set; }
        [ForeignKey("Teacher_Id")]
        [JsonIgnore]
        public Teacher Teachers { get; set; }
        public int Teacher_Id { get; set; }


    }
}
