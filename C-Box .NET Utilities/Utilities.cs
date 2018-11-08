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
using System.Collections.Generic;
using Microsoft.Win32;

namespace C_Box
{
    public class Utilities
    {
        public Utilities()
        { }

        public bool Ping(string ipAddress)
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

        public string[] GetSectionFromConfigurationFile(string file, string section)
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

        public string GetKeyFromConfigurationFile(string file, string section, string key)
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

        public string GetKLCalGradient(string data)
        {
            if (string.IsNullOrEmpty(data))
                return "";
            if (data.Length < 23)
                return "";
            return data.Substring(0, 11);
        }

        public string GetKLCalOffset(string data)
        {
            if (string.IsNullOrEmpty(data))
                return "";
            if (data.Length < 23)
                return "";
            return data.Substring(12);
        }

        public int ConvertHexStringWithSpaceToNumber(string data)
        {
            string hexNum = "";
            if (string.IsNullOrEmpty(data))
                return -1;
            hexNum = data.Replace(" ", "");
            if (hexNum.Length % 2 != 0)
                return -1;
            return Convert.ToInt32(hexNum, 16);
        }

        public string ConvertInt32ToHexStringWithSpace(uint value)
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

        public string ConvertInt16ToHexStringWithSpace(ushort value)
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

        public string ConvertHexStringWithSpecifierToHexStringWithSpace(string hex)
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

        public string ConvertASCIIStringToHex(string data)
        {
            return BitConverter.ToString(Encoding.ASCII.GetBytes(data)).Replace('-', ' ');
        }

        public string ReverseHexString(string data)
        {
            string[] values = null;
            if (data.Length == 0)
                return "";
            values = data.Split(' ').Reverse().ToArray();
            return string.Join(" ", values);

        }

        public string[] ExtractGPSMessages(string data)
        {
            if (data.Length == 0)
                return new string[] { };
            return data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.StartsWith("$GP") && x.Contains("*")).ToArray();
        }

        public string[] SortGPSMessagesBySNRDesc(string[] messages)
        {
            string[] ordered = null;
            if (messages.Length == 0)
                return new string[] { };
            ordered = messages.OrderBy(x => Convert.ToInt32(x.Split(',').Last().Remove(x.Split(',').Last().IndexOf("*")))).ToArray();
            return ordered;
        }

        public string[] SortGLONASSMessagesBySNRDesc(string[] messages)
        {
            string[] ordered = null;
            if (messages.Length == 0)
                return new string[] { };
            ordered = messages.OrderBy(x => Convert.ToInt32(x.Split(',').Last().Remove(x.Split(',').Last().IndexOf("*")))).ToArray();
            return ordered;
        }

