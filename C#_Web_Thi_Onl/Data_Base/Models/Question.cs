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
    public class Question
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Package_Id { get; set; }
        [StringLength(4000, ErrorMessage = "Nội dung câu hỏi không quá 4000 ký tự")]
        public string Question_Name { get; set; }
        [StringLength(8, ErrorMessage = "Mã câu hỏi không quá 8 ký tự")]
        public string Question_Code { get; set; }
        public int Type { get; set; }
        public int Level { get; set; }
        [JsonIgnore]
        public virtual ICollection<Answers> Answerses { get; set; }
        [JsonIgnore]
        public virtual ICollection<Test_Question> Test_Questions { get; set; }
    }
}
