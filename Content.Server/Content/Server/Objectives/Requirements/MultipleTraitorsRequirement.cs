using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Objectives.Requirements
{
	// Token: 0x020002F6 RID: 758
	[DataDefinition]
	public sealed class MultipleTraitorsRequirement : IObjectiveRequirement
	{
		// Token: 0x06000F95 RID: 3989 RVA: 0x00050207 File Offset: 0x0004E407
		[NullableContext(1)]
		public bool CanBeAssigned(Mind mind)
		{
			return EntitySystem.Get<TraitorRuleSystem>().TotalTraitors >= this._requiredTraitors;
		}

		// Token: 0x04000924 RID: 2340
		[DataField("traitors", false, 1, false, false, null)]
		private readonly int _requiredTraitors = 2;
	}
}
