using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json.Serialization;

namespace Data_Base.Models
{
    public class Test
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(8, ErrorMessage = "Mã đề không quá 8 ký tự")]
        public string Test_Code { get; set; }
        public int Status { get; set; }
        [ForeignKey("Id")]
        public int Package_Id { get; set; }
        [JsonIgnore]
        public virtual Package Packages { get; set; }
        [JsonIgnore]
        public virtual ICollection<Test_Question> Test_Questions { get; set; }
    }
}
