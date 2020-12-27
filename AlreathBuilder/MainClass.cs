using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

namespace AlreathBuilder
{
    
    class AlreathBuilder
    {
        //Uses different methods due to different structs for the edits so each method sets up the correct type instead of more work in single method
        //Each struct should be serializable and should be the same format that Alreath is expecting to convert it correctly

        //Structs might switch out for classes, it depends how I get them to setup and what I need them for
        
        [Serializable]
        public struct Job {
            // switch to private vals with a class and use methods to set values?  
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

            //  [("WIS", 6)("CON",4)("LCK",7)]


            //Jobs would normally check only prev joblvls, quests and rarely flags to determine if able to switch jobs

            //I will figure out what method to translate the data to determine what is required and how to check those values

            // Dictionary of the requirements to use this class, name: req
            // req is a string allows for certain flags to be checked as well
            //string name will determine if is not a normal check, thus allows for what condition 
            //ex title/boss :equipped
            //   joblvl/mage : <15

            public List<string> requireList;
            //list of requirements need to pass to use job
            // equip of require fail will keep old job
            // string contains multiple parts to strip, 
            // underscore and slash are delimiters, might switch to only use one or another way to split them up

            //Requires mastery of mage to be equipped and to have Hunter unlocked, why this combination who knows
            //  ["title/equip_Mastery of Mage","job/unlock_hunter"]

            public Dictionary<string, int> unlockList;

            // string is the name of the thing unlocked, this string references what type is unlocked similar to require,
            // the string is passed of and the correct thing is unlocked, these are all saved on the server to pull from
            // This reward system is handled ingame against player and server storage

            //int is the lvl required for the unlock, which is rewarded at that lvl
            //ex title/mageMastery, 100 

            //private ??? unlock
            // this is a collection of what is unlocked at each lvl within the this job
            // lvl 1, Fireball
            // lvl 5 Firebolt
            // lvl 100 = Mastery, Title of Mastery of Mage, Unlocks Wizard Job, etc.


        }
        public struct Monster { }

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

                Console.WriteLine("----Job Builder----");
                Console.WriteLine("Choose New Job (N), Edit (E), List (L), Delete Old (DEL), Help (?), Quit (Q)");
                string input = Console.ReadLine();
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

                        Console.WriteLine("Stat Block:");
                        input = Console.ReadLine().ToLower();
                        //job.statBlock = ;
                        Console.WriteLine("UnlockList:");
                        input = Console.ReadLine().ToLower();
                        //job.unlockList = ;


                        string filePath = filepath + "\\" + jobName + ".jb";
                        //Create Item
                        var binaryFormatter = new BinaryFormatter();
                        using (var fileStream = File.Create(filePath))
                            binaryFormatter.Serialize(fileStream, job);

                        break;
                    case ("E"):
                    case ("EDIT"):

                        Console.WriteLine("Input Name of Job to Edit");
                        //Find which file to edit
                        input = Console.ReadLine().ToLower();

                        //TODO

                        var editList = Directory.GetFiles(filepath, "*.jb");
                        foreach (var file in editList)
                        {
                            //This way we can reference new and old files at the same time by string concat or file
                            if (filepath + "\\" + input + ".jb" == file)
                            {
                                Console.WriteLine("Editing " + input);

                                //Now to figure out what to edit by asking questions again, showing what the old val was
                                // Edit ~ to use existing values
                                // Maybe have a mode to append values like data so things can just be added and not replaced
                                // ~+ to add or extend to existing, ~- to remove or subtract component
                                //This way the things can be loaded in a tweaked without breaking older edits from scratch

                                //Console.WriteLine("StatBlock: ");
                                //Console.WriteLine(file.statblock);
                                //input = Console.ReadLine();
                                //if (input != "~" || input != "~+" || input != "~-"){ replace vals } else {  merge new and old   }


                                break;
                            }
                        }
                        //Didnt find file in dir or finished
                        //Console.WriteLine("File not found in Cur Dir");
                        //ENDTODO


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
                        var delList = Directory.GetFiles(filepath,"*.jb");
                        foreach(var file in delList){
                            Console.WriteLine(file);
                            if (filepath+"\\"+input +".jb"== file)
                            {
                                //DEL ITEM
                                Console.WriteLine("Deleted "+input);
                                File.Delete(file);
                                break;
                            }
                        }
                        break;
                    case ("?"):
                    case ("HELP"):
                        Console.WriteLine("This is the Help Txt");
                        Console.WriteLine("This will be filled with help once systems are in place");
                        break;
                    case ("Q"):
                    case ("QUIT"):
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("Incorrect Input, Try ?");
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
                Console.WriteLine("Choose New (N), Edit (E), List (L), Delete (DEL), Help (?), Quit (Q)");
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
                        Int32.TryParse(input,out int res);
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
                        //Will Get list of all Items in DIR\Items
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
                    case ("?"):
                    case ("HELP"):
                        Console.WriteLine("This is the Help Txt");
                        Console.WriteLine("This will be filled with help once systems are in place");
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
            Console.WriteLine("Choose New Monster (N), Edit (E), List (L), Delete Old (DEL), Help (?), Quit (Q)");
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
                        //Will Get list of all Items in DIR\Monsters
                        var listFiles = Directory.GetFiles(filepath, "*.mob");
                        foreach (var file in listFiles)
                        {
                            Console.WriteLine(file);
                        }
                        break;
                case ("DEL"):
                case ("DELETE"):
                    //
                    Console.WriteLine("Input Name of Monster to Delete");
                    input = Console.ReadLine();
                    //Checks in DIR\Monsters for the inputed name
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
                    case ("?"):
                    case ("HELP"):
                        Console.WriteLine("This is the Help Txt");
                        Console.WriteLine("This will be filled with help once systems are in place");
                        break;
                    case ("Q"):
                    case ("QUIT"):
                    loop = false;
                    break;
                default:
                    Console.WriteLine("Incorrect Input");
                    break;
            }
        } }



        static void Main(string[] args)
        {
            //Select filepath for storage

            Console.WriteLine("Server Storage Dir: ");
            string curDir = Console.ReadLine();

            //Enter just sticks it in C drive for now, not where exe is.


            //Start loop
            bool loop = true;
            while (loop) {
                //print out choices to build
                //
                //Will eventually allow for commandline with alreathbuilder.exe J N 
                // to jump to sub processes
                //
                Console.WriteLine("Choose JobBuilder (J), ItemBuilder (I), MonsterBuilder (M)");
                Console.WriteLine("Change Dir (CD), Exit (E)");

                switch (Console.ReadLine().ToUpper())
                {
                    case "J":
                        //make sure path exists
                        Directory.CreateDirectory(curDir + "\\Jobs");
                        JobBuilder(curDir + "\\Jobs");
                        break;
                    case "I":
                        Directory.CreateDirectory(curDir + "\\Items");
                        ItemBuilder(curDir + "\\Items");
                        break;
                    case "M":
                        Directory.CreateDirectory(curDir + "\\Monsters");
                        MonsterBuilder(curDir + "\\Monsters");
                        break;
                    case "CD":
                        Console.WriteLine("Cur Dir: " + curDir);
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