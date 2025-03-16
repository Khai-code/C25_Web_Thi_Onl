using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data_Base.Migrations
{
    public partial class dataBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade_Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Grade_Code = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Role_Code = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Room_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Room_Code = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Subject_Code = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Summaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start_Time = table.Column<int>(type: "int", maxLength: 14, nullable: false),
                    End_Time = table.Column<int>(type: "int", maxLength: 14, nullable: false),
                    Summary_Code = table.Column<int>(type: "int", maxLength: 14, nullable: false),
                    Summary_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Summaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade_Id = table.Column<int>(type: "int", nullable: false),
                    Teacher_Id = table.Column<int>(type: "int", nullable: false),
                    Class_Code = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Class_Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Max_Student = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    GradesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_Grades_GradesID",
                        column: x => x.GradesID,
                        principalTable: "Grades",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", maxLength: 8, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Full_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    User_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    User_Pass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Phone_Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Data_Of_Birth = table.Column<int>(type: "int", maxLength: 14, nullable: false),
                    Create_Time = table.Column<int>(type: "int", maxLength: 14, nullable: false),
                    Last_Mordification_Time = table.Column<int>(type: "int", maxLength: 14, nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Role_Id = table.Column<int>(type: "int", nullable: false),
                    RolesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject_Id = table.Column<int>(type: "int", nullable: false),
                    Exam_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Exam_Code = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    SubjectsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exams_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subject_Grades",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade_Id = table.Column<int>(type: "int", nullable: false),
                    Subject_Id = table.Column<int>(type: "int", nullable: false),
                    SubjectsId = table.Column<int>(type: "int", nullable: false),
                    GradesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject_Grades", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Subject_Grades_Grades_GradesID",
                        column: x => x.GradesID,
                        principalTable: "Grades",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subject_Grades_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Point_Types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Summary_Id = table.Column<int>(type: "int", nullable: false),
                    Point_Type_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SummariesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Point_Types", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Point_Types_Summaries_SummariesId",
                        column: x => x.SummariesId,
                        principalTable: "Summaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Student_Code = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Teacher_Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exam_Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start_Time = table.Column<int>(type: "int", maxLength: 14, nullable: false),
                    End_Time = table.Column<int>(type: "int", maxLength: 14, nullable: false),
                    Room_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Exam_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomsID = table.Column<int>(type: "int", nullable: false),
                    ExamsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_Rooms_Exams_ExamsId",
                        column: x => x.ExamsId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exam_Rooms_Rooms_RoomsID",
                        column: x => x.RoomsID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Class_Id = table.Column<int>(type: "int", nullable: false),
                    Subject_Id = table.Column<int>(type: "int", nullable: false),
                    Point_Type_Id = table.Column<int>(type: "int", nullable: false),
                    Package_Code = table.Column<int>(type: "int", maxLength: 8, nullable: false),
                    Package_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Create_Time = table.Column<int>(type: "int", maxLength: 14, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubjectsId = table.Column<int>(type: "int", nullable: false),
                    ClassesId = table.Column<int>(type: "int", nullable: false),
                    Point_TypesId = table.Column<int>(type: "int", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Packages_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Packages_Point_Types_Point_TypesId",
                        column: x => x.Point_TypesId,
                        principalTable: "Point_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Packages_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Point_Type_Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject_Id = table.Column<int>(type: "int", nullable: false),
                    Point_Type_Id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Point_TypesId = table.Column<int>(type: "int", nullable: false),
                    SubjectsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Point_Type_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Point_Type_Subjects_Point_Types_Point_TypesId",
                        column: x => x.Point_TypesId,
                        principalTable: "Point_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Point_Type_Subjects_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Learning_Summaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Attendance = table.Column<double>(type: "float", nullable: false),
                    Point_15 = table.Column<double>(type: "float", nullable: false),
                    Point_45 = table.Column<double>(type: "float", nullable: false),
                    Point_Midterm = table.Column<double>(type: "float", nullable: false),
                    Point_Final = table.Column<double>(type: "float", nullable: false),
                    Point_Summary = table.Column<double>(type: "float", nullable: false),
                    Summary_ID = table.Column<int>(type: "int", nullable: false),
                    Student_Id = table.Column<int>(type: "int", nullable: false),
                    Subject_Id = table.Column<int>(type: "int", nullable: false),
                    SubjectsId = table.Column<int>(type: "int", nullable: false),
                    StudentsId = table.Column<int>(type: "int", nullable: false),
                    SummariesId = table.Column<int>(type: "int", nullable: false),
                    Point_TypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Learning_Summaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Learning_Summaries_Point_Types_Point_TypeId",
                        column: x => x.Point_TypeId,
                        principalTable: "Point_Types",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Learning_Summaries_Students_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Learning_Summaries_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Learning_Summaries_Summaries_SummariesId",
                        column: x => x.SummariesId,
                        principalTable: "Summaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Student_Id = table.Column<int>(type: "int", nullable: false),
                    Subject_Id = table.Column<int>(type: "int", nullable: false),
                    Point_Type_Id = table.Column<int>(type: "int", nullable: false),
                    Point = table.Column<double>(type: "float", nullable: false),
                    SubjectsId = table.Column<int>(type: "int", nullable: false),
                    StudentsId = table.Column<int>(type: "int", nullable: false),
                    Point_TypesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scores_Point_Types_Point_TypesId",
                        column: x => x.Point_TypesId,
                        principalTable: "Point_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scores_Students_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scores_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Student_Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Student_Id = table.Column<int>(type: "int", nullable: false),
                    Teacher_Id = table.Column<int>(type: "int", nullable: false),
                    TeachersId = table.Column<int>(type: "int", nullable: false),
                    StudentsId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Student_Classes_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_Classes_Students_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Student_Classes_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Teacher_Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Teacher_Id = table.Column<int>(type: "int", nullable: false),
                    Class_Id = table.Column<int>(type: "int", nullable: false),
                    ClassesId = table.Column<int>(type: "int", nullable: false),
                    TeachersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teacher_Classes_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teacher_Classes_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teacher_Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Teacher_id = table.Column<int>(type: "int", nullable: false),
                    Subject_Id = table.Column<int>(type: "int", nullable: false),
                    TeachersId = table.Column<int>(type: "int", nullable: false),
                    SubjectsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teacher_Subjects_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teacher_Subjects_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exam_Room_Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Exam_Room = table.Column<int>(type: "int", nullable: false),
                    Teacher_Id = table.Column<int>(type: "int", nullable: false),
                    TeachersId = table.Column<int>(type: "int", nullable: false),
                    Exam_RoomsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_Room_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Teachers_Exam_Rooms_Exam_RoomsId",
                        column: x => x.Exam_RoomsId,
                        principalTable: "Exam_Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Teachers_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exam_Room_Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Package_Id = table.Column<int>(type: "int", nullable: false),
                    Exam_Room_Id = table.Column<int>(type: "int", nullable: false),
                    PackagesId = table.Column<int>(type: "int", nullable: false),
                    Exam_RoomsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_Room_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Packages_Exam_Rooms_Exam_RoomsId",
                        column: x => x.Exam_RoomsId,
                        principalTable: "Exam_Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Packages_Packages_PackagesId",
                        column: x => x.PackagesId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Package_Id = table.Column<int>(type: "int", nullable: false),
                    Question_Name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Question_Code = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Test_Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Package_Id = table.Column<int>(type: "int", nullable: false),
                    PackagesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_Packages_PackagesId",
                        column: x => x.PackagesId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exam_Room_Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Student_Id = table.Column<int>(type: "int", nullable: false),
                    Exam_Room_Package_Id = table.Column<int>(type: "int", nullable: false),
                    Check_Time = table.Column<int>(type: "int", maxLength: 14, nullable: false),
                    Exam_Room_PackagesId = table.Column<int>(type: "int", nullable: false),
                    StudentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_Room_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Students_Exam_Room_Packages_Exam_Room_PackagesId",
                        column: x => x.Exam_Room_PackagesId,
                        principalTable: "Exam_Room_Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Students_Students_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answerses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question_Id = table.Column<int>(type: "int", nullable: false),
                    Answers_Name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Answers_Code = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Right_Answer = table.Column<int>(type: "int", nullable: false),
                    QuestionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answerses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answerses_Questions_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Test_Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", maxLength: 8, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question_Id = table.Column<int>(type: "int", nullable: false),
                    Test_Id = table.Column<int>(type: "int", nullable: false),
                    TestsId = table.Column<int>(type: "int", nullable: false),
                    QuestionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Test_Questions_Questions_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Test_Questions_Tests_TestsId",
                        column: x => x.TestsId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exam_HisTories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Exam_Room_Student_Id = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    Create_Time = table.Column<int>(type: "int", maxLength: 14, nullable: false),
                    Exam_Room_StudentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_HisTories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Exam_HisTories_Exam_Room_Students_Exam_Room_StudentsId",
                        column: x => x.Exam_Room_StudentsId,
                        principalTable: "Exam_Room_Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exam_Room_Student_Answer_HisTories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Answer_Id = table.Column<int>(type: "int", nullable: false),
                    Exam_Room_Student_Id = table.Column<int>(type: "int", nullable: false),
                    AnswersId = table.Column<int>(type: "int", nullable: false),
                    Exam_Room_StudentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_Room_Student_Answer_HisTories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Student_Answer_HisTories_Answerses_AnswersId",
                        column: x => x.AnswersId,
                        principalTable: "Answerses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Student_Answer_HisTories_Exam_Room_Students_Exam_Room_StudentsId",
                        column: x => x.Exam_Room_StudentsId,
                        principalTable: "Exam_Room_Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answerses_QuestionsId",
                table: "Answerses",
                column: "QuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_GradesID",
                table: "Classes",
                column: "GradesID");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_HisTories_Exam_Room_StudentsId",
                table: "Exam_HisTories",
                column: "Exam_Room_StudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Packages_Exam_RoomsId",
                table: "Exam_Room_Packages",
                column: "Exam_RoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Packages_PackagesId",
                table: "Exam_Room_Packages",
                column: "PackagesId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Student_Answer_HisTories_AnswersId",
                table: "Exam_Room_Student_Answer_HisTories",
                column: "AnswersId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Student_Answer_HisTories_Exam_Room_StudentsId",
                table: "Exam_Room_Student_Answer_HisTories",
                column: "Exam_Room_StudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Students_Exam_Room_PackagesId",
                table: "Exam_Room_Students",
                column: "Exam_Room_PackagesId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Students_StudentsId",
                table: "Exam_Room_Students",
                column: "StudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Teachers_Exam_RoomsId",
                table: "Exam_Room_Teachers",
                column: "Exam_RoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Teachers_TeachersId",
                table: "Exam_Room_Teachers",
                column: "TeachersId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Rooms_ExamsId",
                table: "Exam_Rooms",
                column: "ExamsId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Rooms_RoomsID",
                table: "Exam_Rooms",
                column: "RoomsID");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_SubjectsId",
                table: "Exams",
                column: "SubjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Summaries_Point_TypeId",
                table: "Learning_Summaries",
                column: "Point_TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Summaries_StudentsId",
                table: "Learning_Summaries",
                column: "StudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Summaries_SubjectsId",
                table: "Learning_Summaries",
                column: "SubjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Summaries_SummariesId",
                table: "Learning_Summaries",
                column: "SummariesId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ClassesId",
                table: "Packages",
                column: "ClassesId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_PackageId",
                table: "Packages",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Point_TypesId",
                table: "Packages",
                column: "Point_TypesId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_SubjectsId",
                table: "Packages",
                column: "SubjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_Point_Type_Subjects_Point_TypesId",
                table: "Point_Type_Subjects",
                column: "Point_TypesId");

            migrationBuilder.CreateIndex(
                name: "IX_Point_Type_Subjects_SubjectsId",
                table: "Point_Type_Subjects",
                column: "SubjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_Point_Types_SummariesId",
                table: "Point_Types",
                column: "SummariesId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_PackageId",
                table: "Questions",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_Point_TypesId",
                table: "Scores",
                column: "Point_TypesId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_StudentsId",
                table: "Scores",
                column: "StudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_SubjectsId",
                table: "Scores",
                column: "SubjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Classes_ClassId",
                table: "Student_Classes",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Classes_StudentsId",
                table: "Student_Classes",
                column: "StudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Classes_TeachersId",
                table: "Student_Classes",
                column: "TeachersId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UsersId",
                table: "Students",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_Grades_GradesID",
                table: "Subject_Grades",
                column: "GradesID");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_Grades_SubjectsId",
                table: "Subject_Grades",
                column: "SubjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_Classes_ClassesId",
                table: "Teacher_Classes",
                column: "ClassesId");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_Classes_TeachersId",
                table: "Teacher_Classes",
                column: "TeachersId");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_Subjects_SubjectsId",
                table: "Teacher_Subjects",
                column: "SubjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_Subjects_TeachersId",
                table: "Teacher_Subjects",
                column: "TeachersId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_UserId",
                table: "Teachers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Test_Questions_QuestionsId",
                table: "Test_Questions",
                column: "QuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Test_Questions_TestsId",
                table: "Test_Questions",
                column: "TestsId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_PackagesId",
                table: "Tests",
                column: "PackagesId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RolesId",
                table: "Users",
                column: "RolesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exam_HisTories");

            migrationBuilder.DropTable(
                name: "Exam_Room_Student_Answer_HisTories");

            migrationBuilder.DropTable(
                name: "Exam_Room_Teachers");

            migrationBuilder.DropTable(
                name: "Learning_Summaries");

            migrationBuilder.DropTable(
                name: "Point_Type_Subjects");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "Student_Classes");

            migrationBuilder.DropTable(
                name: "Subject_Grades");

            migrationBuilder.DropTable(
                name: "Teacher_Classes");

            migrationBuilder.DropTable(
                name: "Teacher_Subjects");

            migrationBuilder.DropTable(
                name: "Test_Questions");

            migrationBuilder.DropTable(
                name: "Answerses");

            migrationBuilder.DropTable(
                name: "Exam_Room_Students");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Exam_Room_Packages");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Exam_Rooms");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Point_Types");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Summaries");
        }
    }
}
