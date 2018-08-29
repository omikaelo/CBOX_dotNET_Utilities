using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using IniParser;
using IniParser.Model;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.NetworkInformation;

namespace C_Box
{
    public static class Utilities
    {
        public static bool Ping(string ipAddress)
        {
            if (ipAddress.Length == 0)
                return false;
            Ping ping;
            PingReply reply;
            ping = new Ping();
            try
            {
                reply = ping.Send(ipAddress);
                if (reply.Status != IPStatus.Success)
                {
                    ping.Dispose();
                    return false;
                }
                ping.Dispose();
                return true;
            }
            catch (PingException e)
            {
                if (ping != null)
                    ping.Dispose();
                throw e;
            }
            catch (InvalidOperationException e)
            {
                if (ping != null)
                    ping.Dispose();
                throw e;
            }
        }

        public static string[] GetSectionFromConfigurationFile(string file, string section)
        {
            if (!File.Exists(file))
                return null;
            if (string.IsNullOrEmpty(section))
                return null;
            int counter = 0;
            FileIniDataParser parser;
            IniData data;
            KeyData[] keys;
            string[] sectionKeys;
            parser = new FileIniDataParser();
            data = parser.ReadFile(file);
            keys = data[section].ToArray();
            sectionKeys = new string[keys.Length];
            foreach (KeyData key in keys)
            {
                sectionKeys[counter] = key.Value;
                counter++;
            }
            return sectionKeys;
        }

        public static string GetKeyFromConfigurationFile(string file, string section, string key)
        {
            if (!File.Exists(file))
                return "";
            if (string.IsNullOrEmpty(section))
                return "";
            if (string.IsNullOrEmpty(key))
                return "";
            FileIniDataParser parser;
            IniData data;
            parser = new FileIniDataParser();
            data = parser.ReadFile(file);
            return data[section][key];
        }

        public static string GetKLCalGradient(string data)
        {
            if (string.IsNullOrEmpty(data))
                return "";
            if (data.Length < 23)
                return "";
            return data.Substring(0, 11);
        }

        public static string GetKLCalOffset(string data)
        {
            if (string.IsNullOrEmpty(data))
                return "";
            if (data.Length < 23)
                return "";
            return data.Substring(12);
        }

        public static int ConvertHexStringWithSpaceToNumber(string data)
        {
            string hexNum = "";
            if (string.IsNullOrEmpty(data))
                return -1;
            hexNum = data.Replace(" ", "");
            if (hexNum.Length % 2 != 0)
                return -1;
            return Convert.ToInt32(hexNum, 16);
        }

        public static string ConvertInt32ToHexStringWithSpace(uint value)
        {
            string hex = Convert.ToString(value, 16);
            string aux = "";
            int residue = hex.Length % 8;
            if (residue != 0)
                hex = hex.PadLeft(hex.Length + residue, '0');
            for (int i = 0; i < hex.Length / 2; i += 2)
            {
                aux = hex.Insert(i + 2, " ").Trim();
            }
            return aux;
        }

        public static string ConvertInt16ToHexStringWithSpace(ushort value)
        {

            string hex = Convert.ToString(value, 16);
            string aux = "";
            int residue = hex.Length % 2;
            if (residue != 0)
                hex = hex.PadLeft(hex.Length + residue, '0');
            for (int i = 0; i < hex.Length / 2; i += 2)
            {
                aux = hex.Insert(i + 2, " ").Trim();
            }
            return aux;
        }

        public static string ConvertHexStringWithSpecifierToHexStringWithSpace(string hex)
        {
            if (string.IsNullOrEmpty(hex) || string.IsNullOrWhiteSpace(hex))
                return "";
            byte[] aux = new byte[] { };
            if (hex.StartsWith("0x"))
            {
                aux = BitConverter.GetBytes(Convert.ToInt32(hex, 16));
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(aux);
                return BitConverter.ToString(aux).Replace('-', ' ');
            }
            return "";
        }

        public static string ConvertASCIIStringToHex(string data)
        {
            return BitConverter.ToString(Encoding.ASCII.GetBytes(data)).Replace('-', ' ');
        }

        public static string ReverseHexString(string data)
        {
            string[] values = null;
            if (data.Length == 0)
                return "";
            values = data.Split(' ').Reverse().ToArray();
            return string.Join(" ", values);

        }

