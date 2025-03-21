using Data_Base.Models.A;
using Data_Base.Models.P;
using Data_Base.Models.T;
using Data_Base.Models.U;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.Q
{
    public class Question
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(4000, ErrorMessage = "Nội dung câu hỏi không quá 4000 ký tự")]
        public string Question_Name { get; set; }
        [StringLength(14, ErrorMessage = "Mã câu hỏi không quá 14 ký tự")]
        public string Question_Code { get; set; }
        public int Type { get; set; }
        public int Level { get; set; }
        [ForeignKey("Package_Id")]
        [JsonIgnore]
        public Package? Packages { get; set; }
        public int Package_Id { get; set; }
        [JsonIgnore]
        public ICollection<Answers> Answerses { get; set; } = new List<Answers>();
        [JsonIgnore]
        public ICollection<Test_Question> Test_Questions { get; set; } = new List<Test_Question>();
    }
}
