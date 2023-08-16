using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hartLogAnalyze.vm
{
    public class hartCommand : baseVM
    {
        public int Number { get; }
        public byte[] AnswerData { get; }

        public hartCommand(int num, byte[] answData)
        {
            Number = num;
            AnswerData = answData;
        }
    }
}
