using Data_Base.Models.S;
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
    public class Teacher_Subject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Teacher_id { get; set; }
        [ForeignKey("Id")]
        public int Subject_Id { get; set; }
        [JsonIgnore]
        public virtual Teacher Teachers { get; set; }
        [JsonIgnore]
        public virtual Subject Subjects { get; set; }
    }
}
