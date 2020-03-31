using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerDTT_New_.SupportClass;
using ServerDTT_New_.DTO;
using ServerDTT_New_.DAO;

namespace ServerDTT_New_.DAO
{
    public class BUDecodeQuestionDAO
    {
        private static BUDecodeQuestionDAO instance;
        public static BUDecodeQuestionDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new BUDecodeQuestionDAO();
                return instance;
            }
        }
        public BUDecodeQuestionDAO() { }

        public List<DecodeQuestion> getQuestions()
        {
            string query = string.Format(@"SELECT * FROM BUDecodeQuestion");
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            List<DecodeQuestion> result = new List<DecodeQuestion>();
            foreach (DataRow row in data.Rows)
            {
                result.Add(new DecodeQuestion(row));
            }
            return result;
        }

        public DecodeQuestion getQuestion(int row, int col)
        {
            string query = string.Format(@"
                           SELECT * FROM BUDecodeQuestion
                           WHERE Row = {0} AND Col = {1}", row + 1, col + 1);
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            DecodeQuestion result = new DecodeQuestion(data.Rows[0]);
            return result;
        }
    }
}
