using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;

namespace Content.Server.Objectives.Interfaces
{
	// Token: 0x020002FA RID: 762
	[NullableContext(1)]
	public interface IObjectiveRequirement
	{
		// Token: 0x06000FA1 RID: 4001
		bool CanBeAssigned(Mind mind);
	}
}
