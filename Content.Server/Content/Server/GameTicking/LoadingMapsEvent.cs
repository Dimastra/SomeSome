using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Maps;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking
{
	// Token: 0x020004AC RID: 1196
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LoadingMapsEvent : EntityEventArgs
	{
		// Token: 0x06001867 RID: 6247 RVA: 0x0007F76C File Offset: 0x0007D96C
		public LoadingMapsEvent(List<GameMapPrototype> maps)
		{
			this.Maps = maps;
		}

		// Token: 0x04000F2A RID: 3882
		public List<GameMapPrototype> Maps;
	}
}
