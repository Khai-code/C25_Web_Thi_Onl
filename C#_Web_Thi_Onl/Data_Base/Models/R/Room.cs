﻿using Data_Base.Models.E;
using Data_Base.Models.U;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.R
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(20, ErrorMessage = "Tên phòng không quá 20 ký tự")]
        public string Room_Name { get; set; }
        [StringLength(14, ErrorMessage = "Mã phòng không quá 14 ký tự")]
        public string Room_Code { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room> Exam_Room { get; set; } = new List<Exam_Room>();
    }
}
