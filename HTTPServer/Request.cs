using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }
    /// <summary>
    /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
    /// </summary>
    /// <returns>True if parsing succeeds, false otherwise.</returns>
    //throw new NotImplementedException();
    //TODO: parse the receivedRequest using the \r\n delimeter   
    // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
    // Parse Request line
    // Validate blank line exists
    // Load header lines into HeaderLines dictionary
    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines = new Dictionary<string, string>();
        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        public HTTPVersion HttpVersion { get => httpVersion; }

        public Request(string requestString)
        {
            this.requestString = requestString;
        }

        public bool ParseRequest()
        {
            string[] Delimeter = new string[] { "\r\n" };
            requestLines = requestString.Split(Delimeter, StringSplitOptions.None);
            if (!ParseRequestLine())
                return false;

            
            
            return true;
        }


        private bool ParseRequestLine()
        {
            string[] line = requestLines[0].Split(' ');

            switch (line[0])
            {
                case "GET":
                    method = RequestMethod.GET;
                    break;
                case "HEAD":
                    return false;
                case "POST":
                    return false;
                default:
                    return false;
            }
            
            relativeURI = line[1];
            if (!ValidateIsURI(relativeURI))
                return false;

            switch (line[2])
            {
                case "HTTP/1.0":
                    httpVersion = HTTPVersion.HTTP10;
                    break;
                case "HTTP/1.1":
                    httpVersion = HTTPVersion.HTTP11;
                    break;
                default:
                    httpVersion = HTTPVersion.HTTP09;
                    break;
            }
           bool res = LoadHeaderLines();
            return res;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            if (!ValidateBlankLine())
                return false;

            for (int i = 1; requestLines[i] != ""; i++)
            {
                string line = requestLines[i];
                string[] lines = line.Split(new[] { ':' }, 2);
                headerLines[lines[0]] = lines[1];
            }
            headerLines["Content-Type"] = "text/html";
            if (!headerLines.ContainsKey("Host") && httpVersion == HTTPVersion.HTTP11)
                return false;
            return true;
        }

        private bool ValidateBlankLine()
        {

            if (!requestLines.Contains(""))
                return false;
            return true;
            
        }

       
    }
}
