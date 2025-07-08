using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.DTO_Import_Excel
{
    public class QuestionImportEssay
    {
        public string Question_Name { get; set; }
        public int Question_Type_Id { get; set; }
        public int Question_Level_Id { get; set; }
        public int Package_Id { get; set; }
        public double Maximum_Score { get; set; }
        public byte[] pictures { get; set; }
    }
}
