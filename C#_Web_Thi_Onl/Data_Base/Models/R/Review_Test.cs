using Data_Base.Models.S;
using Data_Base.Models.T;
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
    public class Review_Test
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Test_Id")]
        [JsonIgnore]
        public Test? Tests { get; set; }
        public int? Test_Id { get; set; }
        [ForeignKey("Student_Id")]
        [JsonIgnore]
        public Student? Students { get; set; }
        public int? Student_Id { get; set; }
        [ForeignKey("Teacher_Id")]
        [JsonIgnore]
        public Teacher? Teachers  { get; set; }
        public int? Teacher_Id { get; set; }
        public string Reason_For_Sending {  get; set; }
        public string Reason_For_Refusal { get; set; }
        public int Status { get; set; }
    }
}
