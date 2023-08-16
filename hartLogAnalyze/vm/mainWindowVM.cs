using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hartLogAnalyze.vm
{
    public class mainWindowVM : baseVM
    {
        private baseVM activeVM;
        public baseVM ActiveVM
        {
            get => activeVM; 
            set
            {
                if (value != activeVM)
                {
                    activeVM = value;
                    RaisePropertyChanged();
                }
            }
        }
        public mainWindowVM()
        {
                activeVM = new hartLogInputVM(this);
        }

        public void ShowVM(baseVM vm)
        {
            ActiveVM = vm;
        }
    }
}
