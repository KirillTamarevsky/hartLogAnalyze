using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hartLogAnalyze.vm
{
    public class hartLogInputVM : baseVM
    {
        private string? hartLog;
        public string? HartLog
        {
            get => hartLog; set
            {
                if (value != hartLog)
                {
                    hartLog = value;
                    RaisePropertyChanged();
                }
            }
        }
        public hartLogInputVM()
        {
                
        }
    }
}
