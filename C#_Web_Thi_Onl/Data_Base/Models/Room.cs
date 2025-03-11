using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models
{
    public class Room
    {
        [Key]
        public int ID { get; set; }
        [StringLength(20, ErrorMessage = "Tên phòng không quá 20 ký tự")]
        public string Room_Name { get; set; }
        [StringLength(8, ErrorMessage = "Mã phòng không quá 8 ký tự")]
        public string Room_Code { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room> Exam_Room { get; set; }
    }
}
