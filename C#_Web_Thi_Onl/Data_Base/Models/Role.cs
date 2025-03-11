using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data_Base.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50, ErrorMessage = "Tên quyền không được quá 50 ký tự")]
        public string Role_Name { get; set; }
        [StringLength(14, ErrorMessage = "Mã code quá 14 ký tự")]
        public string Role_Code { get; set; }
        public int Status { get; set; }
        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }
    }
}
