using Data_Base.Models.L;
using Data_Base.Models.P;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.S
{
    public class Summary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Ngày tháng năm không được để trống")]
        [Range(19000101000000, 20991231235959, ErrorMessage = "Ngày tháng năm không hợp lệ")]
        public long Start_Time { get; set; }
        [Required(ErrorMessage = "Ngày tháng năm không được để trống")]
        [Range(19000101000000, 20991231235959, ErrorMessage = "Ngày tháng năm không hợp lệ")]
        public long End_Time { get; set; }
        [StringLength(50, ErrorMessage = "Tên kỳ không quá 50 ký tự")]
        public string Summary_Name { get; set; }
        [JsonIgnore]
        public ICollection<Score> Scores { get; set; } = new List<Score>();
        [JsonIgnore]
        public ICollection<Learning_Summary> Learning_Summaries { get; set; } = new List<Learning_Summary>();
    }
}
