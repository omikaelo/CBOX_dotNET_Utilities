using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVT;
using AVT.CAN;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace C_Box
{
    public class CalypsoFlash
    {
        Frame response = new Frame();
        Stopwatch elapsed;

        public CalypsoFlash()
        {
            response = new Frame();
            elapsed = null;
        }
        /// <summary>
        /// Retrieves the next sector of data from a collection of 8-byte blocks
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="sectorSize"></param>
        /// <param name="currentSector"></param>
        /// <returns></returns>
        string[] GetNextSectorOfBlocks(string[] blocks, int sectorSize, int currentSector)
        {
            if (blocks.Length == 0)
                throw new ArgumentException("El arreglo de bloques no puede ser nulo");
            if (blocks.Length < sectorSize)
                return null;
            return blocks.Skip(sectorSize * currentSector).Take(sectorSize).ToArray();
        }

        string[] GetBlocksFromAppFile(string appPath, int blockSize)
        {
            List<byte[]> blocks = null;
            List<string> chunks = null;
            byte[] buffer = null;
            int totalBlocks;
            int residue;
            if (appPath.Length == 0)
                throw new ArgumentException("La ruta del archivo no puede ser nula");
            if (!File.Exists(appPath))
                throw new FileNotFoundException("El archivo no existe en el sistema");
            buffer = File.ReadAllBytes(appPath);
            if (buffer.Length == 0)
                return null;
            if (buffer.Length % blockSize > 0)
            {
                residue = buffer.Length % blockSize;
                totalBlocks = (buffer.Length / blockSize) + 1;
                blocks = new List<byte[]>(totalBlocks);
                for (int i = 0; i < totalBlocks; i++)
                {
                    if (i == totalBlocks - 1)
                        blocks.Add(buffer.Skip(blockSize * i).Take(residue).ToArray());
                    else
                        blocks.Add(buffer.Skip(blockSize * i).Take(blockSize).ToArray());
                }
            }
            else
            {
                totalBlocks = buffer.Length / blockSize;
                blocks = new List<byte[]>(totalBlocks);
                for (int i = 0; i < totalBlocks; i++)
                    blocks.Add(buffer.Skip(blockSize * i).Take(blockSize).ToArray());
            }
            chunks = new List<string>(totalBlocks * 256);
            for (int i = 0; i < totalBlocks; i++)
            {
                for (int j = 0; j < 256; j++)
                    chunks.Add(BitConverter.ToString(blocks[i].Skip(8 * j).Take(8).ToArray()).Replace('-', ' '));
            }
            return chunks.ToArray();
        }

        public bool FlashImageToCalypso(ref AVT852 avtInstance, ref CANBus canInstance, string imagePath, int sectorSize, int blockSize, out string stdOutput)
        {
            int totalSectors = 0;
            ushort blockNum = 0;
            int sectorNum = 0;
            ushort baseID = 0x600;
            string[] DataBlocks = null;
            string[] Sector = null;
            try
            {
                DataBlocks = GetBlocksFromAppFile(imagePath, sectorSize);//896
                totalSectors = DataBlocks.Length / blockSize;
                if (DataBlocks.Length > 0)
                {
                    elapsed = new Stopwatch();
                    elapsed.Start();
                    for (sectorNum = 0; sectorNum < totalSectors; sectorNum++)
                    {
                        Sector = GetNextSectorOfBlocks(DataBlocks, blockSize, sectorNum);
                        //Send Block_Begin
                        canInstance.Send("03 33", "00 00 08 00");
                        //Wait for acknowledge
                        response = avtInstance.Events.CAN.GetByID(0x100, AVT.Enums.Channel.CAN_0, 10000);
                        if (response.Data != "03 33")
                        {
                            elapsed.Stop();
                            stdOutput = $"Error in Block_Begin acknowledge, received {response.Data} and expecting 03 33\r\nSector #: {sectorNum}\r\nElapsed time: {elapsed.ElapsedMilliseconds} ms";
                            return false;
                        }
                        for (blockNum = 0; blockNum < Sector.Length; blockNum++)
                        {
                            //Send block of 8 bytes
                            canInstance.Send((ushort)(baseID + blockNum), Sector[blockNum]);
                        }
                        //Send Block_End
                        canInstance.Send("03 35", "");
                        //Wait for acknowledge
                        response = avtInstance.Events.CAN.GetByID(0x100, AVT.Enums.Channel.CAN_0, 10000);
                        if (response.Data != "03 35")
                        {
                            elapsed.Stop();
                            stdOutput = $"Error in Block_End acknowledge, received {response.Data} and expecting 03 35\r\nSector #: {sectorNum}\r\nElapsed time: {elapsed.ElapsedMilliseconds} ms";
                            return false;
                        }
                        //Wait for Block_Transfer_Result
                        response = avtInstance.Events.CAN.GetByID(0x331, AVT.Enums.Channel.CAN_0, 10000);
                        if (response.Data != "00 00 00 00")
                        {
                            elapsed.Stop();
                            stdOutput = $"Error in Block_Transfer_Result response, received {response.Data} and expecting 00 00 00 00\r\nSector #: {sectorNum}\r\nElapsed time: {elapsed.ElapsedMilliseconds} ms";
                            return false;
                        }
                        //Send Flash_Sent_Data
                        canInstance.Send("03 37", "");
                        //Wait for Flash_Done
                        response = avtInstance.Events.CAN.GetByID(0x24f, AVT.Enums.Channel.CAN_0, 20000);
                        if (response.Data != "00 00")
                        {
                            elapsed.Stop();
                            stdOutput = $"Error in Flash_Done response, received {response.Data} and expecting 00 00\r\nSector #: {sectorNum}\r\nElapsed time: {elapsed.ElapsedMilliseconds} ms";
                            return false;
                        }
                    }
                    elapsed.Stop();
                }
                stdOutput = $"Flash process done. Elapsed time: {elapsed.ElapsedMilliseconds} ms";
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
