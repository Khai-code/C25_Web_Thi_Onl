using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Data_Base.Models.Q;

namespace Data_Base.Models.P
{
    public class Package_Type
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(20, ErrorMessage = "Tên loại gói đề không quá 20 ký tự")]
        public string Package_Type_Name { get; set; }
        [JsonIgnore]
        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
