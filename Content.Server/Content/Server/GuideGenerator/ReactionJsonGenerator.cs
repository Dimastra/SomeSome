using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.GuideGenerator
{
	// Token: 0x0200047C RID: 1148
	public sealed class ReactionJsonGenerator
	{
		// Token: 0x060016EE RID: 5870 RVA: 0x00078F60 File Offset: 0x00077160
		[NullableContext(1)]
		public static void PublishJson(StreamWriter file)
		{
			Dictionary<string, ReactionEntry> reactions = (from x in IoCManager.Resolve<IPrototypeManager>().EnumeratePrototypes<ReactionPrototype>()
			select new ReactionEntry(x)).ToDictionary((ReactionEntry x) => x.Id, (ReactionEntry x) => x);
			JsonSerializerOptions serializeOptions = new JsonSerializerOptions
			{
				WriteIndented = true,
				Converters = 
				{
					new UniversalJsonConverter<ReagentEffect>()
				}
			};
			file.Write(JsonSerializer.Serialize<Dictionary<string, ReactionEntry>>(reactions, serializeOptions));
		}
	}
}
