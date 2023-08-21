

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace hartLogAnalyze.vm
{
    public class AnalyzeHARTLogVM : baseVM
    {
        private List<hartCommandVM> hartCommands;
        public List<hartCommandVM> HartCommands { get => hartCommands.Where(c=> c.AnswerDataRepresentations.Any() | c.QueryDataRepresentations.Any()).ToList(); }
        
        private SearchPattern selectedSearchPattern;
        public SearchPattern SelectedSearchPattern
        {
            get => selectedSearchPattern;
            set
            {
                if (selectedSearchPattern != value)
                {
                    selectedSearchPattern = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(PatternToSearch));
                    ApplySearchPattern.Execute(null);
                }
            }
        }
        public ObservableCollection<SearchPattern> SearchPatterns { get; }
        public string PatternToSearch { get => SelectedSearchPattern.PatternToSearch;
        set
            {
                if (SelectedSearchPattern.PatternToSearch != value)
                {
                    SelectedSearchPattern.PatternToSearch = value;
                    RaisePropertyChanged();
                    ApplySearchPattern.Execute(null);
                }
            }
        }
        public ICommand ApplySearchPattern { get; }
        public ICommand PasteFromClipboard { get; }

        public AnalyzeHARTLogVM()
        {
            SearchPatterns = new ObservableCollection<SearchPattern>() 
            {
                new RawHexStringSearchPattern(),
                new ASCIIStringSearchPattern(), 
                new PackedASCIIStringSearchPattern(),
                new IEEE754SearchPattern()

            };

            selectedSearchPattern = SearchPatterns.First();

            hartCommands = new List<hartCommandVM>();

            ApplySearchPattern = new RelayCommand(
                o =>
                {
                    hartCommands.ForEach(c =>
                    {
                        c.AcceptSearchPattern ( SelectedSearchPattern );
                    });
                    RaisePropertyChanged(nameof(HartCommands));
                });

            PasteFromClipboard = new RelayCommand(
                o =>
                {
                    var hartLogString = Clipboard.GetText();
                    if (!string.IsNullOrEmpty(hartLogString)) ParseHartLogString(hartLogString);
                    ApplySearchPattern.Execute(null);
                });

            PasteFromClipboard.Execute(null);

        }

        private void ParseHartLogString(string hartlog)
        {
            hartCommands.Clear();

            Regex regex = new Regex(@"(\d\d\d\d\-\d\d\-\d\d\s*\d\d:\d\d:\d\d):\s*.*SEQID\s*=\s*(\d*)[\w\s\d\n:\-<>]*CMD[\s:]*<(\d*)>[\s]*[\w\s\d\n:\-<>]*DATA[:\s]*<([\d\w\s]*)>[\w\s\d\n:\-<>]*DATA[:\s]*<([\d\w\s]*)>");
            MatchCollection matches = regex.Matches(hartlog);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        if (int.TryParse(match.Groups[3].Value, out int cmdnumber))
                        {
                            var hartcommand = new hartCommandVM(DateTime.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), cmdnumber, StringToByteArray(match.Groups[4].Value), StringToByteArray(match.Groups[5].Value));
                            hartCommands.Add(hartcommand);
                        }
                    }
                }
            }

            RaisePropertyChanged(nameof(HartCommands));
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 3 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}