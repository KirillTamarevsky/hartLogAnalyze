using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

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
        public virtual void Visit(hartCommandVM hartCommandVM)
        {
            hartCommandVM.QueryDataRepresentations.Clear();
            if (hartCommandVM.QueryData.Length == 0)
            {
                hartCommandVM.QueryDataRepresentations.Add(new TextStringVM("нет данных"));
            }
            else
            {
                Process(hartCommandVM.QueryDataRepresentations, hartCommandVM.QueryData);
                if ( hartCommandVM.QueryDataRepresentations.Count == 0)
                {
                    hartCommandVM.QueryDataRepresentations.Add(new TextStringVM("нет совпадений"));
                }
            }

            hartCommandVM.AnswerDataRepresentations.Clear();
            if (hartCommandVM.AnswerData.Length == 0)
            { 
                hartCommandVM.AnswerDataRepresentations.Add(new TextStringVM("нет данных"));
            }    
            else
            {
                Process(hartCommandVM.AnswerDataRepresentations, hartCommandVM.AnswerData);
                if (hartCommandVM.AnswerDataRepresentations.Count == 0)
                {
                    hartCommandVM.AnswerDataRepresentations.Add(new TextStringVM("нет совпадений"));
                }
            }
        }
        public abstract void Process(ObservableCollection<object> observableCollection, byte[] data);

    }
    public class RawHexStringSearchPattern : SearchPattern
    {
        public override string PatternName { get => "RawHex"; }

        public override void Process(ObservableCollection<object> observableCollection, byte[] data)
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
        }

    }
    public class ASCIIStringSearchPattern : SearchPattern
    {
        public override string PatternName { get => "ASCIIString"; }

        public override void Process(ObservableCollection<object> observableCollection, byte[] data)
        {
            observableCollection.Clear();
            var asciiString = ASCIIEncoding.ASCII.GetString(data);
            if ( !string.IsNullOrEmpty(asciiString) && ( string.IsNullOrEmpty(PatternToSearch) || asciiString.Contains(PatternToSearch, StringComparison.OrdinalIgnoreCase)) )
            {
                var compositeTextStringVM = new CompositeTextStringVM();

                foreach (var item in asciiString)
                {
                    if (char.IsControl(item))
                    {
                        compositeTextStringVM.Items.Add(new ControlCharacterVM ($"0x{Convert.ToByte(item).ToString("x2")}") );
                    }
                    else
                    {
                        compositeTextStringVM.Items.Add(new TextStringVM( item.ToString() ) );
                    }
                }
                observableCollection.Add( compositeTextStringVM );
            }
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
    public class CompositeTextStringVM : baseVM
    {
        public ObservableCollection<baseVM> Items { get; }
        public CompositeTextStringVM()
        {
            Items = new ObservableCollection<baseVM>();
        }
    }
    public class ControlCharacterVM : baseVM
    {
        public string DisplayString { get; }
        public ControlCharacterVM( string text)
        {
                DisplayString= text;
        }
    }

    public class PackedASCIIStringSearchPattern : SearchPattern
    {
        public override string PatternName { get => "PackedASCIIString"; }

        public override void Process(ObservableCollection<object> observableCollection, byte[] data)
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

        public override void Process(ObservableCollection<object> observableCollection, byte[] data)
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
        }
    }
}

