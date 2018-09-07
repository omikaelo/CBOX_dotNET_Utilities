using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;
using C_Box;
using System.IO;
using System.Collections.Generic;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //string stdError;
                //string stdOutput;
                //Console.WriteLine(Utilities.ConvertHexASCIIToChar("24 47 50 47 53 56 2c 33 2c 31 2c 30 39 2c 30 31 2c 31 30 2c 30 39 36 2c 33 37 2c 30 34 2c 33 31"));
                //Console.WriteLine(Utilities.ConvertHexASCIIToChar("24 47 4c 47 53 56 2c 31 2c 31 2c 30 30 2a 36 35"));

                //string[] gps = Utilities.ExtractGPSMessages(File.ReadAllText(@"C:\Users\Lear\Documents\nmea_2.txt"));
                //Console.WriteLine($"GPS SNR = {Utilities.ExtractSNRFromGPSMessage(gps[0])}");
                //string[] glo = Utilities.ExtractGLONASSMessages(File.ReadAllText(@"C:\Users\Lear\Documents\nmea_2.txt"));
                //string[] ordered = Utilities.SortGPSMessagesBySNRDesc(gps);
                //Console.WriteLine($"GPS PRN: {Utilities.ExtractIDFromGPSMessage(ordered[0])}");
                //Console.WriteLine($"IMEI: {Utilities.ExtractIMEIFromLog(@"C:\C-BOX\System Files\ttlog.txt")}");
                //Console.WriteLine($"IMSI: {Utilities.ExtractIMSIFromLog(@"C:\C-BOX\System Files\ttlog.txt")}");
                //Console.WriteLine($"ICCID: {Utilities.ExtractICCIDFromLog(@"C:\C-BOX\System Files\ttlog.txt")}");
                //Console.WriteLine($"EUICCID: {Utilities.ExtractEUICCIDFromLog(@"C:\C-BOX\System Files\ttlog.txt")}");
                //Console.WriteLine(Utilities.LaunchShell(@"C:\C-BOX\Utilities\Scripting\ResetCOM\Huaweicomreset.bat", "", out stdOutput, out stdError));
                //Console.WriteLine(stdOutput);
                //Console.WriteLine(Utilities.ConvertDateToBDCFormat());
                //string d = Utilities.ConvertASCIIStringToHex("4K103528200");
                //Console.WriteLine(Utilities.ReverseHexString(d));
                //List<string[]> sectors = null;
                //string[] blocks = Utilities.GetBlocksFromAppFile(@"C:\C-BOX\Variants\ConBox_Low_NAR\Firmware\Calypso\Calypso_APP_v100_nvm\app_0100_nvm_repair.bin", 2048);
                //int sectorCounter = blocks.Length / 256;
                //sectors = new List<string[]>(sectorCounter);
                //for (int i = 0; i < sectorCounter; i++)
                //{
                //    sectors.Add(Utilities.GetNextSectorOfBlocks(blocks, 256, i));
                //}
                //Console.WriteLine(Utilities.FindStringInFile(@"C:\C-BOX\Variants\ConBox_Low_NAR\Firmware\NAD\NAD_APP_v034\version_info.txt", "Buildname = CLU1_CBOXL_ALL_ALL_034PROD"));
                //string stdOut;
                //string stdError;
                //Utilities.GetSectionFromConfigurationFile(@"C:\C-BOX\System Files\Control Signals.ini", "NEST_01");
                //Utilities.LaunchShell(@"C:\C-BOX\Utilities\Scripting\HUAWEI_COM_Reset.bat", "", out stdOut, out stdError, 20000, true, "zamtest", "zamtest");
                //Console.WriteLine(stdOut);
                //Console.WriteLine(stdError);
                //Utilities.ExtractApplicationInterfacePort("Name                                             \r\r\nHUAWEI Mobile Connect - PC UI Interface(COM36)  \r\r\n\r\r\nName                                     \r\r\nHUAWEI Mobile Connect - SerialC(COM38)  \r\r\n\r\r\nName                                     \r\r\nHUAWEI Mobile Connect - SerialB(COM37)  \r\r\n\r\r\nName                                                   \r\r\nHUAWEI Mobile Connect - Application Interface(COM35)  \r\r\n\r\r\n");
                int id;
                string mac;
                SQLTransaction.Server = "10.103.40.65";
                SQLTransaction.DataBase = "itac";
                SQLTransaction.UserID = "mac_usr";
                SQLTransaction.Password = "eol_mac";
                SQLTransaction.GetNextAvailableMAC(out id, out mac);
                Console.WriteLine($"ID= {id}\r\nMAC= {mac}");
                if (id > 0)
                    Console.WriteLine($"MAC is used to false: {SQLTransaction.SetMACIsUsed(id, false)}");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Excepcion: {e.ToString()}");
                Console.ReadLine();
            }
        }
    }
}
