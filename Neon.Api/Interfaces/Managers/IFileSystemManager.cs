namespace Neon.Api.Interfaces.Managers
{
	public interface IFileSystemManager
	{
		void CreateDirectory(string directory);

		void Start();

		void Stop();

		bool IsFileExists(string filename);

		bool WriteToFile<T>(string filename, T obj);

		bool WriteToFile(string filename, string context);

		T ReadFromFile<T>(string filename);

		string ReadFromFile(string filename);
	}
}
