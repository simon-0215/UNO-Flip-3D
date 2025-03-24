using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetworkGame.TCPServer
{
    public class ByteArray
    {
        const int DEFAULT_SIZE = 1024;//默认大小
        int initSize = 0;//初始大小

        public byte[] bytes;
        public int readIdx = 0;

        /// <summary>
        /// 可写开头位置
        /// </summary>
        public int writeIdx = 0;

        private int capacity = 0;//容量

        /// <summary>
        /// 剩余空间
        /// </summary>
        public int remain { get { return capacity - writeIdx; } }

        /// <summary>
        /// 已存数据长度
        /// </summary>
        public int length { get { return writeIdx - readIdx; } }

        public ByteArray(int size = DEFAULT_SIZE)
        {
            bytes = new byte[size];
            capacity = size;
            initSize = size;
            readIdx = 0;
            writeIdx = 0;
        }
        public ByteArray(byte[] pBytes)
        {
            bytes = pBytes;
            capacity = pBytes.Length;
            initSize = pBytes.Length;

            readIdx = 0;
            writeIdx = pBytes.Length;
        }

        public void ReSize(int size)
        {
            if (size < length) return;
            if (size < initSize) return;

            int n = 1;
            while (n < size) n *= 2;

            capacity = n;
            byte[] newBytes = new byte[capacity];
            Array.Copy(bytes, readIdx, newBytes, 0, writeIdx - readIdx);
            bytes = newBytes;

            writeIdx = length;
            readIdx = 0;
        }

        public void MoveBytes()
        {
            if (length > 0)
            {
                Array.Copy(bytes, readIdx, bytes, 0, length);
            }

            writeIdx = length;
            readIdx = 0;
        }

        public void CheckAndMoveBytes()
        {
            if (length < 8 || readIdx == writeIdx)
            {
                MoveBytes();
            }
        }

        public int Write(byte[] bs, int offset, int count)
        {
            if (remain < count)
            {
                ReSize(length + count);
            }
            Array.Copy(bs, offset, bytes, writeIdx, count);
            writeIdx += count;
            return count;
        }

        public int Read(byte[] bs, int offset, int count)
        {
            count = Math.Min(count, length);
            Array.Copy(bytes, readIdx, bs, offset, count);
            readIdx += count;
            CheckAndMoveBytes();
            return count;
        }

        public Int16 ReadInt16()
        {
            if (length < 2) return 0;

            Int16 ret = (Int16)((bytes[1] << 8) | bytes[0]);
            readIdx += 2;
            CheckAndMoveBytes();
            return ret;
        }

        /// <summary>
        /// 只尝试读数据，不移动readIdx
        /// </summary>
        /// <returns></returns>
        public Int16 ReadInt16NotMoveIdx()
        {
            if (length < 2) return 0;

            Int16 ret = (Int16)((bytes[1] << 8) | bytes[0]);
            return ret;
        }

        public Int32 ReadInt32()
        {
            if (length < 4) return 0;

            Int32 ret = (Int32)(
                (bytes[3] << 24) |
                (bytes[2] << 16) |
                (bytes[1] << 8) |
                bytes[0]);

            readIdx += 4;
            CheckAndMoveBytes();
            return ret;
        }

        //打印缓冲区（仅为调试）
        public override string ToString()
        {
            return BitConverter.ToString(bytes, readIdx, length);
        }

        //打印调试信息（仅为调试）
        public string Debug()
        {
            return string.Format("readIdx({0}) writeIdx({1}) bytes({2})",
                readIdx,
                writeIdx,
                BitConverter.ToString(bytes, 0, bytes.Length)
            );
        }
    }
}
