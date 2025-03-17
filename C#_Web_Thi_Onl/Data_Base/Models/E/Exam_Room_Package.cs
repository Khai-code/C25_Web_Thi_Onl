using Data_Base.Models.P;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.E
{
    public class Exam_Room_Package
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Package_Id")]
        [JsonIgnore]
        public Package Packages { get; set; }
        public int Package_Id { get; set; }
        [ForeignKey("Exam_Room_Id")]
        [JsonIgnore]
        public Exam_Room Exam_Rooms { get; set; }
        public int Exam_Room_Id { get; set; }
    }
}
