﻿using System.Collections.Generic;

namespace Neon.Api.Data.UserInteraction
{
	public class UserInteractionData
	{
		public UserInteractionData()
		{
			Fields = new List<UserInteractionField>();
		}

		public string Name { get; set; }

		public List<UserInteractionField> Fields { get; set; }

		public void AddField(UserInteractionField field)
		{
			Fields.Add(field);
		}
	}
}
