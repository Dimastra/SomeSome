using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.UserInterface
{
	// Token: 0x020000F9 RID: 249
	[RegisterComponent]
	public sealed class IntrinsicUIComponent : Component, ISerializationHooks
	{
		// Token: 0x06000491 RID: 1169 RVA: 0x00015F0C File Offset: 0x0001410C
		void ISerializationHooks.AfterDeserialization()
		{
			for (int i = 0; i < this.UIs.Count; i++)
			{
				IntrinsicUIEntry ui = this.UIs[i];
				ui.AfterDeserialization();
				this.UIs[i] = ui;
			}
		}

		// Token: 0x040002AE RID: 686
		[Nullable(1)]
		[DataField("uis", false, 1, true, false, null)]
		public List<IntrinsicUIEntry> UIs = new List<IntrinsicUIEntry>();
	}
}
