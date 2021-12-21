using System;
using System.Collections.Generic;
using System.Linq;
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

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverIp = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(serverIp);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                //TODO: accept connections and start thread for each accepted connection.
                Thread newthread = new Thread(new ParameterizedThreadStart
                (HandleConnection));
                newthread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket clientSocket = (Socket)obj;
            clientSocket.ReceiveTimeout = 0;
            int recLength;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    byte[] data = new byte[1024 * 1024];
                    // TODO: Receive request
                    recLength = clientSocket.Receive(data);

                    // TODO: break the while loop if receivedLen==0

                    // TODO: Create a Request object using received request string

                    // TODO: Call HandleRequest Method that returns the response

                    // TODO: Send Response back to client
                    if (recLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSocket.RemoteEndPoint);
                        break;
                    }
                    Request request = new Request(Encoding.ASCII.GetString(data));
                    Response response = this.HandleRequest(request);

                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));

                }


                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content;
            string defaultPageName = string.Empty;
            Response response;
            string redirectionPath = string.Empty;
            try
            {
                //TODO: check for bad request 

                //TODO: map the relativeURI in request to get the physical path of the resource.

                //TODO: check for redirect

                //TODO: check file exists

                //TODO: read the physical file

                // Create OK response
                if (!request.ParseRequest())
                {

                    defaultPageName = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    content = File.ReadAllText(defaultPageName);
                    response = new Response(StatusCode.BadRequest, request.HeaderLines["Content-Type"],
                         content, redirectionPath,request.HttpVersion);
                }
                else if (GetRedirectionPagePathIFExist(request.relativeURI) != string.Empty)
                {
                    redirectionPath = GetRedirectionPagePathIFExist(request.relativeURI);
                    defaultPageName = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    content = File.ReadAllText(defaultPageName);
                    response = new Response(StatusCode.Redirect, request.HeaderLines["Content-Type"],
                         content, redirectionPath, request.HttpVersion);
                }
                else if (!File.Exists(request.relativeURI))
                {
                    defaultPageName = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    content = File.ReadAllText(defaultPageName);
                    response = new Response(StatusCode.NotFound, request.HeaderLines["Content-Type"],
                         content, redirectionPath, request.HttpVersion);
                }
                else
                {
                    content = File.ReadAllText(request.relativeURI);
                    response = new Response(StatusCode.OK, request.HeaderLines["Content-Type"],
                         content, redirectionPath, request.HttpVersion);
                }
                return response;


            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
                Logger.LogException(ex);
                defaultPageName = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                content = File.ReadAllText(defaultPageName);
                response = new Response(StatusCode.InternalServerError, request.HeaderLines["Content-Type"],
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
                string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
                // TODO: check if filepath not exist log exception using Logger class and return empty string


                // else read file and return its content
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
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(file);
                while (reader.Peek() != -1)
                {
                    string[] rules = reader.ReadLine().Split(',');
                    Configuration.RedirectionRules[rules[0]] = rules[1];
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