        public static string[] ExtractGPSMessages(string data)
        {
            if (data.Length == 0)
                return new string[] { };
            return data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.StartsWith("$GP") && x.Contains("*")).ToArray();
        }

        public static string[] SortGPSMessagesBySNRDesc(string[] messages)
        {
            string[] ordered = null;
            if (messages.Length == 0)
                return new string[] { };
            ordered = messages.OrderBy(x => Convert.ToInt32(x.Split(',').Last().Remove(x.Split(',').Last().IndexOf("*")))).ToArray();
            return ordered;
        }

        public static string[] SortGLONASSMessagesBySNRDesc(string[] messages)
        {
            string[] ordered = null;
            if (messages.Length == 0)
                return new string[] { };
            ordered = messages.OrderBy(x => Convert.ToInt32(x.Split(',').Last().Remove(x.Split(',').Last().IndexOf("*")))).ToArray();
            return ordered;
        }

        public static string[] ExtractGLONASSMessages(string data)
        {
            if (data.Length == 0)
                return new string[] { };
            return data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.StartsWith("$GL")).ToArray();
        }

        public static int ExtractIDFromGPSMessage(string message)
        {
            string id = "";
            if (message.Length == 0)
                return -1;
            if (!message.StartsWith("$GP") || !message.Contains("*"))
                return -1;
            id = message.Split(',')[4];
            return Convert.ToInt32(id);
        }

        public static int ExtractSNRFromGPSMessage(string message)
        {
            if (message.Length == 0)
                return -1;
            if (!message.StartsWith("$GP") || !message.Contains("*"))
                return -1;
            string snr = message.Split(',').Last();
            snr = snr.Remove(snr.IndexOf("*"));
            return Convert.ToInt32(snr);
        }

        public static int ExtractSNRFromGLONASSMessage(string message)
        {
            if (message.Length == 0)
                return -1;
            if (!message.StartsWith("$GL") || !message.Contains("*"))
                return -1;
            string snr = message.Split(',').Last();
            snr = snr.Remove(snr.IndexOf("*"));
            return Convert.ToInt32(snr);
        }

        public static string ConvertHexASCIIToChar(string data)
        {
            if (data.Length == 0)
                return "";
            data = data.Replace("  ", " ").Trim();
            string[] values = data.Split(' ');
            string result = "";
            foreach (string s in values)
            {
                result += Convert.ToChar(Convert.ToUInt32(s, 16));
            }
            return result;
        }

        public static string[] Get8ByteChunksFromFWPath(string data)
        {
            string[] chunks = new string[] { };
            int chunksCounter;
            int res = 0;
            if (string.IsNullOrEmpty(data) || string.IsNullOrWhiteSpace(data))
                return null;
            res = ConvertHexStringWithSpaceToBytes(data).Length % 7;
            if (res != 0)
            {
                res = 7 - res;
                for (int i = 0; i < res; i++)
                {
                    data = data + " 00";
                }
                if (data.Replace(" ", "").Length % 2 != 0)
                    return null;
                chunksCounter = (data.Replace(" ", "").Length / 2) / 7;
                chunks = new string[chunksCounter];
                for (int i = 0; i < chunksCounter; i++)
                {
                    chunks[i] = i.ToString("X2") + " " + data.Substring(0, 20);
                    data = data.Remove(0, 20).Trim();
                }
            }
            else
            {
                if (data.Replace(" ", "").Length % 2 != 0)
                    return null;
                chunksCounter = (data.Replace(" ", "").Length / 2) / 7;
                chunks = new string[chunksCounter];
                for (int i = 0; i < chunksCounter; i++)
                {
                    chunks[i] = i.ToString("X2") + " " + data.Substring(0, 20);
                    data = data.Remove(0, 20).Trim();
                }
            }
            return chunks;
        }

        public static string ConvertDateToBDCFormat()
        {
            string year = DateTime.Now.ToString("yy");
            string day = DateTime.Now.ToString("dd");
            string month = DateTime.Now.ToString("MM");
            return BitConverter.ToString(new byte[] { Convert.ToByte(day, 16), Convert.ToByte(month, 16), Convert.ToByte(year, 16) }).Replace('-', ' ');
        }

        public static bool WriteKeysToFile(string folderPath, string fileName, string Startkeyverificationkonstante, string IKA_SCK, string ECU_Master_KEY, string Debug_CC, string Status_KS)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;
            if (!Directory.Exists(folderPath))
                return false;
            fileName += DateTime.Now.ToString("_ddd_MMM_HH_mm_ss_yyyy", CultureInfo.CreateSpecificCulture("en-US")) + ".txt";
            string filePath = Path.Combine(folderPath, fileName);
            try
            {
                using (StreamWriter stream = File.CreateText(filePath))
                {
                    stream.Write("FAZIT: {0}\n\n", fileName.Remove(fileName.IndexOf('_')));
                    stream.Write("Startkeyverificationkonstante: {0}\n\n", Startkeyverificationkonstante);
                    stream.Write("FAZITdaten:\n\tIKA_SCK: {0}\n", IKA_SCK);
                    stream.Write("Debugdaten:\n\tECU MASTER KEY: {0}\n\tStartkeycheckkonstante(DEBUG_CC): {1}\n\n", ECU_Master_KEY, Debug_CC);
                    stream.Write("Status KS: {0}", Status_KS);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool ExtractLSBAndMSBFromPWD(string pwd, out string lsb, out string msb)
        {
            lsb = msb = "";
            if (string.IsNullOrEmpty(pwd))
                return false;
            if (pwd.Length < 14)
                return false;
            if (pwd.Replace(" ", "").Length % 2 != 0)
                return false;
            lsb = pwd.Replace(" ", "").Substring(6);
            msb = "00" + pwd.Replace(" ", "").Substring(0, 6);
            return true;
        }

        public static bool WriteIPC_Key_ToFile(string folder, string data)
        {
            if (string.IsNullOrEmpty(data))
                return false;
            if (data.Length < 46)
                return false;
            if (data.Replace(" ", "").Length % 2 != 0)
                return false;
            if (!Directory.Exists(folder))
                return false;
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Create(Path.Combine(folder, "key.key"))))
                {
                    writer.Write(ConvertHexStringWithSpaceToBytes(data));
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static string ExtractSerialNumber(string data)
        {

            string sn = "";
            if (string.IsNullOrEmpty(data))
                return "";
            if (!data.Contains(","))
                return "";
            sn = data.Split(',').Where(x => x.Length == 18).FirstOrDefault();
            return sn;
        }

        public static string ExtractPCUIInterfacePort(string data)
        {
            if (data.Length == 0)
                return "";
            string port = data.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries).ElementAt(1).Trim();
            port = port.Remove(0, port.IndexOf("(") + 1).Replace(")", "");
            return port;
        }

        public static string ExtractSerialCPort(string data)
        {
            if (data.Length == 0)
                return "";
            string port = data.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries).ElementAt(4).Trim();
            port = port.Remove(0, port.IndexOf("(") + 1).Replace(")", "");
            return port;
        }

        public static string ExtractSerialBPort(string data)
        {
            if (data.Length == 0)
                return "";
            string port = data.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries).ElementAt(7).Trim();
            port = port.Remove(0, port.IndexOf("(") + 1).Replace(")", "");
            return port;
        }

        public static string Extract3GApplicationPort(string data)
        {
            if (data.Length == 0)
                return "";
            string port = data.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries).ElementAt(10).Trim();
            port = port.Remove(0, port.IndexOf("(") + 1).Replace(")", "");
            return port;
        }

        public static void WriteToLog(string path, string data)
        {
            File.WriteAllText(path, data);
        }

        public static string ExtractApplicationInterfacePort(string data)
        {
            if (data.Length == 0)
                return null;
            string port = data.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries).ElementAt(11).Trim();
            port = port.Remove(0, port.IndexOf("(") + 1).Replace(")", "");
            return port;
        }

        public static string ExtractFAZITString(string data)
        {
            string fazit = "";
            if (string.IsNullOrEmpty(data))
                return "";
            if (!data.Contains(","))
                return "";
            fazit = data.Split(',').Where(x => x.Contains("YSN")).FirstOrDefault();
            if (!string.IsNullOrEmpty(fazit))
                fazit = fazit.Split('#')[3].Replace("*", "").Replace("=", "").Substring(3).Trim();
            return fazit;
        }

        public static string GetEnvironmentVariable(string variable)
        {
            if (string.IsNullOrEmpty(variable))
                return "";
            return Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);
        }

        public static void SetEnvironmentVariable(string variable, string value)
        {
            if (string.IsNullOrEmpty(variable) || string.IsNullOrEmpty(value))
                return;
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.User);
        }

        public static bool SearchProcessByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return Process.GetProcesses().Where(x => x.ProcessName == name).FirstOrDefault() != null;
        }

        public static bool SearchProcessByPID(int pid)
        {
            if (pid < 0)
                return false;
            return Process.GetProcesses().Where(x => x.Id == pid).FirstOrDefault() != null;
        }

        public static int GetProcessPID(string name)
        {
            if (string.IsNullOrEmpty(name))
                return -1;
            return Process.GetProcesses().Where(x => x.ProcessName == name).FirstOrDefault().Id;
        }

        public static bool LaunchProcess(string name, string arguments, int timeout = 0)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            Process process = new Process();
            try
            {
                process.StartInfo.FileName = name;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                process.Start();
                bool pName = SearchProcessByName(name);
                if (timeout > 0)
                {
                    bool result = process.WaitForExit(timeout) & pName;
                    if (!result)
                        KillProcessByPID(process.Id);
                    return result;
                }
                return pName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: {0}", ex.Message);
                throw ex;
            }
        }

        public static int LaunchShell(string name, string arguments, out string stdOutput, out string stdError, int timeout, bool runAsAdmin = false, string user = "", string password = "")
        {
            Process process = new Process();
            int code = 0;
            System.Security.SecureString pass = null;
            if (runAsAdmin)
            {
                if (user.Length == 0)
                {
                    code = 10;
                    stdOutput = "";
                    stdError = "Es necesario proporcionar un nombre de usuario";
                    return code;
                }
                if (password.Length == 0)
                {
                    code = 10;
                    stdOutput = "";
                    stdError = "Es necesario proporcionar la contraseña de usuario";
                    return code;
                }
                pass = new System.Security.SecureString();
                foreach (char c in password)
                    pass.AppendChar(c);
            }
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("El nombre del proceso no puede ser nulo");
            try
            {
                process.StartInfo.FileName = name;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                if (runAsAdmin)
                {
                    process.StartInfo.Verb = "runas";
                    process.StartInfo.UserName = user;
                    process.StartInfo.Password = pass;
                }
                process.Start();
                bool waitResult = process.WaitForExit(timeout);
                if (!waitResult)
                {
                    KillProcessByPID(process.Id);
                    code = 1;
                    stdOutput = "";
                    stdError = "A timeout has occurred while waiting the process to end";
                    process.Dispose();
                    return code;
                }
                code = process.ExitCode;
                stdOutput = process.StandardOutput.ReadToEnd();
                stdError = process.StandardError.ReadToEnd();
                process.Dispose();
                return code;
            }
            catch (Exception e)
            {
                if (process != null)
                    process.Dispose();
                throw e;
            }
        }

        public static bool KillProcessByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            try
            {
                Process process = Process.GetProcesses().Where(x => x.ProcessName == name).FirstOrDefault();
                if (process != null)
                {
                    process.Kill();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: {0}", ex.Message);
                return false;
            }
        }

        public static string ReadFileAndConvertToHex(string filePath)
        {
            if (!File.Exists(filePath))
                return "";
            byte[] content = File.ReadAllBytes(filePath);
            return BitConverter.ToString(content).Replace('-', ' ');
        }

        public static void KillProcessByPID(int pid)
        {
            if (pid < 0)
                return;
            try
            {
                Process process = Process.GetProcesses().Where(x => x.Id == pid).FirstOrDefault();
                if (process != null)
                {
                    process.Kill();
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: {0}", ex.Message);
                return;
            }
        }

        public static byte[] ConvertHexStringWithSpaceToBytes(string data)
        {

            byte[] buffer = new byte[] { };
            if (!string.IsNullOrEmpty(data))
            {
                if (data.Replace(" ", "").Length % 2 != 0)
                    return buffer;
                if (data.Length == 2)
                {
                    buffer = new byte[] { Convert.ToByte(data, 16) };
                    return buffer;
                }
                string[] str = data.Split(' ');
                int i = 0;
                foreach (string s in str)
                {
                    i++;
                    Array.Resize(ref buffer, i);
                    buffer[i - 1] = Convert.ToByte(s, 16);
                }
            }
            return buffer;
        }

        public static string[] ExtractDiscoveredIMEIs(string data)
        {
            string[] discoveredIMEIs = null;
            if (!data.Contains("device"))
                return null;
            discoveredIMEIs = data.Replace("List of devices attached\r\n", "").Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < discoveredIMEIs.Length; i++)
                discoveredIMEIs[i] = discoveredIMEIs[i].Remove(discoveredIMEIs[i].IndexOf("device")).Trim();
            if (discoveredIMEIs.Length > 1)
                Array.Reverse(discoveredIMEIs);
            return discoveredIMEIs;
        }

        public static string ExtractFileNameFromPath(string path)
        {
            try
            {
                if (path.Length == 0)
                    return "";
                if (Path.GetFileName(path).Length < 0)
                    return "";
                return Path.GetFileName(path);
            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }

        public static string ExtractParentDirectory(string path)
        {
            try
            {
                if (path.Length == 0)
                    return "";
                //if (!File.Exists(path))
                //    return "";
                if (Path.GetFileName(path).Length < 0)
                    return "";
                return Path.GetDirectoryName(path);
            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }

        public static string ExtractIMEIFromLog(string logPath)
        {
            string lines = "";
            string imei = "";
            if (!File.Exists(logPath))
                return "";
            lines = File.ReadAllText(logPath);
            Match match = Regex.Match(lines, @"IMEI\r\n([0-9]+)\r\n\r\nOK", RegexOptions.IgnoreCase);
            if (match.Success)
                imei = match.Value.Replace("IMEI\r\n", "").Replace("\r\n\r\nOK", "").Trim();
            return imei;
        }

        public static string ExtractIMSIFromLog(string logPath)
        {
            string lines = "";
            string imsi = "";
            if (!File.Exists(logPath))
                return "";
            lines = File.ReadAllText(logPath);
            Match match = Regex.Match(lines, @"IMSI\r\n([0-9]+)\r\n\r\nOK", RegexOptions.IgnoreCase);
            if (match.Success)
                imsi = match.Value.Replace("IMSI\r\n", "").Replace("\r\n\r\nOK", "").Trim();
            return imsi;
        }

        public static string ExtractICCIDFromLog(string logPath)
        {
            string lines = "";
            string iccid = "";
            if (!File.Exists(logPath))
                return "";
            lines = File.ReadAllText(logPath);
            Match match = Regex.Match(lines, @"ICCID\r\n\^ICCID:\s([0-9]+)\r\n\r\nOK", RegexOptions.IgnoreCase);
            if (match.Success)
                iccid = match.Value.Replace("ICCID\r\n", "").Replace("^ICCID: ", "").Replace("\r\n\r\nOK", "").Trim();
            return iccid;
        }

        public static string ExtractEUICCIDFromLog(string logPath)
        {
            string lines = "";
            string euiccid = "";
            if (!File.Exists(logPath))
                return "";
            lines = File.ReadAllText(logPath);
            Match match = Regex.Match(lines, @"\+CSIM\:\s22\,\""([0-9A-F]+)\""\r\n\r\nOK", RegexOptions.IgnoreCase);
            if (match.Success)
                euiccid = match.Value.Replace("+CSIM: 22,\"", "").Replace("\"\r\n\r\nOK", "").Trim();
            return euiccid;
        }

        public static bool FindStringInFile(string filePath, string dataToFind)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;
                if (dataToFind.Length == 0)
                    return false;
                return File.ReadAllLines(filePath).Where(x => x.Equals(dataToFind)).FirstOrDefault()?.Length > 0;
            }
            catch (IOException e)
            {
                throw e;
            }
            catch (UnauthorizedAccessException e)
            {
                throw e;
            }
        }

        public static string RemoveFileExtension(string path)
        {
            if (path.Length == 0)
                return "";
            return Path.GetFileNameWithoutExtension(path);
        }
    }
}