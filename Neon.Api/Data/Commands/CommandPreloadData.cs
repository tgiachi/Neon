using System;
using System.Collections.Generic;
using System.Reflection;

namespace Neon.Api.Data.Commands
{
	public class CommandPreloadData
	{
		public string CommandName { get; set; }

		public Type SourceType { get; set; }

		public string HelpText { get; set; }

		public string ReturnType { get; set; }

		public bool IsAsync { get; set; }

		public MethodInfo Method { get; set; }

		public List<CommandPreloadParam> Params { get; set; }


		public CommandPreloadData()
		{
			Params = new List<CommandPreloadParam>();
		}
	}
}
