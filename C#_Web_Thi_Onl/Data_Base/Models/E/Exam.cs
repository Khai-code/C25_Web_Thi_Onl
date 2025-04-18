﻿using Data_Base.Models.S;
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
    public class Exam // đề thì 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(50, ErrorMessage = "Tên bài thi không quá 50 ký tự")]
        public string Exam_Name { get; set; }
        [ForeignKey("Subject_Id")]
        [JsonIgnore]
        public Subject? Subjects { get; set; }
        public int Subject_Id { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room> Exam_Rooms { get; set; } = new List<Exam_Room>();

    }
}
