using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Items
{
	// Token: 0x0200029A RID: 666
	public sealed class ItemStatusCollectMessage : EntityEventArgs
	{
		// Token: 0x0400084C RID: 2124
		[Nullable(1)]
		public List<Control> Controls = new List<Control>();
	}
}
