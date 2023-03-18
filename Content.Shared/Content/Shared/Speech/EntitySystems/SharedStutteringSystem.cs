using System;
using System.Runtime.CompilerServices;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;

namespace Content.Shared.Speech.EntitySystems
{
	// Token: 0x02000181 RID: 385
	public abstract class SharedStutteringSystem : EntitySystem
	{
		// Token: 0x060004A4 RID: 1188 RVA: 0x0001210D File Offset: 0x0001030D
		[NullableContext(2)]
		public virtual void DoStutter(EntityUid uid, TimeSpan time, bool refresh, StatusEffectsComponent status = null)
		{
		}
	}
}
