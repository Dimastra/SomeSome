using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Content.Server.Traitor;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Objectives.Requirements
{
	// Token: 0x020002F8 RID: 760
	[DataDefinition]
	public sealed class TraitorRequirement : IObjectiveRequirement
	{
		// Token: 0x06000F99 RID: 3993 RVA: 0x00050267 File Offset: 0x0004E467
		[NullableContext(1)]
		public bool CanBeAssigned(Mind mind)
		{
			return mind.HasRole<TraitorRole>();
		}
	}
}
