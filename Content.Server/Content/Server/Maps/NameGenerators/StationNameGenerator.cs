using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Maps.NameGenerators
{
	// Token: 0x020003DC RID: 988
	[ImplicitDataDefinitionForInheritors]
	public abstract class StationNameGenerator
	{
		// Token: 0x06001454 RID: 5204
		[NullableContext(1)]
		public abstract string FormatName(string input);
	}
}
