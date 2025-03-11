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
    public class Exam_Room_Package
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Package_Id { get; set; }
        [ForeignKey("Id")]
        public int Exam_Room_Id { get; set; }
        [JsonIgnore]
        public virtual Package Packages { get; set; }
        [JsonIgnore]
        public virtual Exam_Room Exam_Rooms { get; set; }
    }
}
