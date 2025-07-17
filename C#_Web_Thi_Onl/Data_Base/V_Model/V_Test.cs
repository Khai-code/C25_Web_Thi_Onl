using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.V_Model
{
    public class V_Test
    {
        public int Id { get; set; }      
        public string Test_Code { get; set; }
        public int Package_Id { get; set; }          
        public int Package_Code { get; set; }
        public string Package_Name { get; set; }
        public string Class_Code { get; set; }
        public string Class_Name { get; set; }
        public int Point_Type_Id { get; set; }
        public string Point_Type_Name { get; set; }
        public int Subject_Id { get; set; }
        public string Subject_Name { get; set; }
        public int Total_Questions { get; set; }    
        public int Student_Id { get; set; }   
        public string Student_Code { get; set; }
        public string Full_Name { get; set; }
        public long Data_Of_Birth { get; set; }
        public string Email { get; set; }
        public string Phone_Number { get; set; }
        public string Address { get; set; }
        public long Start_Time { get; set; }
        public long End_Time { get; set; }
        public int Actual_Execution_Time { get; set; }
        public double Score { get; set; }
    }
}
