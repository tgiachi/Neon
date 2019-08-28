using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace Neon.Api.Utils.Serializer
{
	public class NullStringsAsEmptyEventEmitter : ChainedEventEmitter
	{
		public NullStringsAsEmptyEventEmitter(IEventEmitter nextEmitter)
			: base(nextEmitter)
		{
		}

		public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
		{
			if (eventInfo.Source.Type == typeof(string) && eventInfo.RenderedValue == null)
			{
				emitter.Emit(new Scalar(string.Empty));
			}
			else if (eventInfo.Source.Type == typeof(int) && (int) eventInfo.Source.Value == 0)
			{
				emitter.Emit(new Scalar("0"));
			}
			else
			{
				base.Emit(eventInfo, emitter);
			}
		}
	}
}
