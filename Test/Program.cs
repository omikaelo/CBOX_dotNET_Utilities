using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;
using C_Box;
using System.IO;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(Utilities.ConvertHexASCIIToChar("24 47 50 47 53 56 2c 33 2c 31 2c 30 39 2c 30 31 2c 31 30 2c 30 39 36 2c 33 37 2c 30 34 2c 33 31"));
            //Console.WriteLine(Utilities.ConvertHexASCIIToChar("24 47 4c 47 53 56 2c 31 2c 31 2c 30 30 2a 36 35"));

            //string[] gps = Utilities.ExtractGPSMessages(File.ReadAllText(@"C:\Users\Lear\Documents\nmea_2.txt"));
            //Console.WriteLine($"GPS SNR = {Utilities.ExtractSNRFromGPSMessage(gps[0])}");
            //string[] glo = Utilities.ExtractGLONASSMessages(File.ReadAllText(@"C:\Users\Lear\Documents\nmea_2.txt"));
            //string[] ordered = Utilities.SortGPSMessagesBySNRDesc(gps);
            //Console.WriteLine($"GPS PRN: {Utilities.ExtractIDFromGPSMessage(ordered[0])}");
            Console.WriteLine($"IMEI: {Utilities.ExtractIMEIFromLog(@"C:\C-BOX\System Files\ttlog.txt")}");
            Console.WriteLine($"IMSI: {Utilities.ExtractIMSIFromLog(@"C:\C-BOX\System Files\ttlog.txt")}");
            Console.WriteLine($"ICCID: {Utilities.ExtractICCIDFromLog(@"C:\C-BOX\System Files\ttlog.txt")}");
            Console.WriteLine($"EUICCID: {Utilities.ExtractEUICCIDFromLog(@"C:\C-BOX\System Files\ttlog.txt")}");
            Console.ReadLine();
        }
    }
}
