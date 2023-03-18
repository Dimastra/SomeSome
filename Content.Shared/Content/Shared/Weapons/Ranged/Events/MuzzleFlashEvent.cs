using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Events
{
	// Token: 0x02000055 RID: 85
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class MuzzleFlashEvent : EntityEventArgs
	{
		// Token: 0x0600012A RID: 298 RVA: 0x00006D07 File Offset: 0x00004F07
		public MuzzleFlashEvent(EntityUid uid, string prototype, bool matchRotation = false)
		{
			this.Uid = uid;
			this.Prototype = prototype;
			this.MatchRotation = matchRotation;
		}

		// Token: 0x04000101 RID: 257
		public EntityUid Uid;

		// Token: 0x04000102 RID: 258
		public string Prototype;

		// Token: 0x04000103 RID: 259
		public bool MatchRotation;
	}
}
