using Data_Base.Models.R;
using Data_Base.Models.S;
using Data_Base.Models.T;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.DTO_Import_Excel
{
    public class UserStudentImportDTO
    {
        public string Full_Name { get; set; }
        public string User_Name { get; set; }
        public string User_Pass { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone_Number { get; set; }
        public long Data_Of_Birth { get; set; }
        public long Create_Time { get; set; }
        public long Last_Mordification_Time { get; set; }
        public string? Avatar { get; set; }
        public int Status { get; set; }
        public int Role_Id { get; set; }
    }
}
