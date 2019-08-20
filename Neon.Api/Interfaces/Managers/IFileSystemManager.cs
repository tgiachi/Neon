using System;

namespace Neon.Api.Interfaces.Managers
{
	public interface IFileSystemManager
	{
		void CreateDirectory(string directory);

		string BuildFilePath(string path);
		void Start();

		void Stop();

		bool IsFileExists(string filename);

		bool WriteToFile<T>(string filename, T obj);

		bool WriteToFile(string filename, string context);

		T ReadFromFile<T>(string filename);
		object ReadFromFile(string filename, Type type);

		string ReadFromFile(string filename);
	}
}
