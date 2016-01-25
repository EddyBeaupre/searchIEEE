using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace searchIEEE
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.Search != null)
                {
                    search(options.Search);
                }
                else if (options.Show == true)
                {
                    search(String.Empty);
                }
                else
                    Console.WriteLine(options.GetUsage());
            }
            Environment.Exit(0);
        }

        private static void search(String Needle)
        {
            ieeeCSV ieeeCSV = new ieeeCSV();
            Int64 Count = 0;

            foreach (String file in Directory.GetFiles(Path.GetDirectoryName(Application.UserAppDataPath), @"*.csv"))
            {
                ieeeCSV.Add(Path.GetFileName(file), String.Empty);
            }

            ieeeCSV.loadAll();

            List<IeeeRecord> results = ieeeCSV.search(Needle);

            Console.OutputEncoding = System.Text.Encoding.Unicode;

            if (results != null)
            {
                foreach (IeeeRecord record in results)
                {
                    Console.WriteLine(Count.ToString() + "," + record.Assignment.Trim() + "," + record.Registry.Trim() + ",\"" + record.OrganizationName.Trim() + "\",\"" + record.OrganizationAddress.Trim() + "\"");
                    Count++;
                }
            }
        }
    }

    class Options
    {
        [Option('s', "search", HelpText = "Search term.", MutuallyExclusiveSet = "Search")]
        public string Search { get; set; }

        [Option('a', "all", HelpText = "Show all entries.", MutuallyExclusiveSet = "Search")]
        public bool Show { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            String name = fvi.ProductName; ;
            String author = fvi.CompanyName;
            String version = fvi.FileVersion;
            String Copyright = fvi.LegalCopyright;
            String AppName = Path.GetFileName(fvi.FileName);

            var help = new HelpText
            {
                Heading = new HeadingInfo(name, fvi.FileVersion),
                Copyright = Copyright,
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Usage: " + AppName + " [-s term] [-a]");
            help.AddOptions(this);
            help.AddPostOptionsLine("The databases must be initialized by the GUI version.");
            return help;
        }
    }
}
