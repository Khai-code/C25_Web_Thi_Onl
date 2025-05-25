using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Data_Base.Models.Q
{
    public class Question_Level
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Question_Level_Name { get; set; }
        [JsonIgnore]
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
