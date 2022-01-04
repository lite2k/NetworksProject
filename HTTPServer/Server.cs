using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket
        public Server(int portNumber, string redirectionMatrixPath)
        {

            this.LoadRedirectionRules(redirectionMatrixPath);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverIp = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(serverIp);
        }

            // TODO: Listen to connections, with large backlog.
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            // TODO: accept connections and start thread for each accepted connection.
        public void StartServer()
        {
            serverSocket.Listen(100);
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                Thread newthread = new Thread(new ParameterizedThreadStart
                (HandleConnection));
                newthread.Start(clientSocket);

            }
        }

            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            // TODO: receive requests in while true until remote client closes the socket.
            // TODO: Receive request
            // TODO: break the while loop if receivedLen==0
            // TODO: Create a Request object using received request string
            // TODO: Call HandleRequest Method that returns the response
            // TODO: Send Response back to client
        public void HandleConnection(object obj)
        {

            Socket clientSocket = (Socket)obj;
            clientSocket.ReceiveTimeout = 0;
            int recLength;
            while (true)
            {
                try
                {
                    byte[] data = new byte[1024 * 1024];
                    recLength = clientSocket.Receive(data);

                    if (recLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSocket.RemoteEndPoint);
                        break;
                    }
                    Request request = new Request(Encoding.ASCII.GetString(data));
                    Response response = this.HandleRequest(request);
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));

                }


                    // TODO: log exception using Logger class
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }

                //TODO: check for bad request 
                //TODO: map the relativeURI in request to get the physical path of the resource.
                //TODO: check for redirect
                //TODO: check file exists
                //TODO: read the physical file
                // Create OK response
        Response HandleRequest(Request request)
        {
            
            string content;
            string defaultPageName = string.Empty;
            string redirectionPath = string.Empty;
            try
            {
                bool requestStatus = request.ParseRequest();
                string Uri;
                if (!requestStatus)
                {
                    defaultPageName = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    content = File.ReadAllText(defaultPageName);
                    Response response = new Response(StatusCode.BadRequest, request.HeaderLines["Content-Type"],
                         content, redirectionPath,request.HttpVersion);
                    return response;
                }
                else if (GetRedirectionPagePathIFExist(request.relativeURI) != string.Empty)
                {
                    // to display page
                    /*redirectionPath = GetRedirectionPagePathIFExist(request.relativeURI);
                    redirectionPath = redirectionPath;
                    Uri = Configuration.RootPath + redirectionPath;*/


                    defaultPageName = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    content = File.ReadAllText(defaultPageName);
                    Response response = new Response(StatusCode.Redirect, request.HeaderLines["Content-Type"],
                         content, redirectionPath, request.HttpVersion);
                    return response;
                }
                else if (!File.Exists(Configuration.RootPath + request.relativeURI))
                {
                    defaultPageName = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    content = File.ReadAllText(defaultPageName);
                    Response response = new Response(StatusCode.NotFound, request.HeaderLines["Content-Type"],
                         content, redirectionPath, request.HttpVersion);
                    return response;
                }
                else
                {
                    Uri = Configuration.RootPath + request.relativeURI;
                    content = File.ReadAllText(Uri);
                    Response response = new Response(StatusCode.OK, request.HeaderLines["Content-Type"],
                         content, redirectionPath, request.HttpVersion);
                    return response;
                }
                


            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
                Logger.LogException(ex);
                defaultPageName = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                content = File.ReadAllText(defaultPageName);
                Response response = new Response(StatusCode.InternalServerError, request.HeaderLines["Content-Type"],
                     content, redirectionPath, request.HttpVersion);
                return response;
            }
        }
     
        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                return Configuration.RedirectionRules[relativePath];

            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            try
            {
                // TODO: check if filepath not exist log exception using Logger class and return empty string
                // else read file and return its content
                string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
                return filePath;
            }

            catch (Exception ex)
            {

                Logger.LogException(ex);
                return string.Empty;
                //Environment.Exit(1);
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            Configuration.RedirectionRules = new Dictionary<string, string>();
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(file);
                while (reader.Peek() != -1)
                {
                    string[] rules = reader.ReadLine().Split(',');
                    Configuration.RedirectionRules.Add(rules[0], rules[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
