using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.V_Model
{
    public class V_Student
    {
        public int Id { get; set; }
        public string Student_Code { get; set; }
        public int User_Id { get; set; }
        public string Full_Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone_Number { get; set; }
        public long Data_Of_Birth { get; set; }
        public long Create_Time { get; set; }
        public string Avatar { get; set; }
        public string Class_Code { get; set; }
        public string Class_Name { get; set; }
        public int Role_Id { get; set; }
        public string Role_Name { get; set; }
    }
}
