using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDTT_New_.DTO
{
    public class DecodeQuestion
    {
        public int RowNo { get; set; }
        public int ColNo { get; set; }
        public int QuestionTypeID { get; set; }
        public String Detail { get; set; }
        public String Answer { get; set; }
        public string QuestionVideoName;
        public string QuestionImageName;


        public DecodeQuestion() { }

        public DecodeQuestion(int rowNo, int colNo, int QuestionTypeID, String detail, String answer)
        {
            this.RowNo = rowNo;
            this.ColNo = colNo;
            this.QuestionTypeID = QuestionTypeID;
            this.Detail = detail;
            this.Answer = answer;
        }

        public DecodeQuestion(DataRow row)
        {
            this.RowNo = (int)row["Row"] - 1;
            this.ColNo = (int)row["Col"] - 1;
            this.QuestionTypeID = (int)row["QuestionTypeID"];
            this.Detail = row["Detail"].ToString();
            this.Answer = row["Answer"].ToString();
            QuestionImageName = row["QuestionImageName"].ToString();
            QuestionVideoName = row["QuestionVideoName"].ToString();
        }
    }
}
