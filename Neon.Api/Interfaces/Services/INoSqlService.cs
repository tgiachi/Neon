using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Interfaces.Base;
using Neon.Api.Interfaces.NoSql;

namespace Neon.Api.Interfaces.Services
{
	public interface INoSqlService : INeonService
	{
		INoSqlConnector GetNoSqlConnector(string name);
	}
}
