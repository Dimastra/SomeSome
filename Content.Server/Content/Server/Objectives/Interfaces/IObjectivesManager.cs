using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;

namespace Content.Server.Objectives.Interfaces
{
	// Token: 0x020002FB RID: 763
	[NullableContext(1)]
	public interface IObjectivesManager
	{
		// Token: 0x06000FA2 RID: 4002
		[return: Nullable(2)]
		ObjectivePrototype GetRandomObjective(Mind mind, string objectiveGroupProto);
	}
}
