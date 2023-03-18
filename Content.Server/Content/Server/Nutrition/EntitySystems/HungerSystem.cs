using System;
using System.Runtime.CompilerServices;
using Content.Server.Nutrition.Components;
using Content.Shared.Rejuvenate;
using Robust.Shared.GameObjects;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x02000310 RID: 784
	public sealed class HungerSystem : EntitySystem
	{
		// Token: 0x0600102D RID: 4141 RVA: 0x000539DA File Offset: 0x00051BDA
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HungerComponent, RejuvenateEvent>(new ComponentEventHandler<HungerComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
		}

		// Token: 0x0600102E RID: 4142 RVA: 0x000539F8 File Offset: 0x00051BF8
		public override void Update(float frameTime)
		{
			this._accumulatedFrameTime += frameTime;
			if (this._accumulatedFrameTime > 1f)
			{
				foreach (HungerComponent hungerComponent in this.EntityManager.EntityQuery<HungerComponent>(false))
				{
					hungerComponent.OnUpdate(this._accumulatedFrameTime);
				}
				this._accumulatedFrameTime -= 1f;
			}
		}

		// Token: 0x0600102F RID: 4143 RVA: 0x00053A7C File Offset: 0x00051C7C
		[NullableContext(1)]
		private void OnRejuvenate(EntityUid uid, HungerComponent component, RejuvenateEvent args)
		{
			component.ResetFood();
		}

		// Token: 0x0400095F RID: 2399
		private float _accumulatedFrameTime;
	}
}
