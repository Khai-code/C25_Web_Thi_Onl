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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Question_Id")]
        [JsonIgnore]
        public Question? Questions { get; set; }
        public int Question_Id { get; set; }
        [ForeignKey("Test_Id")]
        [JsonIgnore]
        public Test? Tests { get; set; }
        public int Test_Id { get; set; }
    }
}
