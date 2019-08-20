namespace Neon.Api.Data.ScriptEngine
{
	public class ScriptEngineVariable
	{
		public string Key { get; set; }

		public object Value { get; set; }

		public override string ToString()
		{
			return $"Key {Key} - Value {Value}";

		}
	}
}
