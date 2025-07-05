using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data_Base.Migrations
{
    public partial class data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade_Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.ID);
                });

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT Grades ON;

                IF NOT EXISTS (SELECT 1 FROM Grades WHERE Id = 1)
                    INSERT INTO Grades (Id, Grade_Name) VALUES (1, N'Khối 6');
        
                IF NOT EXISTS (SELECT 1 FROM Grades WHERE Id = 2)
                    INSERT INTO Grades (Id, Grade_Name) VALUES (2, N'Khối 7');

                IF NOT EXISTS (SELECT 1 FROM Grades WHERE Id = 3)
                    INSERT INTO Grades (Id, Grade_Name) VALUES (3, N'Khối 8');

                IF NOT EXISTS (SELECT 1 FROM Grades WHERE Id = 4)
                    INSERT INTO Grades (Id, Grade_Name) VALUES (4, N'Khối 9');

                SET IDENTITY_INSERT Grades OFF;
            ");

            migrationBuilder.CreateTable(
                name: "Package_Types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Package_Type_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package_Types", x => x.Id);
                });

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT Package_Types ON;
        
                IF NOT EXISTS (SELECT 1 FROM Package_Types WHERE Id = 1)
                    INSERT INTO Package_Types (Id, Package_Type_Name) VALUES (1, N'Tự luân');
        
                IF NOT EXISTS (SELECT 1 FROM Package_Types WHERE Id = 2)
                    INSERT INTO Package_Types (Id, Package_Type_Name) VALUES (2, N'Chắc nghiệm');
        
                SET IDENTITY_INSERT Package_Types OFF;
            ");

            migrationBuilder.CreateTable(
                name: "Point_Types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Point_Type_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Point_Types", x => x.Id);
                });

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT Point_Types ON;
        
                IF NOT EXISTS (SELECT 1 FROM Point_Types WHERE Id = 1)
                    INSERT INTO Point_Types (Id, Point_Type_Name) VALUES (1, N'Attendance');
        
                IF NOT EXISTS (SELECT 1 FROM Point_Types WHERE Id = 2)
                    INSERT INTO Point_Types (Id, Point_Type_Name) VALUES (2, N'Point_15');
        
                IF NOT EXISTS (SELECT 1 FROM Point_Types WHERE Id = 3)
                    INSERT INTO Point_Types (Id, Point_Type_Name) VALUES (3, N'Point_45');
        
                IF NOT EXISTS (SELECT 1 FROM Point_Types WHERE Id = 4)
                    INSERT INTO Point_Types (Id, Point_Type_Name) VALUES (4, N'Point_Midterm');
    
                IF NOT EXISTS (SELECT 1 FROM Point_Types WHERE Id = 5)
                    INSERT INTO Point_Types (Id, Point_Type_Name) VALUES (5, N'Point_Final');                                

                SET IDENTITY_INSERT Point_Types OFF;
            ");


            migrationBuilder.CreateTable(
                name: "Question_Levels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question_Level_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question_Levels", x => x.Id);
                });

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT Question_Levels ON;
        
                IF NOT EXISTS (SELECT 1 FROM Question_Levels WHERE Id = 1)
                    INSERT INTO Question_Levels (Id, Question_Level_Name) VALUES (1, N'Dễ');
        
                IF NOT EXISTS (SELECT 1 FROM Question_Levels WHERE Id = 2)
                    INSERT INTO Question_Levels (Id, Question_Level_Name) VALUES (2, N'Trung bình');
        
                IF NOT EXISTS (SELECT 1 FROM Question_Levels WHERE Id = 3)
                    INSERT INTO Question_Levels (Id, Question_Level_Name) VALUES (3, N'Khó');
        
                SET IDENTITY_INSERT Question_Levels OFF;
            ");


            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT Roles ON;

                IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 1)
                    INSERT INTO Roles (Id, Role_Name, Status) VALUES (1, N'Student', 0);

                IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 2)
                    INSERT INTO Roles (Id, Role_Name, Status) VALUES (2, N'Teacher', 0);

                SET IDENTITY_INSERT Roles OFF;
            ");


            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Room_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.ID);
                });

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT Rooms ON;

                IF NOT EXISTS (SELECT 1 FROM Rooms WHERE Id = 1)
                    INSERT INTO Rooms (Id, Room_Name) VALUES (1, N'Phòng 1');

                IF NOT EXISTS (SELECT 1 FROM Rooms WHERE Id = 2)
                    INSERT INTO Rooms (Id, Room_Name) VALUES (2, N'Phòng 2');

                IF NOT EXISTS (SELECT 1 FROM Rooms WHERE Id = 3)
                    INSERT INTO Rooms (Id, Room_Name) VALUES (3, N'Phòng 3');

                IF NOT EXISTS (SELECT 1 FROM Rooms WHERE Id = 4)
                    INSERT INTO Rooms (Id, Room_Name) VALUES (4, N'Phòng 4');

                SET IDENTITY_INSERT Rooms OFF;
            ");


            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT Subjects ON;

                IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = 1)
                    INSERT INTO Subjects (Id, Subject_Name) VALUES (1, N'Toán');

                IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = 2)
                    INSERT INTO Subjects (Id, Subject_Name) VALUES (2, N'Lý');

                IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = 3)
                    INSERT INTO Subjects (Id, Subject_Name) VALUES (3, N'Hóa');

                IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = 4)
                    INSERT INTO Subjects (Id, Subject_Name) VALUES (4, N'Sinh');

                IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = 5)
                    INSERT INTO Subjects (Id, Subject_Name) VALUES (5, N'Sử');

                IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = 6)
                    INSERT INTO Subjects (Id, Subject_Name) VALUES (6, N'Địa');

                IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = 7)
                    INSERT INTO Subjects (Id, Subject_Name) VALUES (7, N'GDCD');

                IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = 8)
                    INSERT INTO Subjects (Id, Subject_Name) VALUES (8, N'Văn');

                IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = 9)
                    INSERT INTO Subjects (Id, Subject_Name) VALUES (9, N'Anh');

                SET IDENTITY_INSERT Subjects OFF;
            ");


            migrationBuilder.CreateTable(
                name: "Summaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start_Time = table.Column<long>(type: "bigint", nullable: false),
                    End_Time = table.Column<long>(type: "bigint", nullable: false),
                    Summary_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Summaries", x => x.Id);
                });

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT Summaries ON;

                IF NOT EXISTS (SELECT 1 FROM Summaries WHERE Id = 1)
                    INSERT INTO Summaries (Id, Start_Time, End_Time, Summary_Name) VALUES (1, 20250601000000, 20250801000000, N'Kỳ 1');
        
                IF NOT EXISTS (SELECT 1 FROM Summaries WHERE Id = 2)
                    INSERT INTO Summaries (Id, Start_Time, End_Time, Summary_Name) VALUES (2, 20250802000000, 20251002000000, N'Kỳ 2');

                SET IDENTITY_INSERT Summaries OFF;
            ");


            migrationBuilder.CreateTable(
                name: "Question_Types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question_Type_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Package_Type_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question_Types", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_Types_Package_Types_Package_Type_Id",
                        column: x => x.Package_Type_Id,
                        principalTable: "Package_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT Question_Types ON;
        
                IF NOT EXISTS (SELECT 1 FROM Question_Types WHERE Id = 1)
                    INSERT INTO Question_Types (Id, Question_Type_Name, Package_Type_Id) VALUES (1, N'TrueFalse', 2);
        
                IF NOT EXISTS (SELECT 1 FROM Question_Types WHERE Id = 2)
                    INSERT INTO Question_Types (Id, Question_Type_Name, Package_Type_Id) VALUES (2, N'Một đáp án', 2);
        
                IF NOT EXISTS (SELECT 1 FROM Question_Types WHERE Id = 3)
                    INSERT INTO Question_Types (Id, Question_Type_Name, Package_Type_Id) VALUES (3, N'Nhiều đáp án', 2);

                IF NOT EXISTS (SELECT 1 FROM Question_Types WHERE Id = 4)
                        INSERT INTO Question_Types (Id, Question_Type_Name, Package_Type_Id) VALUES (4, N'Câu hỏi ngắn', 1);

                IF NOT EXISTS (SELECT 1 FROM Question_Types WHERE Id = 5)
                        INSERT INTO Question_Types (Id, Question_Type_Name, Package_Type_Id) VALUES (5, N'Câu hỏi dài', 1);
        
                SET IDENTITY_INSERT Question_Types OFF;
            ");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Full_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    User_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    User_Pass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone_Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Data_Of_Birth = table.Column<long>(type: "bigint", nullable: false),
                    Create_Time = table.Column<long>(type: "bigint", nullable: false),
                    Last_Mordification_Time = table.Column<long>(type: "bigint", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Role_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_Role_Id",
                        column: x => x.Role_Id,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Exam_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subject_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exams_Subjects_Subject_Id",
                        column: x => x.Subject_Id,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Point_Type_Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Point_Type_Id = table.Column<int>(type: "int", nullable: false),
                    Subject_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Point_Type_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Point_Type_Subjects_Point_Types_Point_Type_Id",
                        column: x => x.Point_Type_Id,
                        principalTable: "Point_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Point_Type_Subjects_Subjects_Subject_Id",
                        column: x => x.Subject_Id,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT Point_Type_Subjects ON;
            
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 1)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (1, 3, 1, 1);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 2)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (2, 3, 2, 1);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 3)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (3, 2, 3, 1);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 4)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (4, 1, 4, 1);
    
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 5)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (5, 1, 5, 1);                

                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 6)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (6, 3, 1, 2);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 7)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (7, 3, 2, 2);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 8)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (8, 2, 3, 2);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 9)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (9, 1, 4, 2);
    
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 10)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (10, 1, 5, 2);                

                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 11)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (11, 3, 1, 3);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 12)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (12, 3, 2, 3);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 13)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (13, 2, 3, 3);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 14)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (14, 1, 4, 3);
    
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 15)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (15, 1, 5, 3);                

                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 16)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (16, 3, 1, 4);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 17)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (17, 3, 2, 4);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 18)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (18, 2, 3, 4);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 19)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (19, 1, 4, 4);
    
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 20)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (20, 1, 5, 4);                

                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 21)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (21, 3, 1, 5);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 22)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (22, 3, 2, 5);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 23)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (23, 2, 3, 5);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 24)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (24, 1, 4, 5);
    
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 25)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (25, 1, 5, 5);                

                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 26)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (26, 3, 1, 6);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 27)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (27, 3, 2, 6);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 28)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (28, 2, 3, 6);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 29)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (29, 1, 4, 6);
    
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 30)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (30, 1, 5, 6);                

                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 31)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (31, 3, 1, 7);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 32)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (32, 3, 2, 7);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 33)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (33, 2, 3, 7);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 34)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (34, 1, 4, 7);
    
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 35)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (35, 1, 5, 7);                

                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 36)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (36, 3, 1, 8);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 37)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (37, 3, 2, 8);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 38)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (38, 2, 3, 8);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 39)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (39, 1, 4, 8);
    
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 40)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (40, 1, 5, 8);                

                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 41)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (41, 3, 1, 9);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 42)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (42, 3, 2, 9);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 43)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (43, 2, 3, 9);
        
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 44)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (44, 1, 4, 9);
    
                IF NOT EXISTS (SELECT 1 FROM Point_Type_Subjects WHERE Id = 45)
                    INSERT INTO Point_Type_Subjects (Id, Quantity, Point_Type_Id, Subject_Id) VALUES (45, 1, 5, 9);           

                SET IDENTITY_INSERT Point_Type_Subjects OFF;
            ");


            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Student_Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Teacher_Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Exam_Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start_Time = table.Column<long>(type: "bigint", nullable: false),
                    End_Time = table.Column<long>(type: "bigint", nullable: false),
                    Room_Id = table.Column<int>(type: "int", nullable: false),
                    Exam_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_Rooms_Exams_Exam_Id",
                        column: x => x.Exam_Id,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Exam_Rooms_Rooms_Room_Id",
                        column: x => x.Room_Id,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
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
                    Subject_Id = table.Column<int>(type: "int", nullable: false),
                    Student_Id = table.Column<int>(type: "int", nullable: false),
                    Summary_ID = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_Learning_Summaries_Students_Student_Id",
                        column: x => x.Student_Id,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Learning_Summaries_Subjects_Subject_Id",
                        column: x => x.Subject_Id,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Learning_Summaries_Summaries_Summary_ID",
                        column: x => x.Summary_ID,
                        principalTable: "Summaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
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
                    Summary_Id = table.Column<int>(type: "int", nullable: false),
                    Point = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scores_Point_Types_Point_Type_Id",
                        column: x => x.Point_Type_Id,
                        principalTable: "Point_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Scores_Students_Student_Id",
                        column: x => x.Student_Id,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Scores_Subjects_Subject_Id",
                        column: x => x.Subject_Id,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Scores_Summaries_Summary_Id",
                        column: x => x.Summary_Id,
                        principalTable: "Summaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Class_Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Class_Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Max_Student = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Grade_Id = table.Column<int>(type: "int", nullable: false),
                    Teacher_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_Grades_Grade_Id",
                        column: x => x.Grade_Id,
                        principalTable: "Grades",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Classes_Teachers_Teacher_Id",
                        column: x => x.Teacher_Id,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Teacher_Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Teacher_id = table.Column<int>(type: "int", nullable: false),
                    Subject_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teacher_Subjects_Subjects_Subject_Id",
                        column: x => x.Subject_Id,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Teacher_Subjects_Teachers_Teacher_id",
                        column: x => x.Teacher_id,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Exam_Room_Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Teacher_Id = table.Column<int>(type: "int", nullable: false),
                    Exam_Room_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_Room_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Teachers_Exam_Rooms_Exam_Room_Id",
                        column: x => x.Exam_Room_Id,
                        principalTable: "Exam_Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Teachers_Teachers_Teacher_Id",
                        column: x => x.Teacher_Id,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Package_Code = table.Column<int>(type: "int", nullable: false),
                    Package_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Create_Time = table.Column<long>(type: "bigint", nullable: false),
                    Number_Of_Questions = table.Column<int>(type: "int", nullable: false),
                    ExecutionTime = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Subject_Id = table.Column<int>(type: "int", nullable: false),
                    Class_Id = table.Column<int>(type: "int", nullable: false),
                    Point_Type_Id = table.Column<int>(type: "int", nullable: false),
                    Package_Type_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Classes_Class_Id",
                        column: x => x.Class_Id,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Packages_Package_Types_Package_Type_Id",
                        column: x => x.Package_Type_Id,
                        principalTable: "Package_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Packages_Point_Types_Point_Type_Id",
                        column: x => x.Point_Type_Id,
                        principalTable: "Point_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Packages_Subjects_Subject_Id",
                        column: x => x.Subject_Id,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Student_Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Student_Id = table.Column<int>(type: "int", nullable: false),
                    Class_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Student_Classes_Classes_Class_Id",
                        column: x => x.Class_Id,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Student_Classes_Students_Student_Id",
                        column: x => x.Student_Id,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Exam_Room_Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Package_Id = table.Column<int>(type: "int", nullable: false),
                    Exam_Room_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_Room_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Packages_Exam_Rooms_Exam_Room_Id",
                        column: x => x.Exam_Room_Id,
                        principalTable: "Exam_Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Packages_Packages_Package_Id",
                        column: x => x.Package_Id,
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
                    Question_Name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Maximum_Score = table.Column<long>(type: "bigint", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Question_Type_Id = table.Column<int>(type: "int", nullable: false),
                    Package_Id = table.Column<int>(type: "int", nullable: false),
                    Question_Level_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Packages_Package_Id",
                        column: x => x.Package_Id,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Questions_Question_Levels_Question_Level_Id",
                        column: x => x.Question_Level_Id,
                        principalTable: "Question_Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Questions_Question_Types_Question_Type_Id",
                        column: x => x.Question_Type_Id,
                        principalTable: "Question_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Test_Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Package_Id = table.Column<int>(type: "int", nullable: false),
                    Student_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_Packages_Package_Id",
                        column: x => x.Package_Id,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Tests_Students_Student_Id",
                        column: x => x.Student_Id,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Answerses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Answers_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Right_Answer = table.Column<int>(type: "int", nullable: true),
                    Points_Earned = table.Column<double>(type: "float", nullable: true),
                    Question_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answerses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answerses_Questions_Question_Id",
                        column: x => x.Question_Id,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Exam_Room_Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Check_Time = table.Column<long>(type: "bigint", nullable: false),
                    Exam_Room_Package_Id = table.Column<int>(type: "int", nullable: false),
                    Student_Id = table.Column<int>(type: "int", nullable: false),
                    Test_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_Room_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Students_Exam_Room_Packages_Exam_Room_Package_Id",
                        column: x => x.Exam_Room_Package_Id,
                        principalTable: "Exam_Room_Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Students_Students_Student_Id",
                        column: x => x.Student_Id,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Students_Tests_Test_Id",
                        column: x => x.Test_Id,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Test_Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question_Id = table.Column<int>(type: "int", nullable: false),
                    Test_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Test_Questions_Questions_Question_Id",
                        column: x => x.Question_Id,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Test_Questions_Tests_Test_Id",
                        column: x => x.Test_Id,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Exam_HisTories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Score = table.Column<double>(type: "float", nullable: false),
                    Create_Time = table.Column<long>(type: "bigint", nullable: false),
                    Actual_Execution_Time = table.Column<long>(type: "bigint", nullable: false),
                    Exam_Room_Student_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_HisTories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_HisTories_Exam_Room_Students_Exam_Room_Student_Id",
                        column: x => x.Exam_Room_Student_Id,
                        principalTable: "Exam_Room_Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Exam_Room_Student_Answer_HisTories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Answer_Id = table.Column<int>(type: "int", nullable: false),
                    Exam_Room_Student_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam_Room_Student_Answer_HisTories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Student_Answer_HisTories_Answerses_Answer_Id",
                        column: x => x.Answer_Id,
                        principalTable: "Answerses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exam_Room_Student_Answer_HisTories_Exam_Room_Students_Exam_Room_Student_Id",
                        column: x => x.Exam_Room_Student_Id,
                        principalTable: "Exam_Room_Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answerses_Question_Id",
                table: "Answerses",
                column: "Question_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_Grade_Id",
                table: "Classes",
                column: "Grade_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_Teacher_Id",
                table: "Classes",
                column: "Teacher_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_HisTories_Exam_Room_Student_Id",
                table: "Exam_HisTories",
                column: "Exam_Room_Student_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Packages_Exam_Room_Id",
                table: "Exam_Room_Packages",
                column: "Exam_Room_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Packages_Package_Id",
                table: "Exam_Room_Packages",
                column: "Package_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Student_Answer_HisTories_Answer_Id",
                table: "Exam_Room_Student_Answer_HisTories",
                column: "Answer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Student_Answer_HisTories_Exam_Room_Student_Id",
                table: "Exam_Room_Student_Answer_HisTories",
                column: "Exam_Room_Student_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Students_Exam_Room_Package_Id",
                table: "Exam_Room_Students",
                column: "Exam_Room_Package_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Students_Student_Id",
                table: "Exam_Room_Students",
                column: "Student_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Students_Test_Id",
                table: "Exam_Room_Students",
                column: "Test_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Teachers_Exam_Room_Id",
                table: "Exam_Room_Teachers",
                column: "Exam_Room_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Room_Teachers_Teacher_Id",
                table: "Exam_Room_Teachers",
                column: "Teacher_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Rooms_Exam_Id",
                table: "Exam_Rooms",
                column: "Exam_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Rooms_Room_Id",
                table: "Exam_Rooms",
                column: "Room_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_Subject_Id",
                table: "Exams",
                column: "Subject_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Summaries_Point_TypeId",
                table: "Learning_Summaries",
                column: "Point_TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Summaries_Student_Id",
                table: "Learning_Summaries",
                column: "Student_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Summaries_Subject_Id",
                table: "Learning_Summaries",
                column: "Subject_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Summaries_Summary_ID",
                table: "Learning_Summaries",
                column: "Summary_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Class_Id",
                table: "Packages",
                column: "Class_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Package_Type_Id",
                table: "Packages",
                column: "Package_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Point_Type_Id",
                table: "Packages",
                column: "Point_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Subject_Id",
                table: "Packages",
                column: "Subject_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Point_Type_Subjects_Point_Type_Id",
                table: "Point_Type_Subjects",
                column: "Point_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Point_Type_Subjects_Subject_Id",
                table: "Point_Type_Subjects",
                column: "Subject_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Question_Types_Package_Type_Id",
                table: "Question_Types",
                column: "Package_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_Package_Id",
                table: "Questions",
                column: "Package_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_Question_Level_Id",
                table: "Questions",
                column: "Question_Level_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_Question_Type_Id",
                table: "Questions",
                column: "Question_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_Point_Type_Id",
                table: "Scores",
                column: "Point_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_Student_Id",
                table: "Scores",
                column: "Student_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_Subject_Id",
                table: "Scores",
                column: "Subject_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_Summary_Id",
                table: "Scores",
                column: "Summary_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Classes_Class_Id",
                table: "Student_Classes",
                column: "Class_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Classes_Student_Id",
                table: "Student_Classes",
                column: "Student_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_User_Id",
                table: "Students",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_Subjects_Subject_Id",
                table: "Teacher_Subjects",
                column: "Subject_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_Subjects_Teacher_id",
                table: "Teacher_Subjects",
                column: "Teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_User_Id",
                table: "Teachers",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Test_Questions_Question_Id",
                table: "Test_Questions",
                column: "Question_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Test_Questions_Test_Id",
                table: "Test_Questions",
                column: "Test_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_Package_Id",
                table: "Tests",
                column: "Package_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_Student_Id",
                table: "Tests",
                column: "Student_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role_Id",
                table: "Users",
                column: "Role_Id");
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
                name: "Teacher_Subjects");

            migrationBuilder.DropTable(
                name: "Test_Questions");

            migrationBuilder.DropTable(
                name: "Answerses");

            migrationBuilder.DropTable(
                name: "Exam_Room_Students");

            migrationBuilder.DropTable(
                name: "Summaries");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Exam_Room_Packages");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Question_Levels");

            migrationBuilder.DropTable(
                name: "Question_Types");

            migrationBuilder.DropTable(
                name: "Exam_Rooms");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Package_Types");

            migrationBuilder.DropTable(
                name: "Point_Types");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
