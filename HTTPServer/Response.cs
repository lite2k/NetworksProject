using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        string httpVer;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath, HTTPVersion version)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            // TODO: Create the response string
            this.code = code;
            switch (version)
            {
                case HTTPVersion.HTTP10:
                    httpVer = "HTTP/1.0";
                    break;
                case HTTPVersion.HTTP11:
                    httpVer = "HTTP/1.1";
                    break;
                case HTTPVersion.HTTP09:
                    httpVer = "HTTP/0.9";
                    break;
                default:
                    break;
            }
            string statusline = GetStatusLine(code, httpVer);
            headerLines.Add(contentType);
            headerLines.Add(content.Length.ToString());
            headerLines.Add(DateTime.Today.ToString());


            responseString = statusline + "\r\n";
            responseString += "Date: " + DateTime.Now.ToString() + "\r\n";
            responseString += "Server: " + Configuration.ServerType + "\r\n";
            responseString += "Content-Type: " + contentType + "\r\n";
            responseString += "Content-Length: " + content.Length.ToString() + "\r\n";

            if (redirectoinPath != string.Empty)
                responseString += "Location: " +"http://localhost:1000"+redirectoinPath + "\r\n";

            responseString += "" + "\r\n";
            responseString += content + "\r\n";
            LogResponse(responseString);
        }

        private string GetStatusLine(StatusCode code, string version)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            switch (code)
            {
                case StatusCode.OK:
                    statusLine = version + " 200 Ok.";
                    break;
                case StatusCode.NotFound:
                    statusLine = version + " 404 Not Found Error.";
                    break;
                case StatusCode.Redirect:
                    statusLine = version + " 301 Redirection Error";
                    break;
                case StatusCode.BadRequest:
                    statusLine = version + " 400 Bad Request Error.";
                    break;
                case StatusCode.InternalServerError:
                    statusLine = version + " 500 Internal Server Error.";
                    break;
                default:
                    break;
            }
            return statusLine;
        }
        private void LogResponse(string response)
        {
            FileStream fs = new FileStream("response.txt", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter writer = new StreamWriter(fs);
            writer.WriteLine(response);
            writer.Close();
            fs.Close();
        }
    }
}
