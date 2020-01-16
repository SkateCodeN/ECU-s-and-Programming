class Ecu_Block_Group
    {
        public string name;
        public List<string> data;
    }

class ECULogReader
    {
        //**********Private Globals for use in this Class
        readonly string filePath;
        readonly string[] headers;
        readonly string date;
        private readonly Ecu_Block_Group[] blocks;
        private readonly List<string> ecuLogs;
        //**********Public Globals to Be used by other Classes
        public string[] PHeaders => headers;
        public string PDate => date;
        public Ecu_Block_Group[] PBlocks => blocks;
        

        /// <summary>
        /// Constructor for the class. Call this only Externally to create Data Structure
        /// </summary>
        /// <param name="file">File Path to the Document (TXT|CSV)</param>
        public ECULogReader(string file)
        {
            filePath = file;
            ecuLogs = ReadAllLogs(filePath, ecuLogs);
            headers = GetHeaderName(ecuLogs[5]);
            date = GetDate(ecuLogs[0]);
            blocks = DistributeBlockData(ecuLogs,headers, blocks);
        }
        /// <summary>
        /// Given a File Path, read all lines and place into a List of strings
        /// </summary>
        /// <param name="file">The Path to the TXT or CSV file to read the ECU Log Data</param>
        /// <param name="ecuLogs">global variable of the class to be a temp holder of all lines in document</param>
        /// <returns></returns>
        private static List<string> ReadAllLogs(string file, List<string> ecuLogs)
    {
            // Load file into Stream Reader, while its not end of file,
            //  Insert line to List<string>
            ecuLogs = new List<string>();
            StreamReader readLogs = new StreamReader(file);
            while (readLogs.EndOfStream != true)
            {
                ecuLogs.Add(readLogs.ReadLine());
            }
            
            return ecuLogs;
        }
        /// <summary>
        /// Get the Date from the Document, written on the first line of document
        /// </summary>
        /// <param name="date">A string line containing (CSV) to be converted to Date</param>
        /// <returns></returns>
        private string GetDate(string date)
        {
            // split the line into csv array, take only the necessary fields.
            string finalDateFormat = "";
            string[] temp = date.Split(',');
            //  Murica format MM-DD-YYYY
            finalDateFormat = temp[0] + "-" + temp[2] + "-" + temp[1] + "-" + temp[3];
            return finalDateFormat;

        }
        /// <summary>
        /// The Headers here are the block names of log data...RPM,MAF etc
        /// </summary>
        /// <param name="headers">Same as the date this is a CSV string containing name of the ECU blocks</param>
        /// <returns></returns>
        private string[] GetHeaderName(string headers)
        {
            // same as the date, split the line string into csv array and return the array
            // We know that position [0] will be "" or if there is a Marker this will be a Number!
            // at the end n = length of string that has data. There will be [n+1....+4] empty "" elements on the Array by design
            string[] holder = headers.Split(',');

            return holder;
        }
        /// <summary>
        /// Take the headers that were in the header array and create the data structure that will hold
        /// Header names and a list/array of log data.
        /// </summary>
        /// <param name="logs">ECU Log data that corresponds to the headers</param>
        /// <param name="header">The ECU Block Name, ex RPM, MAF, Misfire etc</param>
        /// <param name="blocks">Global variable to be written back to the class(Data structure will contain {Name and Data of Blocks}</param>
        /// <returns></returns>
        private static Ecu_Block_Group[] DistributeBlockData(List<string> logs, string[] header, Ecu_Block_Group[] blocks)
        {
            // Create Class|Data Structure of ECU Block Group Elements based on the length of header names in the array
            // Note this can be cleaned to delete Null values common at the start [0] and towards the end [length -1,-2,-3 and -4]
            // This is not my doing, just how VCDS does it.
            blocks = new Ecu_Block_Group[header.Length];
            // Algo: Create an array of ECU Block Groups and place the name of itself based on its own value
            // Time Complexity O(N) N = length of header elements in header string array.
            for (int i = 0; i < header.Length; i++)
            {
                // Clean up the data, do not import to memory if null <- Needs to be corrected to ""
                if (header[i] != null)
                {
                    blocks[i] = new Ecu_Block_Group
                    {
                        name = header[i],
                        data = new List<string>()
                    };
                }
            }
            //  Algo that starts at a line 7, 
            //  splits the log line into an array
            //  Then on that temp array, we send over the log info to its parent class Block
            for(int a =7; a< logs.Count; a++)
            {
                string[] temp = logs[a].Split(',');
                for(int b = 0; b < temp.Length; b++)
                {
                    if (temp[b] !=null)
                    {
                        blocks[b].data.Add(temp[b]);
                    }
                }
            }
            return blocks;

        }

        
    }
