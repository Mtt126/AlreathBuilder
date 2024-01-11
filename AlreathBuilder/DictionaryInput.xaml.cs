using System;
using System.Collections.Generic;
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

namespace AlreathBuilderGUI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DictionaryInput : UserControl
    {
        //Control for custom Dictionary Lists, expandable <string,string>
        //conversion needed to swap into another type cast
        public Dictionary<string,string> dic;


        public DictionaryInput()
        {
            InitializeComponent();
        }


        private void AddPair(string key) 
        { 
        //Adds a Label using key
        //Blank String


        }



    }
}
