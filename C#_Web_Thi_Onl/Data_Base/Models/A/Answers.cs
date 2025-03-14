﻿using Data_Base.Models.E;
using Data_Base.Models.Q;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.A
{
    public class Answers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int Question_Id { get; set; }
        [StringLength(4000, ErrorMessage = "Nội dung đáp án đề không quá 4000 ký tự")]
        public string Answers_Name { get; set; }
        [StringLength(14, ErrorMessage = "Mã đáp án không quá 14 ký tự")]
        public string Answers_Code { get; set; }
        public int Right_Answer { get; set; }
        [JsonIgnore]
        public virtual Question Questions { get; set; }
        [JsonIgnore]
        public virtual ICollection<Exam_Room_Student_Answer_HisTory> Exam_Room_Student_Answer_HisTories { get; set; }
    }
}
