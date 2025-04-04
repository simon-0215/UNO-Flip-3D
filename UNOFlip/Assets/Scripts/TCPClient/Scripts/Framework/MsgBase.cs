using UnityEngine;
using Newtonsoft.Json;
using System;

namespace MyTcpClient
{
    public class MsgBase
    {
        /// <summary>
        /// Э����
        /// </summary>
        public string protoName = "";

        public static byte[] Encode(MsgBase msg)
        {
            string str = JsonConvert.SerializeObject(msg);

            //Debug.Log($"Encode {str}");
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
        {
            string str = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
            //��ע�⡿��ߵ� Type.GetType( ����������ռ�·�� ) ��Щ�����ռ����׳����⣬������������һ����
            MsgBase msg = (MsgBase)JsonConvert.DeserializeObject(str, Type.GetType("MyTcpClient." + protoName));
            return msg;
        }

        public static byte[] EncodeName(MsgBase msg)
        {
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msg.protoName);
            Int16 len = (Int16)nameBytes.Length;
            byte[] bytes = new byte[len + 2];

            //�����ֳ�����С�˱������ʽ������nameBytesǰ��2byte
            bytes[0] = (byte)(len%256);
            bytes[1] = (byte)(len/256);
            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;
        }

        /// <summary>
        /// ����Э�����ַ���
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="count">ͬʱҲ����Э������Ϣ���ֽ���</param>
        /// <returns></returns>
        public static string DecodeName(byte[] bytes, int offset, out int count)
        {
            count = 0;

            //�������㹻���ֽ������ܹ�����Э����������ֵ��2��byte��
            if(offset + 2 > bytes.Length)
            {
                return "";
            }

            //�������㹻���ֽ������ܹ���������Э������Ϣ��len��byte��
            Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
            if(len <= 0 || offset + 2 + len > bytes.Length)
            {
                return "";
            }

            count = 2 + len;
            string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
            return name;
        }

    }
}