        public string[] ExtractGLONASSMessages(string data)
        {
            if (data.Length == 0)
                return new string[] { };
            return data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.StartsWith("$GL") && x.Contains("*")).ToArray();
        }

        public int ExtractIDFromGPSMessage(string message)
        {
            string id = "";
            if (message.Length == 0)
                return -1;
            if (!message.StartsWith("$GP") || !message.Contains("*"))
                return -1;
            id = message.Split(',')[4];
            return Convert.ToInt32(id);
        }

        public int ExtractSNRFromGPSMessage(string message)
        {
            if (message.Length == 0)
                return -1;
            if (!message.StartsWith("$GP") || !message.Contains("*"))
                return -1;
            string snr = message.Split(',').Last();
            snr = snr.Remove(snr.IndexOf("*"));
            return Convert.ToInt32(snr);
        }

        public int ExtractSNRFromGLONASSMessage(string message)
        {
            if (message.Length == 0)
                return -1;
            if (!message.StartsWith("$GL") || !message.Contains("*"))
                return -1;
            string snr = message.Split(',').Last();
            snr = snr.Remove(snr.IndexOf("*"));
            return Convert.ToInt32(snr);
        }

        public string ConvertHexASCIIToChar(string data)
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

        public string[] Get8ByteChunksFromFWPath(string data)
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

        public string ConvertDateToBDCFormat()
        {
            string year = DateTime.Now.ToString("yy");
            string day = DateTime.Now.ToString("dd");
            string month = DateTime.Now.ToString("MM");
            return BitConverter.ToString(new byte[] { Convert.ToByte(year, 16), Convert.ToByte(month, 16), Convert.ToByte(day, 16) }).Replace('-', ' ');
        }

        public bool WriteKeyToIniFile(string filePath, string section, string key, string value)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(filePath);
                data[section][key] = value;
                parser.WriteFile(filePath, data);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool WriteKeysToFile(string folderPath, string fileName, string Startkeyverificationkonstante, string IKA_SCK, string ECU_Master_KEY, string Debug_CC, string Status_KS)
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
                throw ex;
            }
        }

        public bool ExtractLSBAndMSBFromPWD(string pwd, out string lsb, out string msb)
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

        public bool WriteIPC_Key_ToFile(string folder, string data)
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

        public string ExtractSerialNumber(string data)
        {

            string sn = "";
            if (string.IsNullOrEmpty(data))
                return "";
            if (!data.Contains(","))
                return "";
            sn = data.Split(',').Where(x => x.Length == 18).FirstOrDefault();
            return sn;
        }

        public string ExtractPCUIInterfacePort(string data)
        {
            if (data.Length == 0)
                return "";
            string port = data.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries).ElementAt(1).Trim();
            port = port.Remove(0, port.IndexOf("(") + 1).Replace(")", "");
            return port;
        }

        public string ExtractSerialCPort(string data)
        {
            if (data.Length == 0)
                return "";
            string port = data.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries).ElementAt(4).Trim();
            port = port.Remove(0, port.IndexOf("(") + 1).Replace(")", "");
            return port;
        }

        public string ExtractSerialBPort(string data)
        {
            if (data.Length == 0)
                return "";
            string port = data.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries).ElementAt(7).Trim();
            port = port.Remove(0, port.IndexOf("(") + 1).Replace(")", "");
            return port;
        }

        public string Extract3GApplicationPort(string data)
        {
            if (data.Length == 0)
                return "";
            string port = data.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries).ElementAt(10).Trim();
            port = port.Remove(0, port.IndexOf("(") + 1).Replace(")", "");
            return port;
        }

        public string ExtractApplicationPort(string data)
        {
            if (data.Length == 0)
                return "";
            string port = data.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries).ElementAt(11).Trim();
            port = port.Remove(0, port.IndexOf("(") + 1).Replace(")", "");
            return port;
        }

        public string ExtractGPSInterfacePort(string data)
        {
            if (data.Length == 0)
                return "";
            string port = data.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries).ElementAt(14).Trim();
            port = port.Remove(0, port.IndexOf("(") + 1).Replace(")", "");
            return port;
        }

        public bool WriteToFile(string path, string data)
        {
            try
            {
                if (data.Length < 0)
                    return false;
                File.WriteAllText(path, data);
                return true;
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            catch (DirectoryNotFoundException e)
            {
                throw e;
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public string GetCurrentYear()
        {
            return DateTime.Now.Year.ToString();
        }

        public string GetCurrentMonth()
        {
            return DateTime.Today.ToString("MMMM").ToUpper();
        }

        public string GetCurrentDay()
        {
            return DateTime.Now.ToString("dddd_dd").ToUpper();
        }

        public string ExtractFAZITString(string data)
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

        public string GetEnvironmentVariable(string variable)
        {
            if (string.IsNullOrEmpty(variable))
                throw new ArgumentNullException("El nombre de la variable no puede ser nulo");
            return Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);
        }

        public void SetEnvironmentVariable(string variable, string value)
        {
            if (string.IsNullOrEmpty(variable) || string.IsNullOrEmpty(value))
                return;
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.User);
        }

        public bool SearchProcessByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return Process.GetProcesses().Where(x => x.ProcessName == name).FirstOrDefault() != null;
        }

        public bool SearchProcessByPID(int pid)
        {
            if (pid < 0)
                return false;
            return Process.GetProcesses().Where(x => x.Id == pid).FirstOrDefault() != null;
        }

        public int GetProcessPID(string name)
        {
            if (string.IsNullOrEmpty(name))
                return -1;
            return Process.GetProcesses().Where(x => x.ProcessName == name).FirstOrDefault().Id;
        }

        public bool LaunchProcess(string name, string arguments, int timeout = 0)
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


        public int LaunchShell(string name, string arguments, out string stdOutput, out string stdError, int timeout, bool runAsAdmin = false, string user = "", string password = "")
        {
            Process p = null;
            ProcessStartInfo psi = new ProcessStartInfo();
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
                psi.FileName = name;
                psi.Arguments = arguments;
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;
                if (runAsAdmin)
                {
                    psi.Verb = "runas";
                    psi.UserName = user;
                    psi.Password = pass;
                }
                p = Process.Start(psi);
                bool waitResult = p.WaitForExit(timeout);
                if (!waitResult)
                {
                    KillProcessByPID(p.Id);
                    code = 1;
                    stdOutput = "";
                    stdError = "A timeout has occurred while waiting the process to end";
                    p.Dispose();
                    return code;
                }
                code = p.ExitCode;
                stdOutput = p.StandardOutput.ReadToEnd();
                stdError = p.StandardError.ReadToEnd();
                p.Dispose();
                return code;
            }
            catch (Exception e)
            {
                if (p != null)
                    p.Dispose();
                throw e;
            }
        }

        public bool KillProcessByName(string name)
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

        public string ReadFileAndConvertToHex(string filePath)
        {
            if (!File.Exists(filePath))
                return "";
            byte[] content = File.ReadAllBytes(filePath);
            return BitConverter.ToString(content).Replace('-', ' ');
        }

        public void KillProcessByPID(int pid)
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

        public byte[] ConvertHexStringWithSpaceToBytes(string data)
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

        public string[] ExtractDiscoveredIMEIs(string data, int nestIndex)
        {
            string[] discoveredIMEIs = null;
            if (!data.Contains(" "))
                return null;
            discoveredIMEIs = data.Replace("List of devices attached\r\n", "").Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < discoveredIMEIs.Length; i++)
                discoveredIMEIs[i] = discoveredIMEIs[i].Remove(discoveredIMEIs[i].IndexOf(" ")).Trim();
            return discoveredIMEIs;
        }

        public string ExtractFileNameFromPath(string path)
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

        public string ExtractParentDirectory(string path)
        {
            try
            {
                if (path.Length == 0)
                    return "";
                if (Path.GetFileName(path).Length < 0)
                    return "";
                return Path.GetDirectoryName(path);
            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }

        public void FormatATLog(string logPath)
        {
            List<string> lines = null;
            int index = -1;
            try
            {
                if (string.IsNullOrEmpty(logPath))
                    throw new ArgumentNullException("El nombre de archivo no puede ser nulo");
                if (!File.Exists(logPath))
                    throw new FileNotFoundException("El arcvhivo especificado no existe en el sistema");
                lines = File.ReadAllLines(logPath).ToList();
                index = lines.FindIndex(x => x.Contains("^PLMNSELEINFO:"));
                if (index >= 0)
                {
                    lines.RemoveAt(index);
                    File.WriteAllLines(logPath, lines.ToArray());
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string ExtractIMEIFromLog(string logPath)
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

        public string ExtractIMSIFromLog(string logPath)
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

        public string ExtractICCIDFromLog(string logPath)
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

        public string ExtractEUICCIDFromLog(string logPath)
        {
            string lines = "";
            string euiccid = "";
            if (!File.Exists(logPath))
                return "";
            lines = File.ReadAllText(logPath);
            Match firstMatch = Regex.Match(lines, @"\+CSIM\:\s22\,\""([0-9A-F]+)\""\r\n\r\nOK", RegexOptions.IgnoreCase);
            Match secondMatch = Regex.Match(lines, @"\+CSIM\:\s34\,\""([0-9A-F]+)\""\r\n\r\nOK", RegexOptions.IgnoreCase);
            if (firstMatch.Success)
                euiccid = firstMatch.Value.Replace("+CSIM: 22,\"", "").Replace("\"\r\n\r\nOK", "").Trim();
            else if (secondMatch.Success)
                euiccid = secondMatch.Value.Replace("+CSIM: 34,\"", "").Replace("\"\r\n\r\nOK", "").Trim();
            else
                euiccid = "";
            return euiccid;
        }

        public string ExtractSOCIDFromLog(string logPath)
        {
            int index = 0;
            List<string> lines = new List<string>();
            string socid = "";
            try
            {
                if (!File.Exists(logPath))
                    return "";
                lines = File.ReadAllLines(logPath).ToList();
                index = lines.FindIndex(x => x.Contains("Found SOCID:"));
                if (index < 0)
                    return "";
                socid = lines[index].Replace("Found SOCID: ", "").Replace("\r\n", "").Trim();
                return socid;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool FindStringInFile(string filePath, string dataToFind)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;
                if (dataToFind.Length == 0)
                    return false;
                return File.ReadAllLines(filePath).Where(x => x.Contains(dataToFind)).FirstOrDefault()?.Length > 0;
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

        public string RemoveStringFromFilename(string fileName, string strToRemove)
        {
            try
            {
                return fileName.Replace(strToRemove, "");
            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }

        public string RemoveFileExtension(string path)
        {
            if (path.Length == 0)
                return "";
            return Path.GetFileNameWithoutExtension(path);
        }

        public bool UpdateNADMACAddressScript(string mac, string scriptPath, string logPath)
        {
            string[] content = new string[] { };
            try
            {
                if (!File.Exists(scriptPath))
                    return false;
                content = File.ReadAllLines(scriptPath);
                for (int i = 0; i < content.Length; i++)
                {
                    if (content[i].Contains("logopen"))
                        content[i] = $"logopen '{logPath}' 1 0";
                    else if (content[i].Contains("sendln 'AT^DEVINFO=0,"))
                        content[i] = $"sendln 'AT^DEVINFO=0,{mac}'";
                }
                File.WriteAllLines(scriptPath, content);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool CheckIfFileExists(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return false;
                return true;
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public bool CreateSocketLogFolder(string logPath)
        {
            try
            {
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);
                return true;
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public bool UpdateLogPathATScript(string logPath, string scriptPath)
        {
            string[] content = new string[] { };
            if (!File.Exists(scriptPath))
                return false;
            content = File.ReadAllLines(scriptPath);
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Contains("logopen"))
                    content[i] = $"logopen '{logPath}' 1 0";
            }
            File.WriteAllLines(scriptPath, content);
            return true;
        }

        public bool SearchWordInFile(string filePath, string word)
        {
            string content = "";
            try
            {
                if (!File.Exists(filePath))
                    return false;
                content = File.ReadAllText(filePath);
                if (content.Contains(word))
                    return true;
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool UpdateFAZITFile(string filePath, string iccid, string euiccid, string imsi, string oemPartNumber, string socid)
        {
            int index = 0;
            List<string> content = null;
            try
            {
                if (!File.Exists(filePath))
                    return false;
                content = File.ReadAllLines(filePath).ToList();
                index = content.FindIndex(x => x.Contains("iccid"));
                if (index >= 0)
                    content[index] = $"\"iccid\":\"{iccid}\",";
                else
                    return false;
                index = content.FindIndex(x => x.Contains("\"euiccid\""));
                if (index >= 0)
                    content[index] = $"\"euiccid\":\"{euiccid}\",";
                else
                    return false;
                index = content.FindIndex(x => x.Contains("\"imsi\""));
                if (index >= 0)
                    content[index] = $"\"imsi\":\"{imsi}\",";
                else
                    return false;
                index = content.FindIndex(x => x.Contains("\"oemPartNumber\""));
                if (index >= 0)
                    content[index] = $"\"oemPartNumber\":\"{oemPartNumber}\",";
                else
                    return false;
                index = content.FindIndex(x => x.Contains("\"serial\""));
                if (index >= 0)
                    content[index] = content[index].Trim() + ",";
                else
                    return false;
                content.Insert(content.ToList().Count - 1, $"\"socid\":\"{socid}\"");
                File.WriteAllLines(filePath, content);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FixSerialNumberLen(string serialNumber, int len, char padChar)
        {
            string aux = "";
            if (serialNumber.Length > len)
                return "";
            if (len <= 0)
                return "";
            if (padChar == ' ')
                return "";
            aux = serialNumber.PadLeft(len, padChar);
            return aux;
        }

        public string FixFazitString(string fazit)
        {
            if (fazit.Length == 0)
                return "";
            return fazit.Replace("\n", "");
        }

        public bool RemoveHUWAEIDevice(string devconPath, string imei, string parentIdPrefix)
        {
            string stdout;
            string stderr;
            //Search for children
            if (LaunchShell(devconPath, "findall \"*\\Vid_12D1&Sub*\"", out stdout, out stderr, 10000, false, null, null) != 0)
                return false;
            if (stdout.Contains("Error"))
                return false;
            string[] devices = stdout.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string dev in devices)
            {
                if (dev.Contains(parentIdPrefix.ToUpperInvariant()))
                {
                    if (LaunchShell(devconPath, "remove \"@" + dev.Split(':')[0].Trim() + "\"", out stdout, out stderr, 10000, false, null, null) != 0)
                        return false;
                }
            }
            //Now search for composote devices
            if (LaunchShell(devconPath, "findall \"*\\Vid_12D1&PID_15C3\"", out stdout, out stderr, 10000, false, null, null) != 0)
                return false;
            if (stdout.Contains("Error"))
                return false;
            devices = stdout.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string dev in devices)
            {
                if (dev.Contains(imei))
                {
                    if (LaunchShell(devconPath, "remove \"@" + dev.Split(':')[0].Trim() + "\"", out stdout, out stderr, 10000, false, null, null) != 0)
                        return false;
                }
            }
            return true;
        }

        public string GetParentIdPrefix(string imei)
        {
            try
            {
                using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    if (hklm != null)
                    {
                        using (RegistryKey dev = hklm.OpenSubKey($@"SYSTEM\CurrentControlSet\Enum\USB\VID_12D1&PID_15C3\{imei}"))
                        {
                            return (string)dev.GetValue("ParentIdPrefix");
                        }
                    }
                    return "";
                }
            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }

        public bool CheckUSBDeviceMapping(string[] imeiArray, string locationInformation, out string matchedIMEI)
        {
            string data;
            try
            {
                using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    if (hklm != null)
                    {
                        for (int i = 0; i < imeiArray.Length; i++)
                        {
                            using (RegistryKey h = hklm.OpenSubKey($@"SYSTEM\CurrentControlSet\Enum\USB\VID_12D1&PID_15C3\{imeiArray[i]}"))
                            {
                                if (h != null)
                                {
                                    data = (string)h.GetValue("LocationInformation");
                                    if (data.Equals(locationInformation))
                                    {
                                        matchedIMEI = imeiArray[i];
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    matchedIMEI = "";
                    return false;
                }
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            catch (UnauthorizedAccessException e)
            {
                throw e;
            }
            catch (System.Security.SecurityException e)
            {
                throw e;
            }
        }

        public string ReadContentFromFAZITFile(string path)
        {
            if (!File.Exists(path))
                return "";
            return File.ReadAllText(path).Replace("\r\n", "");
        }
    }
}