

namespace hartLogAnalyze.vm
{
    public class AnalyzeHARTLogVM : baseVM
    {
        private mainWindowVM mainWindowVM;
        private string HARTLog { get; }
        public AnalyzeHARTLogVM(mainWindowVM  mainWindow, string hartlog)
        {
            mainWindowVM = mainWindow;
            HARTLog = hartlog;
        }

    }
}