using System;
using System.Collections.Generic;
using System.Linq;
using AVT;
using AVT.CAN;
using System.IO;
using System.Diagnostics;

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
            if (blocks == null)
                throw new ArgumentNullException("El arreglo de bloques no puede ser nulo");
            if (blocks.Length == 0)
                throw new ArgumentException("El arreglo de bloques no puede estar vacío");
            if (blocks.Length < sectorSize)
                throw new ArgumentOutOfRangeException("El tamaño del bloque no puede ser menor al tamaño del sector");
            return blocks.Skip(sectorSize * currentSector).Take(sectorSize).ToArray();
        }

        List<byte[]> GetBlocksFromAppFile(string appPath, int sectorSize)//sectorSize = 2048
        {
            List<byte[]> blocks = null;
            List<byte[]> sector = null;
            byte[] buffer = null;
            int totalBlocks;
            int blockSize;
            int residue;
            if (appPath.Length == 0)
                throw new ArgumentException("La ruta del archivo no puede ser nula");
            if (!File.Exists(appPath))
                throw new FileNotFoundException("El archivo no existe en el sistema");
            try
            {
                buffer = File.ReadAllBytes(appPath);
                if (buffer.Length == 0)
                    return null;
                blockSize = sectorSize / 8;
                if (buffer.Length % sectorSize > 0)
                {
                    residue = buffer.Length % sectorSize;
                    totalBlocks = (buffer.Length / sectorSize) + 1;
                    blocks = new List<byte[]>(totalBlocks);
                    for (int i = 0; i < totalBlocks; i++)
                    {
                        if (i == totalBlocks - 1)
                            blocks.Add(buffer.Skip(sectorSize * i).Take(residue).ToArray());
                        else
                            blocks.Add(buffer.Skip(sectorSize * i).Take(sectorSize).ToArray());
                    }
                }
                else
                {
                    totalBlocks = buffer.Length / sectorSize; //896
                    sector = new List<byte[]>(totalBlocks);
                    byte[] aux;
                    for (int i = 0; i < totalBlocks; i++)
                    {
                        aux = new byte[14 * 256];//3584
                        for (int j = 0; j < 256; j++)
                        {
                            byte[] frame = new byte[8 + 6];
                            frame[0] = 0x12;
                            frame[1] = (byte)(((0x0b) & 0xff00) >> 8);
                            frame[2] = (byte)((0x0b) & 0xff);
                            frame[3] = 0x20;
                            frame[4] = (byte)(((0x600 + j) & 0xff00) >> 8);
                            frame[5] = (byte)(0x600 + j & 0xff);
                            Buffer.BlockCopy(buffer, (8 * j) + (i * 256), frame, 6, 8);
                            Buffer.BlockCopy(frame, 0, aux, j * frame.Length, frame.Length);
                        }
                        sector.Add(aux);
                    }
                }
                return sector;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool FlashImageToCalypso(ref AVT852 avtInstance, ref CANBus canInstance, string imagePath, int sectorSize, out string stdOutput)
        {
            List<byte[]> Sectors = null;
            try
            {
                Sectors = GetBlocksFromAppFile(imagePath, sectorSize);//896
                if (Sectors.Count > 0)
                {
                    elapsed = new Stopwatch();
                    elapsed.Start();
                    for (int sectorNum = 0; sectorNum < Sectors.Count; sectorNum++)
                    {
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
                        //Send sector of 2048 bytes
                        canInstance.SendPacket(Sectors[sectorNum]);
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
