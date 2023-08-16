

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace hartLogAnalyze.vm
{
    public class AnalyzeHARTLogVM : baseVM
    {
        private mainWindowVM mainWindowVM;
        private List<hartCommand> HartCommands{ get; }
        public AnalyzeHARTLogVM(mainWindowVM  mainWindow, string hartlog)
        {
            mainWindowVM = mainWindow;
            HartCommands = new List<hartCommand>();

            Regex regex = new Regex(@"cmd[\s:]*<(\d*)>[\s]*[\w\s\d\n:\-<>]*data[:\s]*<([\d\w\s]*)>");
            MatchCollection matches = regex.Matches(hartlog);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        if ( int.TryParse(match.Groups[1].Value, out int _))
                        {

                        }
                    }
                }
            }
        }

    }
}