using System;
using UnityEngine;

namespace MyTcpClient
{
    public class ByteArray
    {
        const int DEFAULT_SIZE = 1024*8;//Ĭ�ϴ�С
        int initSize = 0;//��ʼ��С

        public byte[] bytes;
        public int readIdx = 0;

        /// <summary>
        /// ��д��ͷλ��
        /// </summary>
        public int writeIdx = 0;

        private int capacity = 0;//����

        /// <summary>
        /// ʣ��ռ�
        /// </summary>
        public int remain { get { return capacity - writeIdx; } }

        /// <summary>
        /// �Ѵ����ݳ���
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

        //��ӡ����������Ϊ���ԣ�
        public override string ToString()
        {
            return BitConverter.ToString(bytes, readIdx, length);
        }

        //��ӡ������Ϣ����Ϊ���ԣ�
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
