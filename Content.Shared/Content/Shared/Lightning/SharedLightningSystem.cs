using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared.Lightning
{
	// Token: 0x0200035F RID: 863
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedLightningSystem : EntitySystem
	{
		// Token: 0x06000A2D RID: 2605 RVA: 0x00021F10 File Offset: 0x00020110
		public string LightningRandomizer()
		{
			return "lightning_" + this._random.Next(1, 12).ToString();
		}

		// Token: 0x040009D9 RID: 2521
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
