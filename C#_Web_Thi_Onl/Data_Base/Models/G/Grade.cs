using Data_Base.Models.C;
using Data_Base.Models.S;
using Data_Base.Models.U;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models.G
{
    public class Grade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(10, ErrorMessage = "Tên khối không quá 10 ký tự")]
        public string Grade_Name { get; set; }
        [JsonIgnore]
        public ICollection<Class> Classes { get; set; } = new List<Class>();

    }
}
