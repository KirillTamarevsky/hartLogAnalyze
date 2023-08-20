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
        public int Number { get; }
        public byte[] QueryData { get; }
        public byte[] AnswerData { get; }

        public ObservableCollection<object> QueryDataRepresentations { get; set; }
        public ObservableCollection<object> AnswerDataRepresentations { get; set; }

        public hartCommandVM(int num, byte[] queryData, byte[] answData)
        {
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
