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
    public class Exam_Room
    {
        [Key]
        public int Id { get; set; }
        [StringLength(14, ErrorMessage = "Sai định dạng thời gian")]
        public int Start_Time { get; set; }
        [StringLength(14, ErrorMessage = "Sai định dạng thời gian")]
        public int End_Time { get; set; }
        [ForeignKey("Id")]
        public Guid Room_Id { get; set; }
        [ForeignKey("Id")]
        public Guid Exam_Id { get; set; }
        [JsonIgnore]
        public virtual Room Rooms { get; set; }
        [JsonIgnore]
        public virtual Exam Exams { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room_Teacher> Exam_Room_Teachers { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room_Package> Exam_Room_Packages { get; set; }
    }
}
