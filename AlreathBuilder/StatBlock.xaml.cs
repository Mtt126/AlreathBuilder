using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for StatBlock.xaml
    /// </summary>
    public partial class StatBlock : UserControl
    {
        public bool load;
        public Dictionary<string,int> statDict;

        public event EventHandler StatUpdate;

        public StatBlock()
        {
            
            InitializeComponent();
            StatBlockInit();
        }
        private void StatBlockInit() 
        {
            load = false;
            if (statDict == null)
            {
                statDict = new Dictionary<string, int>();
                
                Debug.WriteLine("Added Keys");

            }
            RefreshText();

        }

        public void RefreshText() 
        {
            Debug.WriteLine("Stat Refreshed");
            if (!statDict.ContainsKey("Str"))
            {
                statDict.Add("Str", 0);
            }
            if (!statDict.ContainsKey("Dex"))
            {
                statDict.Add("Dex", 0);
            }
            if (!statDict.ContainsKey("Int"))
            {
                statDict.Add("Int", 0);
            }
            if (!statDict.ContainsKey("Wis"))
            {
                statDict.Add("Wis", 0);
            }
            if (!statDict.ContainsKey("Cha"))
            {
                statDict.Add("Cha", 0);
            }
            Debug.Write("Str " + statDict["Str"].ToString());
            Debug.Write("Dex " + statDict["Dex"].ToString());
            Debug.Write("Wis " + statDict["Wis"].ToString());
            Debug.Write("Int " + statDict["Int"].ToString());
            Debug.WriteLine("Cha " + statDict["Cha"].ToString());


            Str_Val.Text = statDict["Str"].ToString();
            Dex_Val.Text = statDict["Dex"].ToString();
            Int_Val.Text = statDict["Int"].ToString();
            Wis_Val.Text = statDict["Wis"].ToString();
            Cha_Val.Text = statDict["Cha"].ToString();


        }

        private void Stat_TextChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("Stat changed");
            if (statDict != null)
            {
                if (load != true)
                {
                    if (statDict.ContainsKey("Str"))
                    {
                        Debug.Write("Str Changed from " + statDict["Str"]);
                        statDict["Str"] = int.Parse(Str_Val.Text);
                        Debug.WriteLine("to " + statDict["Str"]);
                    }
                    Debug.Write("Dex Changed from " + statDict["Dex"]);
                    statDict["Dex"] = int.Parse(Dex_Val.Text);
                    Debug.WriteLine("to " + statDict["Dex"]);

                    Debug.Write("Int Changed from " + statDict["Int"]);
                    statDict["Int"] = int.Parse(Int_Val.Text);
                    Debug.WriteLine("to " + statDict["Int"]);

                    Debug.Write("Wis Changed from " + statDict["Wis"]);
                    statDict["Wis"] = int.Parse(Wis_Val.Text);
                    Debug.WriteLine("to " + statDict["Wis"]);

                    Debug.Write("Cha Changed from " + statDict["Cha"]);
                    statDict["Cha"] = int.Parse(Cha_Val.Text);
                    Debug.WriteLine("to " + statDict["Cha"]);
                }
            }



            StatUpdate_1(sender,e);
        }
        

        protected void StatUpdate_1(object sender, EventArgs e)
        {
            //Debug.WriteLine("StatUpdate Fired");
            //bubble the event up to the parent
            if (this.StatUpdate != null)
                this.StatUpdate(this, e);
        }

        private void Int_Val_TextChanged(object sender, TextChangedEventArgs e)
        {
            Stat_TextChanged(sender,e);
        }

        private void Dex_Val_TextChanged(object sender, TextChangedEventArgs e)
        {
            Stat_TextChanged(sender,e);
        }

        private void Str_Val_TextChanged(object sender, TextChangedEventArgs e)
        {
            Stat_TextChanged(sender, e);
        }

        private void Wis_Val_TextChanged(object sender, TextChangedEventArgs e)
        {
            Stat_TextChanged(sender, e);
        }

        private void Cha_Val_TextChanged(object sender, TextChangedEventArgs e)
        {
            Stat_TextChanged(sender, e);
        }
    }
}
