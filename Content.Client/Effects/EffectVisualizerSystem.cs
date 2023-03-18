using System;
using System.Runtime.CompilerServices;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Effects
{
	// Token: 0x02000334 RID: 820
	public sealed class EffectVisualizerSystem : EntitySystem
	{
		// Token: 0x0600147D RID: 5245 RVA: 0x00078422 File Offset: 0x00076622
		public override void Initialize()
		{
			base.SubscribeLocalEvent<EffectVisualsComponent, AnimationCompletedEvent>(new ComponentEventHandler<EffectVisualsComponent, AnimationCompletedEvent>(this.OnEffectAnimComplete), null, null);
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x00078438 File Offset: 0x00076638
		[NullableContext(1)]
		private void OnEffectAnimComplete(EntityUid uid, EffectVisualsComponent component, AnimationCompletedEvent args)
		{
			base.QueueDel(uid);
		}
	}
}
