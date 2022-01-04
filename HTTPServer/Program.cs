﻿using System.IO;

namespace HTTPServer
{
    class Program
    {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server
        static void Main(string[] args)
        {
            
            CreateRedirectionRulesFile();
            Server server = new Server(1000, "redirectionRules.txt");
            server.StartServer();
        }

            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
        static void CreateRedirectionRulesFile()
        {

            FileStream redirectRules = new FileStream("redirectionRules.txt", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter writer = new StreamWriter(redirectRules);
            writer.WriteLine("\\aboutus.html,\\aboutus2.html");
            writer.Close();
            redirectRules.Close();
        }
         
    }
}
