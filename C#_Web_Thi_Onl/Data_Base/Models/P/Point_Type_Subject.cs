using Data_Base.Models.S;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.P
{
    public class Point_Type_Subject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("Point_Type_Id")]
        [JsonIgnore]
        public virtual Point_Type Point_Types { get; set; }
        public int Point_Type_Id { get; set; }
        [ForeignKey("Subject_Id")]
        [JsonIgnore]
        public virtual Subject Subjects { get; set; }
        public int Subject_Id { get; set; }
    }
}
