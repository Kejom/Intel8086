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

        public Dictionary<string, RegisterParameter> Registers { get; set; }
        public MainWindow()
        {

            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            PopulateList();
            this.Register1.ItemsSource = selectionListAll;
            this.Register1.SelectedItem = selectionListAll[0];
            //this.Register2.ItemsSource = list;
            //this.Register2.SelectedItem = list[0];
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

            selectionListOnlyFull = new ObservableCollection<string>(selectionListAll.Where(i => i[1] == 'X'));
            selectionListOnlyHalfs = new ObservableCollection<string>(selectionListAll.Where(i => i[1] != 'X'));
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
            };
            //Registers.Add("AX", new RegisterParameter("0000"));
            //Registers.Add("AH", new RegisterParameter("00"));
            //Registers.Add("AL", new RegisterParameter("00"));
            //Registers.Add("BX", new RegisterParameter("0000"));
            //Registers.Add("BH", new RegisterParameter("00"));
            //Registers.Add("BL", new RegisterParameter("00"));
            //Registers.Add("CX", new RegisterParameter("0000"));
            //Registers.Add("CH", new RegisterParameter("00"));
            //Registers.Add("CL", new RegisterParameter("00"));
            //Registers.Add("DX", new RegisterParameter("0000"));
            //Registers.Add("DH", new RegisterParameter("00"));
            //Registers.Add("DL", new RegisterParameter("00"));
        }

        private void MovCommand()
        {
            string startRegister = this.Register1.Text;
            string targetRegister = this.Register2.Text;

            Registers[targetRegister].Value = Registers[startRegister].Value;
        }

        private void XchgCommand()
        {
            string startRegister = this.Register1.Text;
            string targetRegister = this.Register2.Text;

            var cache = Registers[targetRegister].Value;
            Registers[targetRegister].Value = Registers[startRegister].Value;
            Registers[startRegister].Value = cache;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string selectedAction = this.Command.Text;

            if (selectedAction == "MOV")
                MovCommand();
            if (selectedAction == "XCHG")
                XchgCommand();

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

        private void HalfRegisterBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var fieldName = ((TextBox)sender).Name;
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
            if (this.Command.SelectedValue == null | this.Register1.SelectedValue == null)
                return;

            var optionSelected = this.Command.SelectedValue.ToString();
            var firstRegister = this.Register1.SelectedValue.ToString();

            if (firstRegister[1] == 'X')
            {
                this.Register2.ItemsSource = selectionListOnlyFull;
                this.Register2.SelectedItem = selectionListOnlyFull[0];
            }
            else
            {
                this.Register2.ItemsSource = selectionListOnlyHalfs;
                this.Register2.SelectedItem = selectionListOnlyHalfs[0];
            }



        }
    }
}
