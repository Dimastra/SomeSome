using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Magic.Events
{
	// Token: 0x020003EB RID: 1003
	public sealed class SmiteSpellEvent : EntityTargetActionEvent
	{
		// Token: 0x04000CBA RID: 3258
		[DataField("deleteNonBrainParts", false, 1, false, false, null)]
		public bool DeleteNonBrainParts = true;
	}
}
