﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MessageServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Run(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        static void Run(string[] args)
        {
            TcpListener listener = new TcpListener(8080);
            listener.Start();

            while (true)
            {
                
                using (TcpClient client = listener.AcceptTcpClient())
                {
                    try
                    {
                        Read(client);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e);
                    }
                }
            }
        }

        static void Read(TcpClient client)
        {
            Database db = new Database();
            Console.WriteLine("Got connection: {0}", DateTime.Now);
            NetworkStream ns = client.GetStream();
            BinaryReader reader = new BinaryReader(ns);

            Message msg = new Message();
            // first read the Id
            msg.Id = reader.ReadInt32();

            // length of content in bytes.
            int length = reader.ReadInt32();

            // read the content bytes into the byte array.
            // recall that java side is writing two bytes for every character.
            byte[] contentArray = reader.ReadBytes(length);
            msg.Content = Encoding.UTF8.GetString(contentArray);
                        
            Console.WriteLine(msg.Id);
            Console.WriteLine(msg.Content);

            db.WriteToFile(msg.Id.ToString());
            db.WriteToFile(msg.Content);
            Console.WriteLine("Writing data...DONE");
            db.WriteToDatabase();

            client.Client.Shutdown(SocketShutdown.Both);
            ns.Close();
        }
    }
}

