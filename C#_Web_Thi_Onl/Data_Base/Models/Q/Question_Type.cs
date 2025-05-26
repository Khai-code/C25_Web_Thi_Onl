using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Base.Models.P;
using System.Text.Json.Serialization;

namespace Data_Base.Models.Q
{
    public class Question_Type
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Question_Type_Name { get; set; }
        [ForeignKey("Package_Type_Id")]
        [JsonIgnore]
        public Package_Type? Package_Types { get; set; }
        public int Package_Type_Id { get; set; }
        [JsonIgnore]
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
