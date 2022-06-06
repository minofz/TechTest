using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;


namespace TechinicalTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
               invalidArgs();
                return;
            }

            string vClearURL = "";

            if (args.Length == 1 && args[0] != "listall")
            {
                invalidArgs();
                return;
            }
            else
            {
                vClearURL = Regex.Match(args[1], @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*").ToString();

                if (vClearURL.Trim() == "")
                {
                    Console.WriteLine($"URL {args[1]} inválida!!");
                }
            }                       

            Console.WriteLine("loading database...");

            cDatabase.CreateDB();
            cDatabase.CreateTables();
                     

            switch (args[0])
            {
                case "load":
                    await LoadURL(vClearURL);
                    break;
                case "loadrecursive":
                    await LoadURL(vClearURL, true);
                    break;
                case "loadandlist":
                    await LoadURL(vClearURL);
                    ListURL(vClearURL);
                    break;
                case "loadandlistrecursive":
                    await LoadURL(vClearURL,true);
                    ListURL("");
                    break;
                case "list":
                    ListURL(args[1]);
                    break;
                case "listall":
                    ListURL("");
                    break;
                default:                        
                    invalidArgs();
                    break;

            }

            Console.ReadKey();
            
        }

        public static async Task LoadURL(string pURL, bool pRecursive =  false)
        {
            Console.WriteLine($"Loading links from {pURL}");
            using (cHttp vhttp = new cHttp())
            {
                QueryHMTL vQuery = await vhttp.queryURL(pURL);

                if (vQuery.ListURL != null && vQuery.ListURL.Trim() != "")
                {
                    string[] vList = vQuery.ListURL.Split("\n");
                    foreach(string vlink in vList)
                    {
                        if (vlink != "")
                        {
                            MappingURL vLinkMap = new MappingURL();
                            vLinkMap.Id = 0;
                            vLinkMap.BaseURL = pURL;
                            vLinkMap.LocatedURL = vlink;

                            cDatabase.Add(vLinkMap);
                        }
                    }

                    Console.WriteLine($"URL {pURL} loaded successfully");

                    if(pRecursive)
                    {                       
                        foreach (string vlink in vList)
                        {
                            string vdomain1 = Regex.Match(vlink, @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)").Groups["Domain"].Value;
                            string vdomain2 = Regex.Match(pURL, @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)").Groups["Domain"].Value;

                            if (vlink != pURL&& !RepeatedURL(vlink) && (vdomain1 == vdomain2) )
                            {
                                await LoadURL(vlink, true);
                            }
                        }
                    }
                }

            }

        }

        public static void ListURL(string pURL, bool pRecursive = false)
        {
            DataTable dtList = cDatabase.GetListMappingURL(pURL);

            if(dtList.Rows.Count < 1)
            {
                Console.WriteLine($"URL {pURL} with no results in the database");
                //Console.ReadKey();
                return;
            }
            Console.WriteLine($"List URL ...");
            foreach (DataRow row in dtList.Rows)
            {
                Console.WriteLine($"Id {row[0]} |BaseURL {row[1]} |URLFound {row[2]}");
            }

        }

        public static bool RepeatedURL(string pURL)
        {
            DataTable dtList = cDatabase.GetListMappingURL(pURL);

            return (dtList.Rows.Count > 0);

        }

        public static void invalidArgs()
        {
            Console.WriteLine("Invalid args");
            Console.WriteLine();
            Console.WriteLine("load [URL]                 - load links from a url in the database");
            Console.WriteLine();
            Console.WriteLine("loadrecursive [URL]        - load links from a url in the database and then recursively load the links from links founded ");
            Console.WriteLine();
            Console.WriteLine("loadandlist [URL]          - load links from a url in the database and list the links");
            Console.WriteLine();
            Console.WriteLine("loadandlistrecursive [URL] - load links from a url in the database and lists the links then recursively load and list the links from links founded");
            Console.WriteLine();
            Console.WriteLine("list [URL]                 - list links from the database of a url ");
            Console.WriteLine();
            Console.WriteLine("listall                    - list all links from the database ");
            Console.ReadKey();
        }
    }
}
