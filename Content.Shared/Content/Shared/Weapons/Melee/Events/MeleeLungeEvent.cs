using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events
{
	// Token: 0x02000077 RID: 119
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class MeleeLungeEvent : EntityEventArgs
	{
		// Token: 0x06000176 RID: 374 RVA: 0x00008A54 File Offset: 0x00006C54
		public MeleeLungeEvent(EntityUid uid, Angle angle, Vector2 localPos, string animation)
		{
			this.Entity = uid;
			this.Angle = angle;
			this.LocalPos = localPos;
			this.Animation = animation;
		}

		// Token: 0x04000195 RID: 405
		public EntityUid Entity;

		// Token: 0x04000196 RID: 406
		public Angle Angle;

		// Token: 0x04000197 RID: 407
		public Vector2 LocalPos;

		// Token: 0x04000198 RID: 408
		public string Animation;
	}
}
