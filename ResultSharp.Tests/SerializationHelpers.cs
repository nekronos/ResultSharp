using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ResultSharp.Tests
{
	static class SerializationHelpers
	{
		public static string Serialize<T>(T obj) where T : ISerializable =>
			JsonConvert.SerializeObject(obj);

		public static T Deserialize<T>(string json) where T : ISerializable =>
			JsonConvert.DeserializeObject<T>(json)!;
	}
}
