using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hartLogAnalyze.vm
{
    public abstract class SearchPattern : baseVM
    {
        public abstract string PatternName { get; }

        private string pattern = string.Empty;
        public string PatternToSearch
        {
            get { return pattern; }
            set
            {
                if (pattern != value)
                {
                    pattern = value;
                    RaisePropertyChanged();
                }
            }
        }
        public abstract void Visit(hartCommandVM hartCommandVM);
        public virtual void Process(hartCommandVM hartCommandVM, Action<ObservableCollection<object>, byte[]> processFunc)
        {
            hartCommandVM.QueryDataRepresentations.Clear();
            processFunc(hartCommandVM.QueryDataRepresentations, hartCommandVM.QueryData);
            hartCommandVM.AnswerDataRepresentations.Clear();
            processFunc(hartCommandVM.AnswerDataRepresentations, hartCommandVM.AnswerData);
        }

    }
    public class RawHexStringSearchPattern : SearchPattern
    {
        public override string PatternName { get => "RawHex"; }

        public override void Visit(hartCommandVM hartCommandVM)
        {
            Action<ObservableCollection<object>, byte[]> processFunc = (observableCollection, data) =>
            {
                observableCollection.Clear();
                string displayText = "no answer data";
                if (data.Length == 0)
                {
                    displayText = "answer data length == 0";
                }
                else
                {
                    displayText = BitConverter.ToString(data);
                }
                if (string.IsNullOrEmpty(PatternToSearch) || displayText.Contains(PatternToSearch, StringComparison.OrdinalIgnoreCase))
                {
                    observableCollection.Add(new TextStringVM(displayText));
                }
            };

            Process(hartCommandVM, processFunc);
        }

    }
    public class ASCIIStringSearchPattern : SearchPattern
    {
        public override string PatternName { get => "ASCIIString"; }

        public override void Visit(hartCommandVM hartCommandVM)
        {
            Action<ObservableCollection<object>, byte[]> processFunc = (observableCollection, data) =>
            {
                observableCollection.Clear();
                var asciiString = ASCIIEncoding.ASCII.GetString(data);
                if (string.IsNullOrEmpty(PatternToSearch) || asciiString.Contains(PatternToSearch, StringComparison.OrdinalIgnoreCase))
                {
                    observableCollection.Add(new TextStringVM(asciiString));
                }
            };
            Process(hartCommandVM, processFunc);
        }
    }
    public class TextStringVM : baseVM
    {
        public string DisplayString { get; private set; }
        public TextStringVM(string stringToDisplay)
        {
            DisplayString = stringToDisplay;
        }
    }


    public class PackedASCIIStringSearchPattern : SearchPattern
    {
        public override string PatternName { get => "PackedASCIIString"; }

        public override void Visit(hartCommandVM hartCommandVM)
        {
            Action<ObservableCollection<object>, byte[]> processFunc = (observableCollection, data) =>
            {
                observableCollection.Clear();
                var bytearr = data;
                string unpackedString = string.Empty;
                int offset = 0;
                while (bytearr.Length >= 3)
                {
                    var packedStringParts =
                        Enumerable.Range(0, (bytearr.Length / 3) * 3)
                        .Where(x => x % 3 == 0)
                        .Select(x => HART_unpack_3bytes_to_string(new byte[] { bytearr[x], bytearr[x + 1], bytearr[x + 2] }));
                    unpackedString = string.Join(null, packedStringParts);
                    if (string.IsNullOrEmpty(PatternToSearch) || unpackedString.Contains(PatternToSearch, StringComparison.OrdinalIgnoreCase))
                    {
                        observableCollection.Add(new TextStringVM($"смещение ({offset})={unpackedString}"));
                    }
                    bytearr = bytearr.SubArray(1, bytearr.Length - 1);
                    offset++;
                }
            };
            Process(hartCommandVM, processFunc);
        }
        private string HART_unpack_3bytes_to_string(byte[] bytes)
        {
            // 11111122 22223333 33444444
            if (bytes.Length != 3) throw new ArgumentException();
            var c1 = (bytes[0] & 0b11111100) >> 2;
            var c2 = (bytes[0] & 0b00000011) << 4 | (bytes[1] & 0b11110000) >> 4;
            var c3 = (bytes[1] & 0b00001111) << 2 | (bytes[2] & 0b11000000) >> 6;
            var c4 = (bytes[2] & 0b00111111);

            c1 |= ((c1 & 0b00100000) ^ 0b00100000) << 1;
            c2 |= ((c2 & 0b00100000) ^ 0b00100000) << 1;
            c3 |= ((c3 & 0b00100000) ^ 0b00100000) << 1;
            c4 |= ((c4 & 0b00100000) ^ 0b00100000) << 1;


            var result = System.Text.Encoding.ASCII.GetString(new byte[] { (byte)c1, (byte)c2, (byte)c3, (byte)c4 });
            return result;
        }
    }

    public class IEEE754SearchPattern : SearchPattern
    {
        public override string PatternName { get => "IEEE754"; }

        public override void Visit(hartCommandVM hartCommandVM)
        {
            Action<ObservableCollection<object>, byte[]> processFunc = (observableCollection, data) =>
            {
                observableCollection.Clear();
                var bytearr = data;
                int offset = 0;
                while (bytearr.Length >= 4)
                {
                    Single ieeeSingle = BitConverter.ToSingle(new byte[] { bytearr[3], bytearr[2], bytearr[1], bytearr[0] }, 0);
                    var ieeeString = ieeeSingle.ToString("0.00000000");
                    if (string.IsNullOrEmpty(PatternToSearch) || ieeeString.Replace(',', '.').Contains(PatternToSearch.Replace(',', '.')))
                    {
                        observableCollection.Add(new TextStringVM($"смещение ({offset})={ieeeString}"));
                    }
                    bytearr = bytearr.SubArray(1, bytearr.Length - 1);
                    offset++;
                }
            };
            Process(hartCommandVM, processFunc);

        }
    }
}

