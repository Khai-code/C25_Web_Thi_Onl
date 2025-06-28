using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.V_Model
{
    public class V_Package
    {
        public int Id { get; set; }//
        public int Package_Code { get; set; }//
        public string Package_Name { get; set; }//
        public long Create_Time { get; set; }//
        public int Number_Of_Questions { get; set; }//
        public int ExecutionTime { get; set; } //
        public int Point_Type_Id { get; set; }//
        public string Point_Type_Name { get; set; }//
        public int Package_Type_Id { get; set; }//
        public string Package_Type_Name { get; set; }//
        public int Subject_Id { get; set; }//
        public string Subject_Name { get; set; }//
        public string Class_Code { get; set; }//
        public string Class_Name { get; set; }//
        public int Exam_Room_Id { get; set; }//
        public long Start_Time { get; set; }//
        public long End_Time { get; set; }//
        public int Exam_Id { get; set; }//
        public string Exam_Name { get; set; }//
        public int Room_Id { get; set; }//
        public string Room_Name { get; set; }//
        public string Teacher_Code { get; set; }//
        public string Teacher_Full_Name { get; set; }//
        public string Teacher_Email { get; set; }//
        public string Teaacher_Address { get; set; }//
        public string Teacher_Phone_Number { get; set; }//
        public long Teacher_Data_Of_Birth { get; set; }//
        public string Teacher_Avatar { get; set; }//
        public int Users_Id { get; set; }//
    }
}
