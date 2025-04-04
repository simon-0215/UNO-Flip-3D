using UnityEngine;
using Newtonsoft.Json;
using System;

namespace MyTcpClient
{
    public class MsgBase
    {
        /// <summary>
        /// 协议名
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
            //【注意】这边的 Type.GetType( 里面的命名空间路径 ) 少些命名空间容易出问题，还不报错，这是一个坑
            MsgBase msg = (MsgBase)JsonConvert.DeserializeObject(str, Type.GetType("MyTcpClient." + protoName));
            return msg;
        }

        public static byte[] EncodeName(MsgBase msg)
        {
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msg.protoName);
            Int16 len = (Int16)nameBytes.Length;
            byte[] bytes = new byte[len + 2];

            //将名字长度以小端编码的形式放置在nameBytes前面2byte
            bytes[0] = (byte)(len%256);
            bytes[1] = (byte)(len/256);
            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;
        }

        /// <summary>
        /// 解析协议名字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="count">同时也返回协议名信息的字节数</param>
        /// <returns></returns>
        public static string DecodeName(byte[] bytes, int offset, out int count)
        {
            count = 0;

            //必须有足够的字节数，能够解析协议名长度数值（2个byte）
            if(offset + 2 > bytes.Length)
            {
                return "";
            }

            //必须有足够的字节数，能够继续解析协议名信息（len个byte）
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
