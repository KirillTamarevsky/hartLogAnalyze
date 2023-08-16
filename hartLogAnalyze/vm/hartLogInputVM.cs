using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace hartLogAnalyze.vm
{
    public class hartLogInputVM : baseVM
    {
        private mainWindowVM mainWindowVM;
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
        public ICommand OKCommand { get; } 
        public hartLogInputVM(mainWindowVM mainWVM)
        {
            mainWindowVM = mainWVM;
            OKCommand = new RelayCommand(o =>
            {
                mainWindowVM.ShowVM(new AnalyzeHARTLogVM(mainWindowVM, hartLog));
            },
            o => !string.IsNullOrEmpty(hartLog) );
        }
    }
}
