﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ixq.Redis
{
    /// <summary>
    ///     二进制序列化服务。
    /// </summary>
    public class BinarySerializableService : ISerializableService
    {
        public object Deserialize(byte[] data)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(data))
            {
                var result = binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }

        public byte[] Serialize(object obj)
        {
            if (obj == null)
                return null;
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                var data = memoryStream.ToArray();
                return data;
            }
        }
    }
}