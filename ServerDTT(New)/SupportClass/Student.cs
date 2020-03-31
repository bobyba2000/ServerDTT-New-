using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDTT_New_.SupportClass
{
    public class Student
    {
        public string Name;
        public int Point;
        public Student()
        {
            Name = "";
            Point = 0;
        }

        public Student(string name, int point)
        {
            Name = name;
            Point = point;
        }

        int AddPoint(int point)
        {
            Point += point;
            return Point;
        }
    }
}
