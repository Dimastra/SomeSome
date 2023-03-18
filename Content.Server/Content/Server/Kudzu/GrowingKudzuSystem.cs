using System;
using System.Runtime.CompilerServices;
using Content.Shared.Kudzu;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Kudzu
{
	// Token: 0x0200042B RID: 1067
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GrowingKudzuSystem : EntitySystem
	{
		// Token: 0x06001593 RID: 5523 RVA: 0x000711C0 File Offset: 0x0006F3C0
		public override void Initialize()
		{
			base.SubscribeLocalEvent<GrowingKudzuComponent, ComponentAdd>(new ComponentEventHandler<GrowingKudzuComponent, ComponentAdd>(this.SetupKudzu), null, null);
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x000711D8 File Offset: 0x0006F3D8
		private void SetupKudzu(EntityUid uid, GrowingKudzuComponent component, ComponentAdd args)
		{
			AppearanceComponent appearance;
			if (!this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, KudzuVisuals.Variant, this._robustRandom.Next(1, 3), appearance);
			this._appearance.SetData(uid, KudzuVisuals.GrowthLevel, 1, appearance);
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x00071234 File Offset: 0x0006F434
		public override void Update(float frameTime)
		{
			this._accumulatedFrameTime += frameTime;
			if (this._accumulatedFrameTime < 0.5f)
			{
				return;
			}
			this._accumulatedFrameTime -= 0.5f;
			foreach (ValueTuple<GrowingKudzuComponent, AppearanceComponent> valueTuple in this.EntityManager.EntityQuery<GrowingKudzuComponent, AppearanceComponent>(false))
			{
				GrowingKudzuComponent kudzu = valueTuple.Item1;
				AppearanceComponent appearance = valueTuple.Item2;
				if (kudzu.GrowthLevel < 3 && RandomExtensions.Prob(this._robustRandom, kudzu.GrowthTickSkipChange))
				{
					kudzu.GrowthLevel++;
					SpreaderComponent spreader;
					if (kudzu.GrowthLevel == 3 && this.EntityManager.TryGetComponent<SpreaderComponent>(kudzu.Owner, ref spreader))
					{
						this.EntityManager.RemoveComponent<GrowingKudzuComponent>(kudzu.Owner);
					}
					this._appearance.SetData(kudzu.Owner, KudzuVisuals.GrowthLevel, kudzu.GrowthLevel, appearance);
				}
			}
		}

		// Token: 0x04000D6A RID: 3434
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000D6B RID: 3435
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000D6C RID: 3436
		private float _accumulatedFrameTime;
	}
}
