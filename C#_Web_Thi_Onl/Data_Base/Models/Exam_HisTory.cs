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
    public class Exam_HisTory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Id")]
        public int Exam_Room_Student_Id { get; set; }
        public double Score { get; set; }
        [StringLength(14, ErrorMessage = "Sai định dạng thời gian")]
        public int Create_Time { get; set; }
        [JsonIgnore]
        public virtual Exam_Room_Student Exam_Room_Students { get; set; }
    }
}
