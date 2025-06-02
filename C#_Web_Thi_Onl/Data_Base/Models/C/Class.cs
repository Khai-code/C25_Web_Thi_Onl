using Data_Base.Models.G;
using Data_Base.Models.S;
using Data_Base.Models.T;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.C
{
    public class Class
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Class_Code { get; set; }
        [StringLength(10, ErrorMessage = "Tên lớp không quá 10 ký tự")]
        public string Class_Name { get; set; }
        [Range(1, 50, ErrorMessage = "Một lớp không quá 50 học sinh")]
        public int Max_Student { get; set; }
        public int Number {  get; set; }
        [ForeignKey("Grade_Id")]
        [JsonIgnore]
        public Grade? Grades { get; set; }
        public int Grade_Id { get; set; }
        [ForeignKey("Teacher_Id")]
        [JsonIgnore]
        public Teacher? Teachers { get; set; }
        public int Teacher_Id { get; set; }
        [JsonIgnore]
        public ICollection<Student_Class> Student_Classes { get; set; } = new List<Student_Class>();
    }
}
