using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.V_Model
{
    public class V_Package
    {
        [Key]
        public int Id { get; set; }  // Từ bảng p (Package)
        public int Package_Code { get; set; }
        public string Package_Name { get; set; }
        public long Create_Time { get; set; }
        public int Number_Of_Questions { get; set; }
        public int ExecutionTime { get; set; }
        public int Um_Lock { get; set; }
        public int Status { get; set; }
        public int Subject_Id { get; set; }
        public string Subject_Name { get; set; }
        public int Room_Id { get; set; }
        public string Room_Name { get; set; }
        public int Package_Type_Id { get; set; }
        public string Package_Type_Name { get; set; }
        public int Point_Type_Id { get; set; }
        public string Point_Type_Name { get; set; }
        //public int User_Id { get; set; }
        //public string Full_Name { get; set; }
        //public string Email { get; set; }
        //public string Address { get; set; }
        //public string Phone_Number { get; set; }
        //public long Data_Of_Birth { get; set; }
        //public int Teacher_Id { get; set; }
        //public string Teacher_Code { get; set; }
        public int Class_Id { get; set; }
        public string Class_Code { get; set; }
        public string Class_Name { get; set; }
        public int Number { get; set; }
        public int Exam_Room_Package_Id { get; set; }
        //public int Exam_Room_Teacher_Id { get; set; }
        public int Exam_Room_Id { get; set; }
        public long Start_Time { get; set; }
        public long End_Time { get; set; }
        public int Exam_Id { get; set; }
        public string Exam_Name { get; set; }
        //GV1 - giám thị 1(Giáo viên chủ nhiệm)
        public int Exam_Room_Teacher1_Id { get; set; }
        public int GV1_Id { get; set; }
        public string GV1_Code { get; set; }
        public int GV1_User_Id { get; set; }
        public string GV1_Name { get; set; }
        public string GV1_Email { get; set; }
        public string GV1_Phone { get; set; }
        public string GV1_Address { get; set; }
        public long GV1_DoB { get; set; }
        public int GV1_Confirm { get; set; }
        //GV1 - giám thị 2
        public int Exam_Room_Teacher2_Id { get; set; }
        public int GV2_Id { get; set; }
        public string GV2_Code { get; set; }
        public int GV2_User_Id { get; set; }
        public string GV2_Name { get; set; }
        public string GV2_Email { get; set; }
        public string GV2_Phone { get; set; }
        public string GV2_Address { get; set; }
        public long GV2_DoB { get; set; }
        public int GV2_Confirm { get; set; }
        //GV ra đề
        public int TeacherPackage_Id { get; set; }
        public string TeacherPackage_Code { get; set; }
        public int TeacherPackage_User_Id { get; set; }
        public string TeacherPackage_Name { get; set; }
        public string TeacherPackage_Email { get; set; }
        public string TeacherPackage_Phone { get; set; }
        public string TeacherPackage_Address { get; set; }
        public long TeacherPackage_DoB { get; set; }

        //Giáo viên chấm thi
        public int? TeacherMark_Id { get; set; }
        public string? TeacherMark_Code { get; set; }
        public int? TeacherMark_User_Id { get; set; }
        public string? TeacherMark_Name { get; set; }
        public string? TeacherMark_Email { get; set; }
        public string? TeacherMark_Phone { get; set; }
        public string? TeacherMark_Address { get; set; }
        public long? TeacherMark_DoB { get; set; }
    }
}
