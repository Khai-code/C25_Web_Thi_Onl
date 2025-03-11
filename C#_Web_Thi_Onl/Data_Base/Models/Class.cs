using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Grade_Id { get; set; }
        [ForeignKey("Id")]
        public int Teacher_Id { get; set; }
        [StringLength(8, ErrorMessage = "Mã lớp không hợp lệ")]
        public string Class_Code { get; set; }
        [StringLength(10, ErrorMessage = "Tên lớp không quá 10 ký tự")]
        public string Class_Name { get; set; }
        [StringLength(50, ErrorMessage = "Một lớp không qua 50 học sinh")]
        public int Max_Student {  get; set; }
        [JsonIgnore]
        public virtual Grade Grades { get; set; }
        [JsonIgnore]
        public virtual ICollection<Student_Class> Student_Classes { get; set; }
        [JsonIgnore]
        public virtual ICollection<Teacher_Class> Teacher_Classes { get; set; }



    }
}
