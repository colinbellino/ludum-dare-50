using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Game.Core
{
	public static class BinaryFileSerializer
	{
		public static void Serialize<T>(T data, string path)
		{
			var formatter = new BinaryFormatter();
			var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
			formatter.Serialize(stream, data);
			stream.Close();
		}

		public static bool Deserialize<T>(string path, ref T data)
		{
			try
			{
				var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
				var formatter = new BinaryFormatter();
				data = (T)formatter.Deserialize(stream);
				stream.Close();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
