using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee
{
	// Token: 0x0200006C RID: 108
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class DamageEffectEvent : EntityEventArgs
	{
		// Token: 0x06000143 RID: 323 RVA: 0x000070E7 File Offset: 0x000052E7
		public DamageEffectEvent(Color color, List<EntityUid> entities)
		{
			this.Color = color;
			this.Entities = entities;
		}

		// Token: 0x04000158 RID: 344
		public Color Color;

		// Token: 0x04000159 RID: 345
		public List<EntityUid> Entities;
	}
}
