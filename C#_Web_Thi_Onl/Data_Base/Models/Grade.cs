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
    public class Grade
    {
        [Key]
        public int ID { get; set; }
        [StringLength(10, ErrorMessage = "Tên khối không quá 10 ký tự")]
        public string Grade_Name { get; set; }
        [StringLength(8, ErrorMessage = "Mã lớp không quá 8 ký tự")]
        public string Grade_Code { get; set; }
        [JsonIgnore]
        public virtual ICollection<Subject_Grade> Subject_Grades { get; set; }
        [JsonIgnore]
        public virtual ICollection<Class> Classes { get; set; }

    }
}
