using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static AlreathBuilderGUI.MainWindow;

namespace AlreathBuilderGUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class JobWindow : Window
    {
        public JobWindow()
        {
            InitializeComponent();
            JobInit();
        }

        List<Job> jobList;
        Job curJob;
        bool load;
        private void JobInit() {
            load = false;
            //Blank List
            jobList = new List<Job>();
            //Init Job
            NewJob();
            
            //LoadList for dropdown
            JobsLoad();

        }

        private void NewJob() 
        {
            curJob = new Job()
            {
                name = "newJob",
                desc = "A blank Job",
                statBlock = new Dictionary<string, int>
                {
                    { "Str", 0 },
                    { "Dex", 0 },
                    { "Int", 0 },
                    { "Wis", 0 },
                    { "Cha", 0 }
                },
                requireList = new List<string>(),
                unlockList = new Dictionary<string, int>()
            };
            
        }

        private void JobsLoad() {
            //Adds all Jobs in storage to the dropdown, along a New Job Selection Option

            //Clear Children
            SelectedBox.Items.Clear();

            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Jobs\\", "*.jb")) 
            {
                Debug.WriteLine("Added to SelectedBox" + System.IO.Path.GetFileName(file));
                SelectedBox.Items.Add(file);
                //Add to JobList?
            }
            //Add SelectBox NewJob
            SelectedBox.Items.Add("NewJob");

        }
        private void JobBu() {
            //JobBuilder
            //  --Stat Block
            //  --Lvl Unlocks
            //      --Skills
            //      --Titles (which trigger mastery)
            //      --Job Unlock
            
                //Console.WriteLine("----Job Builder----");
                //Console.WriteLine("Choose New Item (N), Edit (E), List (L), Delete Old (DEL), Quit (Q)");
                //string input = Console.ReadLine();
                string input = "";
                string filepath = "\\";
                switch (input.ToUpper())
                {
                    case ("E"):
                    case ("EDIT"):

                        Console.WriteLine("Input Name of Job to Edit");
                        //
                        input = Console.ReadLine().ToLower();

                        //TODO

                        break;

                    case ("DEL"):
                    case ("DELETE"):
                        Console.WriteLine("Input Name of Item to Delete");
                        input = Console.ReadLine().ToLower();
                        //Checks in DIR\Items for the inputed name
                        var delList = Directory.GetFiles(filepath, "*.jb");
                        foreach (var file in delList)
                        {
                            Console.WriteLine(file);
                            if (filepath + "\\" + input + ".jb" == file)
                            {
                                //DEL ITEM
                                Console.WriteLine("Deleted " + input);
                                File.Delete(file);
                                break;
                            }
                        }
                        break;
                    default:
                        Console.WriteLine("Incorrect Input");
                        break;
                }
            

        }

        private void LoadJob(string filename) 
        {
            curJob = new Job();
            string filepath = System.IO.Path.GetFullPath(filename);
            Debug.WriteLine("File Name" + filename);


            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite);
            curJob = (Job) binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            //Update UI
            JobName.Text = curJob.name;
            JobDesc.Text = curJob.desc;

            foreach (KeyValuePair<string,int> KV in curJob.statBlock) 
            {
                Debug.Write(KV.Key + " " +KV.Value.ToString());
            
            }
            Debug.Write("\n");
            JobStatBlock.load = true;
            JobStatBlock.statDict = curJob.statBlock;

            JobStatBlock.RefreshText();
            JobStatBlock.load = false;
        }

        private void SaveJob() 
        {
            Debug.WriteLine($"Job {curJob.name}");
            //CurrentDirectory
            string filePath = Directory.GetCurrentDirectory() +"\\Jobs\\" + curJob.name + ".jb";

            //Create Item
            var binaryFormatter = new BinaryFormatter();
            var fileStream = File.Open(filePath,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.ReadWrite);
            binaryFormatter.Serialize(fileStream, curJob);
            fileStream.Close();
            
        }

        private void JobFinishedBut(object sender, RoutedEventArgs e)
        {
            //Return to MainWindow
            DialogResult = true;
        }

        private void SaveJobBut(object sender, RoutedEventArgs e)
        {
            SaveJob();
            JobsLoad();
        }

        private void SelectedBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Selected: " + SelectedBox.Text);
            //If "NewJob" Selected
            if (SelectedBox.SelectedIndex == SelectedBox.Items.Count - 1)
            {
                NewJob();
            }

            else
            {
                //Change curJob to SelectedBox.SelectedItem

                LoadJob(SelectedBox.SelectedItem.ToString());

            }

        }

        private void JobName_TextChanged(object sender, TextChangedEventArgs e) 
        {
            curJob.name = JobName.Text;
        }

        private void JobDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            curJob.desc = JobDesc.Text;
        }

        private void StatBlock_TextChanged(object sender, EventArgs e) 
        {
            JobStatBlock.StatUpdate += new EventHandler(StatBlockUpdate);
        }
        
        protected void StatBlockUpdate(object sender, EventArgs e) 
        {
            curJob.statBlock = JobStatBlock.statDict;
        }
    }
}
