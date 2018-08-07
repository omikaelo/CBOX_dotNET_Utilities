using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;
using C_Box;


namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //            string[] imeis = Utilities.ExtractDiscoveredIMEIs(@"List of devices attached
            //358449080043422	device

            //");
            //            foreach (string imei in imeis)
            //                Console.Write($"Device IMEI: {imei}\n");
            //Console.WriteLine($"File name: {Utilities.ExtractFileNameFromPath(@"C:\C-BOX\Variants\ConBox Low ECE\Firmware\NAD\NAD_EOL\dutnad-2.7.3-B110")}");
            //Console.WriteLine($"ascii to string {Utilities.ConvertHexASCIIToChar("33 35 39 34 34 39 30 38")}");
            string p = Utilities.ExtractPCUIInterfacePort("Name \r\r HUAWEI Mobile Connect - PC UI Interface (COM4) \r\r \r\r Name \r\r HUAWEI Mobile Connect - SerialC (COM7) \r\r \r\r Name \r\r HUAWEI Mobile Connect - SerialB (COM6) \r\r \r\r Name \r\r HUAWEI Mobile Connect - 3G Application Interface (COM3) \r\r HUAWEI Mobile Connect - Application Interface (COM5) \r\r \r\r ");
            Console.WriteLine($"PCUI Interface {p}");
            p = Utilities.ExtractSerialCPort("Name \r\r HUAWEI Mobile Connect - PC UI Interface (COM4) \r\r \r\r Name \r\r HUAWEI Mobile Connect - SerialC (COM7) \r\r \r\r Name \r\r HUAWEI Mobile Connect - SerialB (COM6) \r\r \r\r Name \r\r HUAWEI Mobile Connect - 3G Application Interface (COM3) \r\r HUAWEI Mobile Connect - Application Interface (COM5) \r\r \r\r ");
            Console.WriteLine($"SerialC {p}");
            p = Utilities.ExtractSerialBPort("Name \r\r HUAWEI Mobile Connect - PC UI Interface (COM4) \r\r \r\r Name \r\r HUAWEI Mobile Connect - SerialC (COM7) \r\r \r\r Name \r\r HUAWEI Mobile Connect - SerialB (COM6) \r\r \r\r Name \r\r HUAWEI Mobile Connect - 3G Application Interface (COM3) \r\r HUAWEI Mobile Connect - Application Interface (COM5) \r\r \r\r ");
            Console.WriteLine($"SerialB {p}");
            p = Utilities.Extract3GApplicationPort("Name \r\r HUAWEI Mobile Connect - PC UI Interface (COM4) \r\r \r\r Name \r\r HUAWEI Mobile Connect - SerialC (COM7) \r\r \r\r Name \r\r HUAWEI Mobile Connect - SerialB (COM6) \r\r \r\r Name \r\r HUAWEI Mobile Connect - 3G Application Interface (COM3) \r\r HUAWEI Mobile Connect - Application Interface (COM5) \r\r \r\r ");
            Console.WriteLine($"3G Application Interface {p}");
            p = Utilities.ExtractApplicationInterfacePort("Name \r\r HUAWEI Mobile Connect - PC UI Interface (COM4) \r\r \r\r Name \r\r HUAWEI Mobile Connect - SerialC (COM7) \r\r \r\r Name \r\r HUAWEI Mobile Connect - SerialB (COM6) \r\r \r\r Name \r\r HUAWEI Mobile Connect - 3G Application Interface (COM3) \r\r HUAWEI Mobile Connect - Application Interface (COM5) \r\r \r\r ");
            Console.WriteLine($"Application Interface {p}");
            Console.ReadLine();
        }
    }
}
