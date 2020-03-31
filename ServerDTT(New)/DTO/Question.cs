using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ServerDTT_New_.DTO
{
    class Question
    {
        public string QuestionID { get; set; }
        public string Detail { get; set; }
        public string Answer { get; set; }
        public Int32 QuestionTypeID { get; set; }
        public string Note { get; set; }
        public string QuestionImageName;
        public string QuestionVideoName;
        public string AnswerImageName;
        public string AnswerVideoName;


        public Question() { }

        public Question(string questionID, string detail, string answer, Int32 questionTypeID, string note, string questionImageName, string questionVideoName, string answerImageName,string answerVideoName)
        {
            this.QuestionID = questionID;
            this.Detail = detail;
            this.Answer = answer;
            this.QuestionTypeID = questionTypeID;
            this.Note = note;
            this.QuestionImageName = questionImageName;
            this.QuestionVideoName = questionVideoName;
            this.AnswerImageName = answerImageName;
            this.AnswerVideoName = answerVideoName;
        }

        public Question(DataRow row)
        {
            this.QuestionID = row["QuestionID"].ToString();
            this.Detail = row["Detail"].ToString();
            this.Answer = row["Answer"].ToString();
            this.QuestionTypeID = (Int32)row["QuestionTypeID"];
            this.Note = row["Note"].ToString();
            this.AnswerImageName = row["AnswerImageName"].ToString();
            this.AnswerVideoName = row["AnswerVideoName"].ToString();
            this.QuestionImageName = row["QuestionImageName"].ToString();
            this.QuestionVideoName = row["QuestionVideoName"].ToString();
        }
    }
}
