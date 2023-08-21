using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace hartLogAnalyze.vm
{
    public class hartCommandVM : baseVM
    {
        public DateTime DateTime { get; }
        public int SEQID { get; }
        public int Number { get; }
        public byte[] QueryData { get; }
        public byte[] AnswerData { get; }

        public ObservableCollection<object> QueryDataRepresentations { get; set; }
        public ObservableCollection<object> AnswerDataRepresentations { get; set; }

        public hartCommandVM(DateTime _datetime, int seqid, int num, byte[] queryData, byte[] answData)
        {
            DateTime = _datetime;
            SEQID = seqid;
            Number = num;
            QueryData = queryData;
            AnswerData = answData;
            QueryDataRepresentations = new ObservableCollection<object>();
            AnswerDataRepresentations = new ObservableCollection<object>();
        }

        internal void AcceptSearchPattern(SearchPattern searchPattern)
        {
            searchPattern.Visit(this);
        }
    }

}
