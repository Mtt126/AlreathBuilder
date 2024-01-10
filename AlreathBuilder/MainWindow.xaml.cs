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

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;


namespace AlreathBuilderGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Structs
        [Serializable]
        public struct Job
        {
            // switch to a private and methods to set values?  
            // or leave public to allow tweaks
            public string name;
            private string desc;

            private int exp;
            private int lvl;

            public Dictionary<string, int> statBlock;
            // Statblock is the stats that are added to the base stats every lvl, 
            // so lvl 1 mage has +5 INT and +5 WIS,
            // lvl 5 mage has +25 Int and +25 WIS,
            // lvl 1 Wizard had +10 INT and + 10 WIS

            //This require dictionary may not just apply to jobs so extra info is applied to description
            //Jobs would normally check only prev joblvls, quests and rarely flags

            // Dictionary of the requirements to use this class, name: req
            // req is a string allows for certain flags to be checked as well
            //string name will determine if is not a normal check, thus allows for what condition 
            //ex title/boss :equipped
            //   joblvl/mage : <15

            public List<string> requireList;
            //list of requirements need to pass to use job
            // equip of require fail will keep old job
            // string contains multiple parts to strip, 
            // underscore is delimiter
            // title_equip_Mastery of Mage
            // job_unlock_

            public Dictionary<string, int> unlockList;

            // string is the name of the thing unlocked, this string references what type is unlocked similar to require,
            // the string is passed of and the correct thing is unlocked, these are all saved on the server to pull from
            //ex title/mageMastery, 100 

            //private ??? unlock
            // this is a collection of what is unlocked at each lvl within the this job
            // lvl 1, Fireball
            // lvl 5 Firebolt
            // lvl 100 = Mastery, Title of Mastery of Mage, Unlocks Wizard Job, etc.
        }

        public struct Monster { }


        //Vars
        string curDir;


        public MainWindow()
        {
            InitializeComponent();

            //Setup vars
            curDir = Directory.GetCurrentDirectory();

        }

        private void JobsBut(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(curDir + "\\Jobs");
            JobBuilder(curDir + "\\Jobs");
        }
        private void MonsterBut(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(curDir + "\\Monsters");
            MonsterBuilder(curDir + "\\Monsters");
        }
        private void ItemBut(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(curDir + "\\Items");
            ItemBuilder(curDir + "\\Items");
        }

        private void QuestBut(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(curDir + "\\Quests");
            QuestBuilder(curDir + "\\Quests");
        }
        private void FeatBut(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(curDir + "\\Feats");
            FeatBuilder(curDir + "\\Feats");
        }

        //Save and Load don't happen here, Save on window closure, load happens on all in directory on BuilderMethods



        private static void JobBuilder(string filepath, string command = "")
        {
            //JobBuilder
            //  --Stat Block
            //  --Lvl Unlocks
            //      --Skills
            //      --Titles (which trigger mastery)
            //      --Job Unlock
            bool loop = true;
            while (loop)
            {
                //Console.WriteLine("----Job Builder----");
                //Console.WriteLine("Choose New Item (N), Edit (E), List (L), Delete Old (DEL), Quit (Q)");
                //string input = Console.ReadLine();
                string input = "";
                switch (input.ToUpper())
                {
                    case ("N"):
                    case ("NEW"):
                        //New item 
                        Console.WriteLine("Input Name of Job to Create (converts to lower)");
                        input = Console.ReadLine().ToLower();

                        Job job = new Job();

                        string jobName = input;
                        //Set values for job
                        job.name = input;


                        string filePath = filepath + "\\" + jobName + ".jb";
                        //Create Item
                        var binaryFormatter = new BinaryFormatter();
                        using (var fileStream = File.Create(filePath))
                            binaryFormatter.Serialize(fileStream, job);

                        break;
                    case ("E"):
                    case ("EDIT"):

                        Console.WriteLine("Input Name of Job to Edit");
                        //
                        input = Console.ReadLine().ToLower();

                        //TODO

                        break;
                    case ("L"):
                    case ("LIST"):
                        //Will Get list of all Items in DIR\Jobs
                        var listFiles = Directory.GetFiles(filepath, "*.jb");
                        foreach (var file in listFiles)
                        {
                            Console.WriteLine(file);
                        }
                        break;
                    case ("DEL"):
                    case ("DELETE"):
                        //
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
                    case ("Q"):
                    case ("QUIT"):
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("Incorrect Input");
                        break;
                }
            }
        }
        private static void ItemBuilder(string filepath)
        {
            //ItemBuilder
            //  --Basic Item Data
            //      --Name,desc,Amount,worth, script
            //  --Script Data, if script not null
            //      --Damage Type
            //      --Weapon Speed
            //      --Weapon Type (for animations)
            bool loop = true;
            while (loop)
            {

                Console.WriteLine("----Item Builder----");
                Console.WriteLine("Choose New (N), Edit (E), List (L), Delete (DEL), Quit (Q)");
                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case ("N"):
                    case ("NEW"):
                        //New item 
                        Console.WriteLine("Input Name of Item to Create");
                        input = Console.ReadLine();

                        Hashtable item = new Hashtable();

                        string itemName = input;
                        item["Name"] = itemName;
                        Console.WriteLine("Item Desc:");
                        input = Console.ReadLine();
                        item["Desc"] = input;
                        item["Amount"] = 1;
                        Console.WriteLine("Item Base Worth:");
                        input = Console.ReadLine();
                        Int32.TryParse(input, out int res);
                        item["Worth"] = res;
                        Console.WriteLine("Item Script:");
                        input = Console.ReadLine();
                        item["Script"] = input;
                        Console.WriteLine("Script Data:");
                        input = Console.ReadLine();
                        item["SData"] = input;

                        string filePath = filepath + "\\" + itemName + ".item";
                        //Create Item
                        var binaryFormatter = new BinaryFormatter();
                        using (var fileStream = File.Create(filePath))
                            binaryFormatter.Serialize(fileStream, item);


                        break;
                    case ("E"):
                    case ("EDIT"):

                        Console.WriteLine("Input Name of Item to Edit");
                        //
                        input = Console.ReadLine();
                        //TODO


                        break;
                    case ("L"):
                    case ("LIST"):
                        //Will Get list of all Items in DIR\Jobs
                        var listFiles = Directory.GetFiles(filepath, "*.item");
                        foreach (var file in listFiles)
                        {
                            Console.WriteLine(file);
                        }
                        break;
                    case ("DEL"):
                    case ("DELETE"):
                        //
                        Console.WriteLine("Input Name of Item to Delete");
                        input = Console.ReadLine();
                        //Checks in DIR\Items for the inputed name
                        var delList = Directory.GetFiles(filepath, "*.item");
                        foreach (var file in delList)
                        {
                            if (input == file)
                            {
                                //DEL ITEM
                                File.Delete(filepath + "\\" + file);
                            }
                        }
                        break;
                    case ("Q"):
                    case ("QUIT"):
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("Incorrect Input");
                        break;
                }
            }
        }

        private static void MonsterBuilder(string filepath)
        {
            //Monster Builder
            //  --Monster type, class
            //  --Monster Stats
            //  --Monster abilities
            //  --Monster Loot Table
            bool loop = true;
            while (loop)
            {

                Console.WriteLine("----Monster Builder----");
                Console.WriteLine("Choose New Item (N), Edit (E), List (L), Delete Old (DEL), Quit (Q)");
                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case ("N"):
                        //New item 
                        Console.WriteLine("Input Name of Monster to Create");
                        input = Console.ReadLine();

                        //Item item = new Item();
                        Hashtable item = new Hashtable();

                        string itemName = input;
                        item["Name"] = itemName;

                        string filePath = filepath + "\\" + itemName + ".mob";
                        //Create Item
                        var binaryFormatter = new BinaryFormatter();
                        using (var fileStream = File.Create(filePath))
                            binaryFormatter.Serialize(fileStream, item);


                        break;
                    case ("E"):
                    case ("EDIT"):
                        Console.WriteLine("Input Name of Monster to Edit");
                        //
                        input = Console.ReadLine();
                        //TODO
                        break;
                    case ("L"):
                    case ("LIST"):
                        //Will Get list of all Items in DIR\Jobs
                        var listFiles = Directory.GetFiles(filepath, "*.mob");
                        foreach (var file in listFiles)
                        {
                            Console.WriteLine(file);
                        }
                        break;
                    case ("DEL"):
                        //
                        Console.WriteLine("Input Name of Monster to Delete");
                        input = Console.ReadLine();
                        //Checks in DIR\Items for the inputed name
                        var delList = Directory.GetFiles(filepath, "*.mob");
                        foreach (var file in delList)
                        {
                            if (input == file)
                            {
                                //DEL ITEM
                                File.Delete(filepath + "\\" + file);
                            }
                        }
                        break;
                    case ("Q"):
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("Incorrect Input");
                        break;
                }
            }
        }

        private static void QuestBuilder(string filepath)
        {
            //Monster Builder
            //  --Monster type, class
            //  --Monster Stats
            //  --Monster abilities
            //  --Monster Loot Table
            bool loop = true;
            while (loop)
            {

                Console.WriteLine("----Monster Builder----");
                Console.WriteLine("Choose New Item (N), Edit (E), List (L), Delete Old (DEL), Quit (Q)");
                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case ("N"):
                        //New item 
                        Console.WriteLine("Input Name of Monster to Create");
                        input = Console.ReadLine();

                        //Item item = new Item();
                        Hashtable item = new Hashtable();

                        string itemName = input;
                        item["Name"] = itemName;

                        string filePath = filepath + "\\" + itemName + ".mob";
                        //Create Item
                        var binaryFormatter = new BinaryFormatter();
                        using (var fileStream = File.Create(filePath))
                            binaryFormatter.Serialize(fileStream, item);


                        break;
                    case ("E"):
                    case ("EDIT"):
                        Console.WriteLine("Input Name of Monster to Edit");
                        //
                        input = Console.ReadLine();
                        //TODO
                        break;
                    case ("L"):
                    case ("LIST"):
                        //Will Get list of all Items in DIR\Jobs
                        var listFiles = Directory.GetFiles(filepath, "*.mob");
                        foreach (var file in listFiles)
                        {
                            Console.WriteLine(file);
                        }
                        break;
                    case ("DEL"):
                        //
                        Console.WriteLine("Input Name of Monster to Delete");
                        input = Console.ReadLine();
                        //Checks in DIR\Items for the inputed name
                        var delList = Directory.GetFiles(filepath, "*.mob");
                        foreach (var file in delList)
                        {
                            if (input == file)
                            {
                                //DEL ITEM
                                File.Delete(filepath + "\\" + file);
                            }
                        }
                        break;
                    case ("Q"):
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("Incorrect Input");
                        break;
                }
            }
        }

        private static void FeatBuilder(string filepath)
        {
            //Monster Builder
            //  --Monster type, class
            //  --Monster Stats
            //  --Monster abilities
            //  --Monster Loot Table
            bool loop = true;
            while (loop)
            {

                Console.WriteLine("----Monster Builder----");
                Console.WriteLine("Choose New Item (N), Edit (E), List (L), Delete Old (DEL), Quit (Q)");
                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case ("N"):
                        //New item 
                        Console.WriteLine("Input Name of Monster to Create");
                        input = Console.ReadLine();

                        //Item item = new Item();
                        Hashtable item = new Hashtable();

                        string itemName = input;
                        item["Name"] = itemName;

                        string filePath = filepath + "\\" + itemName + ".mob";
                        //Create Item
                        var binaryFormatter = new BinaryFormatter();
                        using (var fileStream = File.Create(filePath))
                            binaryFormatter.Serialize(fileStream, item);


                        break;
                    case ("E"):
                    case ("EDIT"):
                        Console.WriteLine("Input Name of Monster to Edit");
                        //
                        input = Console.ReadLine();
                        //TODO
                        break;
                    case ("L"):
                    case ("LIST"):
                        //Will Get list of all Items in DIR\Jobs
                        var listFiles = Directory.GetFiles(filepath, "*.mob");
                        foreach (var file in listFiles)
                        {
                            Console.WriteLine(file);
                        }
                        break;
                    case ("DEL"):
                        //
                        Console.WriteLine("Input Name of Monster to Delete");
                        input = Console.ReadLine();
                        //Checks in DIR\Items for the inputed name
                        var delList = Directory.GetFiles(filepath, "*.mob");
                        foreach (var file in delList)
                        {
                            if (input == file)
                            {
                                //DEL ITEM
                                File.Delete(filepath + "\\" + file);
                            }
                        }
                        break;
                    case ("Q"):
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("Incorrect Input");
                        break;
                }
            }
        }


        private void QuitBut(object sender, RoutedEventArgs e)
        {

        }
    }
}



namespace AlreathBuilder
{

    class AlreathBuilder
    {

        static void Main(string[] args)
        {
            //Select filepath for storage

            Console.WriteLine("Server Storage Dir: ");
            string curDir = Console.ReadLine();

            //Start loop
            bool loop = true;
            while (loop)
            {
                //print out choices to build
                //
                //Will eventually allow for commandline with alreathbuilder.exe J N 
                // to jump to sub processes
                //
                Console.WriteLine("Choose JobBuilder (J), ItemBuilder (I), MonsterBuilder (M)");
                Console.WriteLine("Change Dir (CD), Exit (E)");

                switch (Console.ReadLine().ToUpper())
                {
                    case "CD":
                        Console.WriteLine("New Storage Dir:");
                        curDir = Console.ReadLine();
                        break;
                    case "E":
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("Incorrect Input");
                        break;
                }
            }
        }

    }
}