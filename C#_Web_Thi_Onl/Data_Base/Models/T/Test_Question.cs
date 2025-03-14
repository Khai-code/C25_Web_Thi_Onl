using Data_Base.Models.Q;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.T
{
    public class Test_Question
    {
        [Key]
        [StringLength(8, ErrorMessage = "Mã gói đề không quá 8 ký tự")]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Question_Id { get; set; }
        [ForeignKey("Id")]
        public int Test_Id { get; set; }
        [JsonIgnore]
        public virtual Test Tests { get; set; }
        [JsonIgnore]
        public virtual Question Questions { get; set; }
    }
}
