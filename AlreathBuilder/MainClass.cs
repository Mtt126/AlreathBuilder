﻿using System;
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
        // Classes would allow the data to be hidden and more self managed, with returning info for the data that can be shared
        
        [Serializable]
        public struct Job {
            // switch to private vals with a class and use methods to set values?  
            // or leave public to allow tweaks
            public string name;
            public string desc;

            public int exp;
            public int lvl;

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

            //Requires mastery of mage to be equipped and to have Hunter unlocked, why this example combination who knows
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

        public struct Item{ }

        private static void JobBuilder(string filepath, string command = "")
        {
            //JobBuilder
            //  --Stat Block
            //  --Requirements
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

                        Job job = new Job
                        {
                            //Set default values for job on init, name can be input as this was just entered
                            // I could setup defaults here to be overridden so the memory is atleast filled untill replaced
                            name = input, desc = "Un-Init Desc", exp = 0, lvl = 0, statBlock = { }, requireList = { }, unlockList = { }
                        };

                        Console.WriteLine("Job Desc:");
                        job.desc = Console.ReadLine();

                        Console.WriteLine("Stat Block:");
                        Console.WriteLine("EX: CHA,5 STR,7 WIS,8 ");
                        input = Console.ReadLine().ToLower();
                        
                        string[] inputstats = input.Split(' ');
                        int statcount = inputstats.Length;
                        Dictionary<string, int> stats = new Dictionary<string, int>();
                        // Split produces 1 as default, so need to check if blank
                        if (statcount != 1 && inputstats[0]!="") {
                            //Console.WriteLine(statcount);
                            foreach (string stat in inputstats) {
                                string st = stat.Split(',')[0];
                                int val = int.Parse(stat.Split(',')[1]);
                                Console.WriteLine(st);
                                stats.Add(st, val);
                            }
                        }
                        job.statBlock = stats;

                        Console.WriteLine("Requirements:");
                        Console.WriteLine("EX: Skill_LVL/Fireball, Job_Unlock/Warden, Title_Equip/Mastery of Mage ");
                        Console.WriteLine("Comma for split items, _ for more sub reqs, / for nameof req");

                        input = Console.ReadLine().ToLower();

                        string[] requireLst = input.Split(',');
                        int reqcount = requireLst.Length;
                        List<string> reqs = new List<string>();
                        // Split produces 1 as default, so its always something even if blank
                        // should also check if the split is empty or correct, so an empty isnt included in the dict
                        if (reqcount != 1 && requireLst[0] != "")
                        {
                            //Just copy the array to list for now, might switch to a struct system with flags instead and use a system as the others


                            reqs = requireLst.ToList();

                            //foreach (string req in requireLst)
                            //{
                            //    reqs.Add(req);
                            //}
                        }
                        job.requireList = reqs;

                        Console.WriteLine("UnlockList:");
                        Console.WriteLine("EX: Skill/Fireball,1 Job/Warden,15 Title/Mastery,100 ");
                        Console.WriteLine("Comma for split items, / for typing");
                        input = Console.ReadLine().ToLower();

                        string[] inputUnlock = input.Split(' ');
                        int unlockcount = inputUnlock.Length;
                        Dictionary<string, int> unlocks = new Dictionary<string, int>();
                        // Split produces 1 as default, so its always something even if blank
                        // should also check if the split is empty or correct, so an empty isnt included in the dict
                        if (unlockcount !=1)
                        {
                            //Console.WriteLine(unlockcount);
                            foreach (string unlock in inputUnlock)
                            {
                                string ul = unlock.Split(',')[0];
                                int val = int.Parse(unlock.Split(',')[1]);
                            }
                        }
                        job.unlockList = unlocks;

                        

                        string filePath = filepath + "\\" + job.name + ".jb";
                        //Create file and write data
                        var binaryFormatter = new BinaryFormatter();
                        using (var fileStream = File.Open(filePath,FileMode.OpenOrCreate))
                        {
                            binaryFormatter.Serialize(fileStream, job);
                            fileStream.Close();
                        }
                        break;

                    case ("E"):
                    case ("EDIT"):

                        Console.WriteLine("Input Name of Job to Edit");
                        //Find which file to edit
                        input = Console.ReadLine().ToLower();

                        //TODO

                        var editList = Directory.GetFiles(filepath, "*.jb");
                        foreach (var fileStr in editList)
                        {
                            //This way we can reference new and old files at the same time by string concat or file
                            // editjob is the edited job which will be saved as file after done editing 
                            // fileStr is path of old file


                            if (filepath + "\\" + input + ".jb" == fileStr)
                            {
                                Console.WriteLine("Editing " + input);

                                //Now to figure out what to edit by asking questions again, showing what the old val was
                                // Edit ~ to use existing values
                                // Maybe have a mode to append values like data so things can just be added and not replaced
                                // ~+ to add or extend to existing, ~- to remove or subtract component
                                //This way the things can be loaded in a tweaked without breaking older edits from scratch


                                // file is the loaded job, the local saved file 

                                using (var fileStream = File.Open(fileStr, FileMode.Open,FileAccess.ReadWrite))
                                {
                                    var binaryFormatter2 = new BinaryFormatter();
                                    Job file = (Job)binaryFormatter2.Deserialize(fileStream);

                                    Console.WriteLine("StatBlock: ");
                                    Dictionary<string, int> newStats = new Dictionary<string, int>();
                                    //Loop thro the statblocks to display the existing data
                                    foreach (var sta in file.statBlock) 
                                    {
                                        Console.WriteLine(sta);
                                        Console.WriteLine("~ to keep, ~- to remove, ~+ to add to it, any val to replace");
                                        input = Console.ReadLine();
                                        if (input[0] != '~' )
                                        {
                                            //override vals
                                            newStats[sta.Key] = int.Parse(input);
                                        }
                                        else
                                        {
                                            //merge new and old, action depends on input status
                                            int change = 0;
                                            if (input.Length > 1 )
                                            {
                                                if (input[1] == '+')
                                                {
                                                    change = int.Parse(input.Substring(2).Trim());
                                                    //Add to val
                                                    newStats[sta.Key] = file.statBlock[sta.Key] + change;
                                                }
                                                if (input[1] == '-')
                                                {
                                                    change = int.Parse(input.Substring(2).Trim());
                                                    //Subtract from val
                                                    newStats[sta.Key] = file.statBlock[sta.Key] - change;
                                                }
                                            }
                                            else
                                            {
                                                //keep old val, so just get old data
                                                newStats[sta.Key] = file.statBlock[sta.Key];
                                            }   
                                        }
                                        Console.WriteLine(newStats[sta.Key]);
                                    }
                                    

                                    Console.WriteLine("Requirements: ");
                                    List<string> newReqs = new List<string>();
                                    //Loop thro the statblocks to display the existing data
                                    foreach (var req in file.requireList)
                                    {
                                        Console.WriteLine(req);
                                        Console.WriteLine("~ to keep, ~- to remove, ~+ to add to it, any val to replace");
                                        input = Console.ReadLine();
                                        if (input[0] != '~')
                                        {
                                            //override vals
                                            //file.statBlock[sta.Key] = int.Parse(input);
                                        }
                                        else
                                        {
                                            //merge new and old, action depends on input status
                                            int change = 0;
                                            if (input[1] == '+')
                                            {
                                                change = int.Parse(input.Substring(1));
                                                //Add to val
                                                //file.statBlock[sta.Key] = file.statBlock[sta.Key] + change;
                                            }
                                            if (input[1] == '-')
                                            {
                                                change = int.Parse(input.Substring(1));
                                                //Subtract from val
                                                //file.statBlock[sta.Key] = file.statBlock[sta.Key] - change;
                                            }
                                            else
                                            {
                                                //keep old val, so do nothing
                                            }
                                        }
                                    }
                                    Console.WriteLine("Unlocks: ");
                                    Dictionary<string, int> newUnlks = new Dictionary<string, int>();
                                    //Loop thro the statblocks to display the existing data
                                    foreach (var unl in file.unlockList)
                                    {
                                        Console.WriteLine(unl);
                                        Console.WriteLine("~ to keep, ~- to remove, ~+ to add to it, any val to replace");
                                        input = Console.ReadLine();
                                        if (input[0] != '~')
                                        {
                                            //override vals
                                            //file.statBlock[sta.Key] = int.Parse(input);
                                        }
                                        else
                                        {
                                            //merge new and old, action depends on input status
                                            int change = 0;
                                            if (input[1] == '+')
                                            {
                                                change = int.Parse(input.Substring(1));
                                                //Add to val
                                                //file.statBlock[sta.Key] = file.statBlock[sta.Key] + change;
                                            }
                                            if (input[1] == '-')
                                            {
                                                change = int.Parse(input.Substring(1));
                                                //Subtract from val
                                                //file.statBlock[sta.Key] = file.statBlock[sta.Key] - change;
                                            }
                                            else
                                            {
                                                //keep old val, so do nothing
                                            }


                                        }
                                    }



                                    foreach (var stat in file.statBlock) {
                                        Console.Write(stat.Key);
                                        Console.WriteLine(stat.Value);
                                    }


                                    Console.WriteLine("Saving Edits");
                                    //Write new data
                                    file.statBlock = newStats;
                                    file.requireList = newReqs;
                                    file.unlockList = newUnlks;

                                    foreach (var stat in file.statBlock)
                                    {
                                        Console.Write(stat.Key);
                                        Console.WriteLine(stat.Value);
                                    }


                                    binaryFormatter2.Serialize(fileStream,file);


                                    //Close stream, no further edits needed
                                    fileStream.Close();
                                }
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
                            //Console.WriteLine(file);
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
                        {
                            binaryFormatter.Serialize(fileStream, item);
                            fileStream.Close();
                        }

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