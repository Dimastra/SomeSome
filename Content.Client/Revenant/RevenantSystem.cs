using System;
using System.Runtime.CompilerServices;
using Content.Shared.Revenant;
using Content.Shared.Revenant.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Revenant
{
	// Token: 0x0200016A RID: 362
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RevenantSystem : EntitySystem
	{
		// Token: 0x0600096C RID: 2412 RVA: 0x000370D6 File Offset: 0x000352D6
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RevenantComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<RevenantComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x000370F4 File Offset: 0x000352F4
		private void OnAppearanceChange(EntityUid uid, RevenantComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			if (this._appearance.TryGetData<bool>(uid, RevenantVisuals.Harvesting, ref flag, args.Component) && flag)
			{
				args.Sprite.LayerSetState(0, component.HarvestingState);
				return;
			}
			bool flag2;
			if (this._appearance.TryGetData<bool>(uid, RevenantVisuals.Stunned, ref flag2, args.Component) && flag2)
			{
				args.Sprite.LayerSetState(0, component.StunnedState);
				return;
			}
			bool flag3;
			if (this._appearance.TryGetData<bool>(uid, RevenantVisuals.Corporeal, ref flag3, args.Component))
			{
				if (flag3)
				{
					args.Sprite.LayerSetState(0, component.CorporealState);
					return;
				}
				args.Sprite.LayerSetState(0, component.State);
			}
		}

		// Token: 0x040004C1 RID: 1217
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
