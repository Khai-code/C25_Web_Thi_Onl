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
    public class Teacher_Class
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Teacher_Id { get; set; }
        [ForeignKey("Id")]
        public int Class_Id { get; set; }
        [JsonIgnore]
        public virtual Class Classes { get; set; }
        [JsonIgnore]
        public virtual Teacher Teachers { get; set; }

    }
}
