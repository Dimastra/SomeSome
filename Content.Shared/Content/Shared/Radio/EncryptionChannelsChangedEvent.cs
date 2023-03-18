using System;
using System.Runtime.CompilerServices;
using Content.Shared.Radio.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Radio
{
	// Token: 0x0200021C RID: 540
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EncryptionChannelsChangedEvent : EntityEventArgs
	{
		// Token: 0x06000602 RID: 1538 RVA: 0x00015264 File Offset: 0x00013464
		public EncryptionChannelsChangedEvent(EncryptionKeyHolderComponent component)
		{
			this.Component = component;
		}

		// Token: 0x04000601 RID: 1537
		public readonly EncryptionKeyHolderComponent Component;
	}
}
