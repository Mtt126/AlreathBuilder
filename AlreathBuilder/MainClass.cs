using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AlreathBuilder
{

    class AlreathBuilder
    {
        //Uses different methods due to different structs for the edits so each method sets up the correct type instead of more work in single method
        //Each struct should be serializable and should be the same format that Alreath is expecting to convert it correctly

        //Structs might switch out for classes, it depends how I get them to setup and what I need them for
        // Classes would allow the data to be hidden and more self managed, with returning info for the data that can be shared
        [Serializable]
        public struct LocPoint
        {                       // Simple X,Y point in the worldspace, loc is name of worldspace which helps transitions between different worlds, 
                                //so 15,324 in one world can easily switch to 67,248 and not cause to much change, 
                                //the change is handled ingame by checking the transition areas and finding a global path and then a local path to the transition area
            public string loc;  //worldspace name
            public int x;       //3d space
            public int y;
            public int z;
        }
        [Serializable]
        public struct Requirement
        {
            // This is a requirement, so it check if condition is met, like
            // equip_sword returns true if correct
            // for now its just a string bool pair, bool changes on condition
            // might switch to class and return bool as a getReq method or use a class with req input to return a bool and the req values will change
            public string cond;
            public bool check;
        }
        [Serializable]
        public struct Job
        {
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

            public List<Requirement> requireList;
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



        [Serializable]
        public struct NPC
        {
            //NPC has Relations and AI packages as well as Loot tables, some of this may only be needed after runtime so some data may be purged from this list
            public string name;
            public Dictionary<string, int> Relations;    // Name,Val pairs for relationship to determine actions during runtime,
            public string AIType;                       // Used to 
            public List<string> factions;               // List of factions this NPC belongs too, used with another's relations to determine actions

            public List<LocPoint> PatrolPoints;         // List of PatrolPoints
            public bool Roamer;                         // Allow for AI to change locations
            //public List<string> Nearby;                 // Will become a list of nearby gameobjects to help store data, starts blank anyways and will gain data at runtime
            public int DetectRange;                     // Int to determine range of nearby objects
            public LocPoint HomeLoc;                      // Starting area to return to 
            public LocPoint CurLoc;                       // Might not need until runtime, but current location

            public List<string> LootTables;           //List of Loot Tables, name of lootTables to store loot tables separate to reuse ex. goblins loot


        }

        [Serializable]
        public struct Item
        {
            public string name;                     // Name of Item
            public string desc;                     // Desc of Item, contains more info than tooltip
            public string tooltip;                  // Tooltip on hoverover and quick check, expect jokes galore
            public bool equipable;                  //Can be equipped, not checking requirements to deem
            public List<Requirement> requireList;   //Requirements needed to use item
            public int amount;          // Amount of item, 1> otherwise deletes item from invent
            public int worth;           // Base worth for NPC sales
            public List<int> gearslots; // Itemslots to check for equip/unequip etc
            public string script;       // Name of script
            public string scriptdata;   // Data for script to run

        }

        [Serializable]
        public struct LootDrop
        {                       // All the info for a lootdrop
            public string loot;        // Name of item to include in table, as Items are in storage
            public int chance;         // Base chance of item
            public int amount;         // Amount as item data storage doesnt hold multiple, ex. 10 arrows
            public int rangeMax;      // Amount and rangeMax to allow for a chance amount of items, 2 to 4 arrows, using amount as Min val, range smaller than amount are ignored
            public Requirement req;    //Requirement in order for loot to be rolled, so quest stages and etc, this means it will be ignored if bool in req is false
        }

        [Serializable]
        public struct LootTable
        {
            public string name;    //name of loot table
            public List<LootDrop> Drops;
            // Normal which rolls for each item individually, ignore other options 
            //  1/2 3/5 4/5   -> 50% for item1, 60% for item2, 80% for item3
            //
            public bool weight;     // Weighted which shifts the chances based on each drop's chances, 
                                    // 1/2 3/5 4/5   -> 5/19 6/19 8/19   
                                    // 50% 60% 80%   -> 26%  32%  42%
            public int repeat;      // Repeat, num of times it goes thro the list, 
                                    // 5/19 6/19 8/19 
                                    //      roll 6  -> item 2
                                    //      roll 17 -> item 3
            public bool unique;     // Unique, rolls one item in group, items roll in order of list
                                    //  1/2 3/5 4/5       50% for item1                
                                    //                    failure = 60% for item2
                                    //                              failure = 80% for item3                   
                                    //                                        failure = no item     
            public bool dupe;       // allow for dupes on rolls, usually true, else once an item rolls, it will not get rerolled on repeat,
                                    // 5/19 6/19 8/19      roll 10 -> item 2
                                    //                     
                                    // 5/19 -(6/19)- 8/19      
                                    //                     roll 7 -> fails

            public bool recalc;     // Recalcs after roll, so weights shift when no dupe rolls 
                                    //  5/19 6/19 8/19      (12) roll item 2
                                    //  1/2      4/5
                                    //  50%      80%
                                    //  5/13     8/13       (7) roll item 3
                                    //  1/2
                                    //  50%
                                    //  5/5                 (2) roll item 1
            public bool noLuck;                // NoLuck, Luck doesnt change base probs, which otherwise tweaks chances 
        }


        [Serializable]
        public struct Reward
        {
            //Reward info to properly generate rewards, references name and type to correct find in storage
            public string rewardName;       //Finds name under type folder
            public string rewardType;       // ex. MasteryofMage under Title   
            //public string rewardData; //May not use, was gonna declare data if not found but thats gonna be at runtime
        }
        [Serializable]
        public struct Objective
        {
            // Objective Condition          //This flag needs to be triggered
            public bool Hidden;                      // Hide from UI
            public bool Last;             //Finish Quest once objective complete
            public bool Optional;         //Is this optional to complete
            public int Jumpto;            //Jumps to QstStage once obj done, allows for branches if certain objectives are done, 10-> 21 instead of usual 20
        }
        [Serializable]
        public struct QstStage
        {
            public int StageNum;            //Reference num
            public List<Objective> ObjList; //List of objectives till moving to next stage

            public int NextQstStage; // used to determine what stage to move to once all non optional objectives are finished , ex 10,20,40
        }
        [Serializable]
        public struct Quest
        {
            public string QID; // Quest Name
            public bool hidden; // Quest Hidden
            public int limit;  // Limit time till failure, this will countdown
            public string group; //Shows up with others under the same group, ex. Fright-Night quests, Starter quests, Mastery quests
            public int repeat; // Amount of times can repeat, counted vs completed, -1 for forever, 5 means 5 types to complete
            public int curStage;//this is the stage that is checked for conditions, starts with the first in list and moves thro, nextStage is based on QstStage info
            public string desc; // Desc of the quest for checking what it is
            public List<QstStage> StageList; // Main content will all the objectives
            public List<Reward> QstRwds; // List of rewards once quest is finished

        }

        private static void JobBuilder(string filepath, string command = "")
        {
            //JobBuilder
            //  --Stat Block
            //      -Breaks apart based on single entry
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
                            // I could setup defaults here to be overridden so the memory is atleast filled until replaced
                            name = input,
                            desc = "Un-Init Desc",
                            exp = 0,
                            lvl = 0,
                            statBlock = { },
                            requireList = { },
                            unlockList = { }
                        };

                        Console.WriteLine("Job Desc:");
                        job.desc = Console.ReadLine();

                        Console.WriteLine("Stat Block:");
                        Console.WriteLine("EX: CHA,5 STR,7 WIS,8 ");
                        input = Console.ReadLine().ToLower();

                        string[] inputstats = input.Split(' ');
                        int statcount = inputstats.Length;
                        Dictionary<string, int> stats = new Dictionary<string, int>();
                        // Split produces 1? as default, so need to check if blank
                        Console.WriteLine(statcount);
                        if (statcount != 1 && inputstats[0] != "")
                        {

                            foreach (string stat in inputstats)
                            {
                                string st = stat.Split(',')[0];
                                //might need to verify input vars to help parse
                                int val = int.Parse(stat.Split(',')[1]);
                                Console.WriteLine(st);
                                stats.Add(st, val);
                            }
                        }
                        job.statBlock = stats;

                        Console.WriteLine("Requirements:");
                        Console.WriteLine("EX: Skill_LVL_10/Fireball, Job_Unlock/Warden, Title_Equip/Mastery of Mage ");
                        Console.WriteLine("Comma for split items, _ for more sub reqs, / for nameof req");

                        input = Console.ReadLine().ToLower();

                        string[] requireLst = input.Split(',');
                        int reqcount = requireLst.Length;
                        // Split produces 1 as default, so its always something even if blank
                        // should also check if the split is empty or correct, so an empty isnt included in the dict
                        if (reqcount != 1 && requireLst[0] != "")
                        {

                            Requirement newreq = new Requirement
                            {
                                check = false,          //default to false, it will be updated on runtime as conditions are checked
                                cond = "equipped_sword" // will switch to actually be correct requirement
                            };
                            job.requireList.Add(newreq);


                        }

                        Console.WriteLine("UnlockList:");
                        Console.WriteLine("EX: Skill/Fireball,1 Job/Warden,15 Title/Mastery,100 ");
                        Console.WriteLine("Space for split items, comma between lvl,  / for typing");
                        input = Console.ReadLine().ToLower();

                        string[] inputUnlock = input.Split(' ');
                        int unlockcount = inputUnlock.Length;
                        Dictionary<string, int> unlocks = new Dictionary<string, int>();
                        // Split produces 1 as default, so its always something even if blank
                        // should also check if the split is empty or correct, so an empty isnt included in the dict
                        if (unlockcount != 1)
                        {
                            //Console.WriteLine(unlockcount);
                            foreach (string unlock in inputUnlock)
                            {
                                string ul = unlock.Split(',')[0];
                                int val = int.Parse(unlock.Split(',')[1]);
                            }
                        }
                        job.unlockList = unlocks;



                        string filePath = filepath + "\\" + job.name + ".job";
                        //Create file and write data
                        var binaryFormatter = new BinaryFormatter();
                        using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate))
                        {
                            binaryFormatter.Serialize(fileStream, job);
                            fileStream.Close();
                        }
                        break;

                    case ("E"):
                    case ("EDIT"):
                        bool editing = true;
                        while (editing)
                        {
                            Console.WriteLine("____EDIT MODE____");
                            Console.WriteLine("Input Name of Job to Edit");
                            //Find which file to edit
                            input = Console.ReadLine().ToLower();

                            //TODO

                            var editList = Directory.GetFiles(filepath, "*.job");
                            foreach (var fileStr in editList)
                            {
                                //This way we can reference new and old files at the same time by string concat or file
                                // editjob is the edited job which will be saved as file after done editing 
                                // fileStr is path of old file

                                if (filepath + "\\" + input + ".job" == fileStr)
                                {
                                    Console.WriteLine("Editing " + input);

                                    //Now to figure out what to edit by asking questions again, showing what the old val was
                                    // Edit ~ to use existing values
                                    // Maybe have a mode to append values like data so things can just be added and not replaced
                                    // ~+ to add or extend to existing, ~- to remove or subtract component
                                    //This way the things can be loaded in a tweaked without breaking older edits from scratch

                                    // file is the loaded job, the local saved file 

                                    using (var fileStream = File.Open(fileStr, FileMode.Open, FileAccess.ReadWrite))
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
                                            if (input[0] != '~')
                                            {
                                                //override vals
                                                newStats[sta.Key] = int.Parse(input);
                                            }
                                            else
                                            {
                                                //merge new and old, action depends on input status
                                                int change = 0;
                                                if (input.Length > 1)
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

                                        foreach (var stat in file.statBlock)
                                        {
                                            Console.Write(stat.Key);
                                            Console.WriteLine(stat.Value);
                                        }

                                        Console.WriteLine("Saving Edits");
                                        //Write new data
                                        file.statBlock = newStats;
                                        //file.requireList = newReqs;
                                        file.unlockList = newUnlks;

                                        foreach (var stat in file.statBlock)
                                        {
                                            Console.Write(stat.Key);
                                            Console.WriteLine(stat.Value);
                                        }

                                        binaryFormatter2.Serialize(fileStream, file);

                                        //Close stream, no further edits needed
                                        fileStream.Close();
                                    }
                                    break;
                                }
                            }
                            //Didnt find file in dir or finished
                            //Console.WriteLine("File not found in Cur Dir");
                            //ENDTODO

                            Console.WriteLine("Finished editing? [Y/N]");
                            if (Console.ReadLine() == "y")
                            {
                                editing = false;
                            }
                        }
                        break;
                    case ("L"):
                    case ("LIST"):
                        //Will Get list of all Items in DIR\Jobs
                        var listFiles = Directory.GetFiles(filepath, "*.job");
                        foreach (var file in listFiles)
                        {
                            Console.WriteLine(file);
                        }
                        break;
                    case ("DEL"):
                    case ("DELETE"):
                        // Deletes the file
                        Console.WriteLine("Input Name of Item to Delete");
                        input = Console.ReadLine().ToLower();
                        //Checks in DIR\Items for the inputed name
                        var delList = Directory.GetFiles(filepath, "*.job");
                        foreach (var file in delList)
                        {
                            //Console.WriteLine(file);
                            if (filepath + "\\" + input + ".jb" == file)
                            {
                                //DEL ITEM
                                Console.WriteLine("Deleted " + input);
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
        private static void ItemBuilder(string filepath, string command = "")
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

                        Item item = new Item();

                        string itemName = input;
                        item.name = itemName;
                        Console.WriteLine("Item Desc:");
                        input = Console.ReadLine();
                        item.desc = input;
                        item.amount = 1;
                        Console.WriteLine("Item Base Worth:");
                        input = Console.ReadLine();
                        int.TryParse(input, out int res);
                        item.worth = res;
                        Console.WriteLine("Item Script:");
                        input = Console.ReadLine();
                        item.script = input;
                        Console.WriteLine("Script Data:");
                        input = Console.ReadLine();
                        item.scriptdata = input;

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
                        bool editing = true;
                        while (editing) {

                            Console.WriteLine("Input Name of Item to Edit");
                            //
                            input = Console.ReadLine();
                            //TODO

                            //Find item to edit

                            //Get what detail to edit
                            Console.WriteLine("Name,desc,worth,script ?");
                            switch (Console.ReadLine().ToLower()) {
                                case ("name"):

                                    break;
                                case ("desc"):

                                    break;
                                case ("worth"):
                                    break;
                                case ("script"):
                                    break;
                                case ("data"):
                                    break;
                                default:
                                    break;

                            }
                            Console.WriteLine("Done Editing? (Y/N)");
                            input = Console.ReadLine();
                            if (input == "Y") {
                                editing = false;
                            }
                        }
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
        private static void LootBuilder(string filepath, string command = "")
        {
            //LootBuilder
            //  --LootTable vars
            //      
            //  -- Items
            //      -ItemBuilder if needed
            //      -Drop Chance
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("----Loot Builder----");
                Console.WriteLine("Choose New Loot Table (N), Edit (E), List (L), Delete Old (DEL), Help (?), Quit (Q)");
                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case ("N"):
                    case ("NEW"):
                        //New item 
                        Console.WriteLine("Input Name of Loot Table to Create (converts to lower)");
                        input = Console.ReadLine().ToLower();

                        LootTable LT;
                        LT.name = input;




                        string filePath = filepath + "\\" + LT.name + ".lot";
                        //Create file and write data
                        var binaryFormatter = new BinaryFormatter();
                        using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate))
                        {
                            //binaryFormatter.Serialize(fileStream, LT);
                            fileStream.Close();
                        }
                        break;

                    case ("E"):
                    case ("EDIT"):
                        bool editing = true;
                        while (editing)
                        {
                            Console.WriteLine("____EDIT MODE____");
                            Console.WriteLine("Input Name of Loot Table to Edit");
                            //Find which file to edit
                            input = Console.ReadLine().ToLower();

                            //TODO

                            var editList = Directory.GetFiles(filepath, "*.lot");
                            foreach (var fileStr in editList)
                            {
                                //This way we can reference new and old files at the same time by string concat or file
                                // editjob is the edited job which will be saved as file after done editing 
                                // fileStr is path of old file

                                if (filepath + "\\" + input + ".lot" == fileStr)
                                {
                                    Console.WriteLine("Editing " + input);

                                    //Now to figure out what to edit by asking questions again, showing what the old val was
                                    // Edit ~ to use existing values
                                    // Maybe have a mode to append values like data so things can just be added and not replaced
                                    // ~+ to add or extend to existing, ~- to remove or subtract component
                                    //This way the things can be loaded in a tweaked without breaking older edits from scratch

                                    // file is the loaded job, the local saved file 

                                    using (var fileStream = File.Open(fileStr, FileMode.Open, FileAccess.ReadWrite))
                                    {
                                        var binaryFormatter2 = new BinaryFormatter();
                                        LootTable file = (LootTable)binaryFormatter2.Deserialize(fileStream);



                                        Console.WriteLine("Saving Edits");
                                        //Write new data




                                        binaryFormatter2.Serialize(fileStream, file);

                                        //Close stream, no further edits needed
                                        fileStream.Close();
                                    }
                                    break;
                                }
                            }
                            //Didnt find file in dir or finished
                            //Console.WriteLine("File not found in Cur Dir");
                            //ENDTODO

                            Console.WriteLine("Finished editing? [Y/N]");
                            if (Console.ReadLine() == "y")
                            {
                                editing = false;
                            }
                        }
                        break;
                    case ("L"):
                    case ("LIST"):
                        //Will Get list of all Items in DIR\Loot
                        var listFiles = Directory.GetFiles(filepath, "*.lot");
                        foreach (var file in listFiles)
                        {
                            Console.WriteLine(file);
                        }
                        break;
                    case ("DEL"):
                    case ("DELETE"):
                        // Deletes the file
                        Console.WriteLine("Input Name of Item to Delete");
                        input = Console.ReadLine().ToLower();
                        //Checks in DIR\Items for the inputed name
                        var delList = Directory.GetFiles(filepath, "*.lot");
                        foreach (var file in delList)
                        {
                            //Console.WriteLine(file);
                            if (filepath + "\\" + input + ".lot" == file)
                            {
                                //DEL ITEM
                                Console.WriteLine("Deleted " + input);
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
        private static void NPCBuilder(string filepath, string command = "")
        {
            //NPC Builder
            //  --NPC Base Info
            //      - Name
            //      - Class/Job
            //      - AI Type info
            //  --NPC Stats
            //  --NPC Abilities
            //      -Townsfolk
            //          -Trainer    +Learn Skills /Classes
            //          -Trader     +Buy/sell Items
            //          -Craftsmen  +Produce/Maintain Gear
            //  --NPC Loot Table
            //      -List current LootTables
            //      -New LootTable or use one in list
            //      -Will run LootBuilder if needed
            //          -Will run ItemBuilder if needed

            bool loop = true;
            while (loop)
            {

                Console.WriteLine("----NPC Builder----");
                Console.WriteLine("Choose New NPC (N), Edit (E), List (L), Delete Old (DEL), Help (?), Quit (Q)");
                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case ("N"):
                        //New item 
                        Console.WriteLine("Input Name of NPC to Create");
                        input = Console.ReadLine();

                        NPC npc = new NPC
                        {
                            name = input
                        };



                        string filePath = filepath + "\\" + npc.name + ".mob";
                        //Create Item
                        var binaryFormatter = new BinaryFormatter();
                        using (var fileStream = File.Create(filePath))
                            binaryFormatter.Serialize(fileStream, npc);
                        break;
                    case ("E"):
                    case ("EDIT"):
                        Console.WriteLine("Input Name of NPC to Edit");
                        //
                        input = Console.ReadLine();
                        //TODO
                        break;
                    case ("L"):
                    case ("LIST"):
                        //Will Get list of all files in DIR\NPC
                        var listFiles = Directory.GetFiles(filepath, "*.mob");
                        foreach (var file in listFiles)
                        {
                            Console.WriteLine(file);
                        }
                        break;
                    case ("DEL"):
                    case ("DELETE"):
                        Console.WriteLine("Input Name of NPC to Delete");
                        input = Console.ReadLine();
                        //Checks in DIR\NPC for the inputed name
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
            }
        }
        private static void QuestBuilder(string filepath, string command = "")
        {
            //Quest Builder
            //  --Quest Type 
            //      -Hidden, limited, Repeatable
            //  --Quest Stages
            //      -Loop til finished
            //      -Stage Objectives
            //      -Stage Desc
            //      -Stage Rewards
            //      -Next Stage
            //
            //  --Quest Rewards
            bool loop = true;
            while (loop)
            {

                Console.WriteLine("----Quest Builder----");
                Console.WriteLine("Choose New Item (N), Edit (E), List (L), Delete Old (DEL), Quit (Q)");
                string input = Console.ReadLine();
                switch (input.ToUpper())
                {
                    case ("N"):
                    case ("NEW"):
                        //New Quest
                        Console.WriteLine("Input Name of Quest to Create");
                        input = Console.ReadLine();

                        Quest qst = new Quest();
                        qst.QID = input;

                        string filePath = filepath + "\\" + input + ".qst";

                        //Need to check if input already exists, 
                        var binaryFormatter = new BinaryFormatter();
                        using (var fileStream = File.Create(filePath))
                            binaryFormatter.Serialize(fileStream, qst);


                        //Time to set up all the info and write the data again
                        Console.WriteLine("Quest Desc");
                        qst.desc = Console.ReadLine();
                        Console.WriteLine("Quest Hidden?");
                        switch (Console.ReadLine().ToLower())
                        {
                            case ("true"):
                            case ("y"):
                                qst.hidden = true;
                                break;
                            default:
                                qst.hidden = false;
                                break;

                        }
                        Console.WriteLine("Limited Time? (-1 to n)");

                        int val;
                        val = int.Parse(Console.ReadLine());
                        qst.limit = val;

                        Console.WriteLine("Repeatable? (-1 to n)");


                        val = int.Parse(Console.ReadLine());
                        qst.limit = val;

                        Console.WriteLine("Quest Group?");
                        qst.group = Console.ReadLine().ToLower();


                        bool staging = true;
                        while (staging)
                        {
                            //List of objectives
                            //Reward for stage complete
                            Console.WriteLine("____Staging___");






                            Console.WriteLine("Continue Staging");
                            switch (Console.ReadLine().ToLower())
                            {
                                case ("yes"):
                                case ("y"):
                                    break;
                                default:
                                    staging = false;
                                    break;

                            }

                        }


                        break;
                    case ("E"):
                    case ("EDIT"):
                        bool editing = true;
                        while (editing)
                        {
                            Console.WriteLine("____EDIT MODE____");
                            Console.WriteLine("Input Name of Quest to Edit");
                            //Find which file to edit
                            input = Console.ReadLine().ToLower();

                            //TODO

                            var editList = Directory.GetFiles(filepath, "*.qst");
                            foreach (var fileStr in editList)
                            {
                                //This way we can reference new and old files at the same time by string concat or file
                                // editjob is the edited job which will be saved as file after done editing 
                                // fileStr is path of old file

                                if (filepath + "\\" + input + ".qst" == fileStr)
                                {
                                    Console.WriteLine("Editing " + input);

                                    //Now to figure out what to edit by asking questions again, showing what the old val was
                                    // Edit ~ to use existing values
                                    // Maybe have a mode to append values like data so things can just be added and not replaced
                                    // ~+ to add or extend to existing, ~- to remove or subtract component
                                    //This way the things can be loaded in a tweaked without breaking older edits from scratch

                                    // file is the loaded job, the local saved file 

                                    using (var fileStream = File.Open(fileStr, FileMode.Open, FileAccess.ReadWrite))
                                    {
                                        var binaryFormatter2 = new BinaryFormatter();
                                        Quest file = (Quest)binaryFormatter2.Deserialize(fileStream);


                                        //TODO


                                        Console.WriteLine("StatBlock: ");
                                        Dictionary<string, int> newStats = new Dictionary<string, int>();
                                        //Loop thro the queststages to display the existing data
                                        foreach (var sta in file.StageList)
                                        {
                                            Console.WriteLine(sta);
                                            Console.WriteLine("~ to keep, ~- to remove, ~+ to add to it, any val to replace");
                                            input = Console.ReadLine();
                                            if (input[0] != '~')
                                            {
                                                //override vals

                                            }

                                        }






                                        Console.WriteLine("Saving Edits");
                                        //Write new data


                                        binaryFormatter2.Serialize(fileStream, file);

                                        //Close stream, no further edits needed
                                        fileStream.Close();
                                    }
                                    break;
                                }
                            }
                            //Didnt find file in dir or finished
                            //Console.WriteLine("File not found in Cur Dir");
                            //ENDTODO

                            Console.WriteLine("Finished editing? [Y/N]");
                            if (Console.ReadLine() == "y")
                            {
                                editing = false;
                            }
                        }
                        break;
                    case ("L"):
                    case ("LIST"):
                        //Will Get list of all Items in DIR\Quest
                        var listFiles = Directory.GetFiles(filepath, "*.qst");
                        foreach (var file in listFiles)
                        {
                            Console.WriteLine(file);
                        }
                        break;
                    case ("DEL"):
                        //
                        Console.WriteLine("Input Name of Quest to Delete");
                        input = Console.ReadLine();
                        //Checks in DIR\Quest for the inputed name
                        var delList = Directory.GetFiles(filepath, "*.qst");
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


        static void Main(string[] args)
        {
            //Select filepath for storage

            Console.WriteLine("Server Storage Dir: ");
            string path = Console.ReadLine();

            if (path.Trim() == "")
            {
                //blank path
                // use curDir
                path = Directory.GetCurrentDirectory();
            }

            string curDir = path;

            //Enter just sticks it in C drive for now, not where exe is.


            //Start loop
            bool loop = true;
            while (loop)
            {
                //print out choices to build
                //
                //Will eventually allow for commandline with alreathbuilder.exe J N 
                // to jump to sub processes, using command string input
                //
                Console.WriteLine("Choose JobBuilder (J), ItemBuilder (I), LootBuilder (L), NPCBuilder (N), QuestBuilder (Q)");
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
                    case "L":
                        Directory.CreateDirectory(curDir + "\\Loot");
                        LootBuilder(curDir + "\\Loot");
                        break;
                    case "N":
                        Directory.CreateDirectory(curDir + "\\NPC");
                        NPCBuilder(curDir + "\\NPC");
                        break;
                    case "Q":
                        Directory.CreateDirectory(curDir + "\\Quests");
                        QuestBuilder(curDir + "\\Quests");
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