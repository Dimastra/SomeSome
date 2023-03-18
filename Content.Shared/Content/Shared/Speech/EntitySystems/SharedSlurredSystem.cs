using System;
using System.Runtime.CompilerServices;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;

namespace Content.Shared.Speech.EntitySystems
{
	// Token: 0x02000180 RID: 384
	public abstract class SharedSlurredSystem : EntitySystem
	{
		// Token: 0x060004A2 RID: 1186 RVA: 0x00012103 File Offset: 0x00010303
		[NullableContext(2)]
		public virtual void DoSlur(EntityUid uid, TimeSpan time, StatusEffectsComponent status = null)
		{
		}
	}
}
