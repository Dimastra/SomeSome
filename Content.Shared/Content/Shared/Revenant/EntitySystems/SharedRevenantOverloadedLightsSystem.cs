using System;
using System.Runtime.CompilerServices;
using Content.Shared.Revenant.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Revenant.EntitySystems
{
	// Token: 0x020001F7 RID: 503
	public abstract class SharedRevenantOverloadedLightsSystem : EntitySystem
	{
		// Token: 0x06000597 RID: 1431 RVA: 0x000144C8 File Offset: 0x000126C8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			RevenantOverloadedLightsComponent comp;
			while (base.EntityQueryEnumerator<RevenantOverloadedLightsComponent>().MoveNext(ref comp))
			{
				comp.Accumulator += frameTime;
				if (comp.Accumulator >= comp.ZapDelay)
				{
					this.OnZap(comp);
					base.RemComp(comp.Owner, comp);
				}
			}
		}

		// Token: 0x06000598 RID: 1432
		[NullableContext(1)]
		protected abstract void OnZap(RevenantOverloadedLightsComponent component);
	}
}
