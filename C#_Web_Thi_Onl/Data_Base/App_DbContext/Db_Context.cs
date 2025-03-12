﻿using Data_Base.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.App_DbContext
{
    public class Db_Context : DbContext
    {
        public Db_Context()
        {
            
        }

        public Db_Context(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student_Class> Student_Classes { get; set; }
        public DbSet<Exam_Room_Student> Exam_Room_Students { get; set; }
        public DbSet<Exam_Room_Student_Answer_HisTory> Exam_Room_Student_Answer_HisTories { get; set; }
        public DbSet<Teacher_Subject> Teacher_Subjects { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Subject_Grade> Subject_Grades { get; set; }
        public DbSet<Point_Type_Subject> Point_Type_Subjects { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Point_Type> Point_Types { get; set; }
        public DbSet<Learning_Summary> Learning_Summaries { get; set; }
        public DbSet<Exam_Room> Exam_Rooms { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Exam_Room_Package> Exam_Room_Packages { get; set; }
        public DbSet<Exam_HisTory> Exam_HisTories { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<Summary> Summaries { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Test_Question> Test_Questions { get; set; }
        public DbSet<Answers> Answerses { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Teacher_Class> Teacher_Classes { get; set; }
        public DbSet<Exam_Room_Teacher> Exam_Room_Teachers { get; set; }
    }
}
