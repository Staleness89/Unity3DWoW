using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Assets.Script.Shared
{
    public class UpdateMask
    {

        public UpdateMask()
        {
            mCount = 0;
            mBlocks = 0;
            mUpdateMask = new byte[0];
        }


        public void SetBit(UInt16 index)
        {
            mUpdateMask[index >> 3] |= (byte)(1 << (index & 0x7));
        }

        public void UnsetBit(UInt16 index)
        {
            mUpdateMask[index >> 3] &= (byte)(0xff ^ (1 << (index & 0x7)));
        }

        public bool GetBit(UInt16 index)
        {
            return (mUpdateMask[index >> 3] & (1 << (index & 0x7))) != 0;
        }

        public UInt16 GetBlockCount() { return mBlocks; }
        public UInt16 GetLength() { return (UInt16)(mBlocks << 2); }
        public UInt16 GetCount() { return mCount; }
        public byte[] GetMask() { return mUpdateMask; }

        public void SetCount(UInt16 valuesCount)
        {

            mCount = valuesCount;
            mBlocks = (UInt16)((valuesCount >> 5) + 1);

            mUpdateMask = new byte[mBlocks * 4];
        }

        public void SetMask(byte[] data, UInt16 length)
        {

            mCount = (UInt16)(length << 5);
            mBlocks = (UInt16)(length >> 2);

            mUpdateMask = new byte[mBlocks * 4];
            mUpdateMask = data;
        }

        public void Clear()
        {
            mUpdateMask = new byte[mBlocks * 4];
        }

        public static byte[] Decompress(int Length, byte[] Data)
        {
            byte[] Output = new byte[Length];
            Stream s = new DeflateStream(new MemoryStream(Data), CompressionMode.Decompress);
            int Offset = 0;
            while (true)
            {
                int size = s.Read(Output, Offset, Length);
                if (size == Length) break;
                Offset += size;
                Length -= size;
            }
            return Output;
        }

        public static void Decompress(int Length, byte[] Data, string Filename)
        {
            byte[] Output = Decompress(Length, Data);
            FileStream fs = new FileStream(Filename, FileMode.Create, FileAccess.Write);
            fs.Write(Output, 0, Length);
            fs.Close();
        }

        public static byte[] Compress(byte[] Data)
        {
            MemoryStream ms = new MemoryStream();
            Stream s = new DeflateStream(ms, CompressionMode.Compress);
            s.Write(Data, 0, Data.Length);
            s.Close();
            return ms.ToArray();
        }

        private UInt16 mCount; // in values
        private UInt16 mBlocks; // in UInt32 blocks
        private byte[] mUpdateMask;
    };
}
