using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;

namespace Content.Client.Actions
{
	// Token: 0x020004F0 RID: 1264
	public sealed class FillActionSlotEvent : EntityEventArgs
	{
		// Token: 0x04000F55 RID: 3925
		[Nullable(2)]
		public ActionType Action;
	}
}
