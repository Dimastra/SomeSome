using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Maps
{
	// Token: 0x0200033A RID: 826
	public abstract class SharedGridDraggingSystem : EntitySystem
	{
		// Token: 0x0400097B RID: 2427
		[Nullable(1)]
		public const string CommandName = "griddrag";
	}
}
