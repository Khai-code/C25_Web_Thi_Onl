using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models
{
    public class Summary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(14, ErrorMessage = "Sai định dạng thời gian")]
        public int Start_Time { get; set; }
        [StringLength(14, ErrorMessage = "Sai định dạng thời gian")]
        public int End_Time { get; set; }
        [StringLength(14, ErrorMessage = "Mã kỳ không quá 14 ký tự")]
        public int Summary_Code { get; set; }
        [StringLength(50, ErrorMessage = "Tên kỳ không quá 50 ký tự")]
        public string Summary_Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<Point_Type> Point_Types { get; set; }
        [JsonIgnore]
        public virtual ICollection<Learning_Summary> Learning_Summaries { get; set; }
    }
}
