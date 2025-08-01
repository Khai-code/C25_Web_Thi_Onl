﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json.Serialization;
using Data_Base.Models.P;
using Data_Base.Models.E;
using Data_Base.Models.S;
using Data_Base.Models.R;

namespace Data_Base.Models.T
{
    public class Test
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Test_Code { get; set; }
        public int Status { get; set; }
        [ForeignKey("Package_Id")]
        [JsonIgnore]
        public Package? Packages { get; set; }
        public int Package_Id { get; set; }
        [ForeignKey("Student_Id")]
        [JsonIgnore]
        public Student? Student { get; set; }
        public int Student_Id { get; set; }
        [JsonIgnore]
        public ICollection<Test_Question> Test_Questions { get; set; } = new List<Test_Question>();
        [JsonIgnore]
        public ICollection<Exam_Room_Student> Exam_Room_Students { get; set; } = new List<Exam_Room_Student>();
        [JsonIgnore]
        public ICollection<Review_Test> Review_Tests { get; set; } = new List<Review_Test>();
        [JsonIgnore]
        public ICollection<Score> Scores { get; set; } = new List<Score>();
    }
}
