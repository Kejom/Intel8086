using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Intel8086
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> selectionListAll = new ObservableCollection<string>();

        public ObservableCollection<string> selectionListOnlyFull;

        public ObservableCollection<string> selectionListOnlyHalfs;

        public ObservableCollection<string> selectionListFullAndMem;


        public Dictionary<string, RegisterParameter> Registers { get; set; }

        private IntelStack stack;
        private IntelMemory memory;
        public MainWindow()
        {

            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            PopulateList();
            this.Register1.ItemsSource = selectionListAll;
            this.Register1.SelectedItem = selectionListAll[0];
            stack = new IntelStack();
            memory = new IntelMemory();

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeRegisters();
            this.DataContext = Registers;
        }
        private void PopulateList()
        {
            selectionListAll.Add("AX");
            selectionListAll.Add("AH");
            selectionListAll.Add("AL");
            selectionListAll.Add("BX");
            selectionListAll.Add("BH");
            selectionListAll.Add("BL");
            selectionListAll.Add("CX");
            selectionListAll.Add("CH");
            selectionListAll.Add("CL");
            selectionListAll.Add("DX");
            selectionListAll.Add("DH");
            selectionListAll.Add("DL");
            selectionListAll.Add("SI+DISP");
            selectionListAll.Add("DI+DISP");
            selectionListAll.Add("BP+DISP");
            selectionListAll.Add("BX+DISP");
            selectionListAll.Add("SI+BP+DISP");
            selectionListAll.Add("DI+BP+DISP");
            selectionListAll.Add("SI+BX+DISP");
            selectionListAll.Add("DI+BX+DISP");

            selectionListOnlyFull = new ObservableCollection<string>(selectionListAll.Where(i => i[1] == 'X' && i.Length == 2));
            selectionListOnlyHalfs = new ObservableCollection<string>(selectionListAll.Where(i => i[1] != 'X' && i.Length == 2));
            var memActions = new ObservableCollection<string>(selectionListAll.Where(i => i.Length > 2));
            selectionListFullAndMem = new ObservableCollection<string>(selectionListOnlyFull.Concat(memActions));
        }

        private void InitializeRegisters()
        {
            Registers = new Dictionary<string, RegisterParameter>() {
                {"AX", new RegisterParameter("0000")},
                {"AH", new RegisterParameter("00") },
                {"AL", new RegisterParameter("00") },
                {"BX", new RegisterParameter("0000")},
                {"BH", new RegisterParameter("00") },
                {"BL", new RegisterParameter("00") },
                {"CX", new RegisterParameter("0000")},
                {"CH", new RegisterParameter("00") },
                {"CL", new RegisterParameter("00") },
                {"DX", new RegisterParameter("0000")},
                {"DH", new RegisterParameter("00") },
                {"DL", new RegisterParameter("00") },
                {"SI", new RegisterParameter("0000")},
                {"DI", new RegisterParameter("0000")},
                {"BP", new RegisterParameter("0000")},
                {"SP", new RegisterParameter("0000")},
                {"DISP", new RegisterParameter("0000")},
            };

        }

        private void MovCommand()
        {
            string startRegister = this.Register1.Text;
            string targetRegister = this.Register2.Text;

            if(startRegister.Length > 2 || targetRegister.Length > 2)
            {
                MemMovCommand();
                return;
            }

            Registers[targetRegister].Value = Registers[startRegister].Value;

            if (targetRegister[1] != 'X')
                HalfRegisterBoxTextChanged(targetRegister);

        }

        private void MemMovCommand()
        {
            string startRegister = this.Register1.Text;
            string targetRegister = this.Register2.Text;
            var indexes = new string[0];
            var hashes = new string[0];
            var newValue = "";

            if(startRegister.Length > 2)
            {
                indexes = startRegister.Split('+');
                hashes = indexes.Select(n => Registers[n].Value).ToArray();
                newValue = hashes.Length > 2 ? memory.Get(hashes[0], hashes[1], hashes[2])  : memory.Get(hashes[0], hashes[1]);
                Registers[targetRegister].Value = newValue;
                return;
            }
            indexes = targetRegister.Split('+');
            hashes = indexes.Select(n => Registers[n].Value).ToArray();
            newValue = Registers[startRegister].Value;
            if (hashes.Length > 2)
                memory.Mov(hashes[0], hashes[1], hashes[2], newValue);
            else
                memory.Mov(hashes[0], hashes[1], newValue);
        }

        private void XchgCommand()
        {
            string startRegister = this.Register1.Text;
            string targetRegister = this.Register2.Text;

            if (startRegister.Length > 2 || targetRegister.Length > 2)
            {
                MemXchgCommand();
                return;
            }

            var cache = Registers[targetRegister].Value;
            Registers[targetRegister].Value = Registers[startRegister].Value;
            Registers[startRegister].Value = cache;

            if (startRegister[1] != 'X')
                HalfRegisterBoxTextChanged(startRegister);
            if (targetRegister[1] != 'X')
                HalfRegisterBoxTextChanged(targetRegister);
        }

        private void MemXchgCommand()
        {
            string startRegister = this.Register1.Text;
            string targetRegister = this.Register2.Text;
            var memoryRegister = startRegister.Length > 2 ? startRegister : targetRegister;
            var intelRegister = startRegister.Length > 2 ? targetRegister : startRegister;

            var cache = Registers[intelRegister].Value;
            var indexes = memoryRegister.Split('+');
            var hashes = indexes.Select(n => Registers[n].Value).ToArray();
            Registers[intelRegister].Value = hashes.Length > 2 ? memory.Xchg(hashes[0], hashes[1], hashes[2], cache) : memory.Xchg(hashes[0], hashes[1], cache);
        }
        private void PushCommand()
        {
            var selectedRegister = this.Register1.Text;
            var registerValue = Registers[selectedRegister].Value;

            stack.Push(registerValue);
        }

        private void PopCommand()
        {
            var selectedRegister = this.Register1.Text;
            var popedValue = stack.Pop();
            Registers[selectedRegister].Value = popedValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate())
                return;

            string selectedAction = this.Command.Text;

            if (selectedAction == "MOV")
                MovCommand();
            if (selectedAction == "XCHG")
                XchgCommand();
            if (selectedAction == "PUSH")
                PushCommand();
            if (selectedAction == "POP")
                PopCommand();

            MessageBox.Show(string.Format("Wykonano operację \"{0}\"", selectedAction), "Sukces!");

        }

        private void RegisterBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var fieldName = ((TextBox)sender).Name;
            string higherBits = fieldName[0] + "H";
            string lowerBits = fieldName[0] + "L";

            var fieldValue = Registers[fieldName].Value;

            if(fieldValue.Length == 4)
            {
                Registers[higherBits].Value = fieldValue.Substring(0, 2);
                Registers[lowerBits].Value = fieldValue.Substring(2, 2);
            }

        }

        private void HalfRegisterBoxTextChanged(string fieldName)
        {
            string higherBits = fieldName[0] + "H";
            string lowerBits = fieldName[0] + "L";
            string fullRegister = fieldName[0] + "X";
            
            if(Registers[fieldName].Value.Length == 2)
            {
                Registers[fullRegister].Value = Registers[higherBits].Value + Registers[lowerBits].Value;
            }
        }

        private void Command_Selected(object sender, RoutedEventArgs e)
        {
            var combobox = (ComboBox)sender;
            if (Command.SelectedValue == null | Register1.SelectedValue == null)
                return;

            var selectedItem = (ComboBoxItem)Command.SelectedValue;
            var optionSelected = selectedItem.Content.ToString();
            var firstRegister = Register1.SelectedValue.ToString();

            if(optionSelected == "PUSH" || optionSelected == "POP")
            {
                Register1.ItemsSource = selectionListOnlyFull;
                Register1.SelectedItem = Register1.SelectedItem == null ? selectionListOnlyFull[0] : Register1.SelectedItem;
                toggleRegister2();
                return;
            }
            toggleRegister2(false);
            Register1.ItemsSource = selectionListAll;
            if(firstRegister.Length > 2)
            {
                Register2.ItemsSource = selectionListOnlyFull;
                Register2.SelectedItem = selectionListOnlyFull[0];

                return;
            }
            if (firstRegister[1] == 'X')
            {
                Register2.ItemsSource = selectionListFullAndMem;
                Register2.SelectedItem = selectionListFullAndMem[0];

            }
            else
            {
                Register2.ItemsSource = selectionListOnlyHalfs;
                Register2.SelectedItem = selectionListOnlyHalfs[0];
            }



        }

        private void toggleRegister2(bool hide = true)
        {
            var newValue = hide ? Visibility.Hidden : Visibility.Visible;
            var newSpan = hide ? 4 : 6;
            this.Register2.Visibility = newValue;
            this.R2Label.Visibility = newValue;
            ControlsBorder.SetValue(Grid.ColumnSpanProperty, newSpan);

        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var item in Registers)
            {
                item.Value.Value = new string('0', item.Value.Value.Length);
            }
        }

        private bool Validate()
        {
            foreach (var item in Registers)
            {
                if (item.Value.Value.Length != 4 ||!Helpers.validateHex(item.Value.Value))
                {
                    MessageBox.Show("Wszystkie pola muszą zawierać poprawne dane, akceptowane są tylko znaki hexadecymalne (0-9, A-F). \nKażde pole musi zawierać 4 znaki.", "Błąd Walidacji!");
                    return false;
                }
            }
            return true;
        }
    }
}
