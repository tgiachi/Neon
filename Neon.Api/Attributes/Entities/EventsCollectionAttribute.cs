using System;

namespace Neon.Api.Attributes.Entities
{
	[AttributeUsage(AttributeTargets.Class)]
	public class EventsCollectionAttribute : Attribute
	{
		public string CollectionName { get; set; }

		public EventsCollectionAttribute(string collectionName)
		{
			CollectionName = collectionName;
		}
	}
}
