using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003D4 RID: 980
	public sealed class ContactInteractionEvent : HandledEntityEventArgs
	{
		// Token: 0x06000B7C RID: 2940 RVA: 0x000261DF File Offset: 0x000243DF
		public ContactInteractionEvent(EntityUid other)
		{
			this.Other = other;
		}

		// Token: 0x04000B37 RID: 2871
		public readonly EntityUid Other;
	}
}
