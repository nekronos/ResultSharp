using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ResultSharp.Tests
{
	static class SerializationHelpers
	{
		public static byte[] Serialize<T>(T obj) where T : ISerializable
		{
			using (var stream = new MemoryStream())
			{
				new BinaryFormatter().Serialize(stream, obj);
				return stream.ToArray();
			}
		}

		public static T Deserialize<T>(byte[] data) where T : ISerializable
		{
			using (var stream = new MemoryStream(data))
			{
				return (T)new BinaryFormatter().Deserialize(stream);
			}
		}
	}
}
