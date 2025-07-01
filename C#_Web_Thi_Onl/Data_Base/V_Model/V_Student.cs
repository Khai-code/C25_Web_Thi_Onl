using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.V_Model
{
    public class V_Student
    {
        [Key]
        public int Id { get; set; } // Students.Id
        public string Student_Code { get; set; }

        // User
        public int User_Id { get; set; }
        public string Full_Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone_Number { get; set; }
        public long Data_Of_Birth { get; set; }
        public string Avatar { get; set; }

        // Role
        public int Role_Id { get; set; }
        public string Role_Name { get; set; }

        // Student-Class
        public int Student_Class_Id { get; set; }

        // Class
        public int Class_Id { get; set; }
        public string Class_Code { get; set; }
        public string Class_Name { get; set; }
    }
}
