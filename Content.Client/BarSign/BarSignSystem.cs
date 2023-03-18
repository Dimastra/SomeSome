using System;
using System.Runtime.CompilerServices;
using Content.Shared.BarSign;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.BarSign
{
	// Token: 0x02000425 RID: 1061
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class BarSignSystem : VisualizerSystem<BarSignComponent>
	{
		// Token: 0x060019E4 RID: 6628 RVA: 0x00094564 File Offset: 0x00092764
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BarSignComponent, ComponentHandleState>(new ComponentEventRefHandler<BarSignComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x00094580 File Offset: 0x00092780
		private void OnHandleState(EntityUid uid, BarSignComponent component, ref ComponentHandleState args)
		{
			BarSignComponentState barSignComponentState = args.Current as BarSignComponentState;
			if (barSignComponentState == null)
			{
				return;
			}
			component.CurrentSign = barSignComponentState.CurrentSign;
			this.UpdateAppearance(component, null, null);
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x000945B2 File Offset: 0x000927B2
		protected override void OnAppearanceChange(EntityUid uid, BarSignComponent component, ref AppearanceChangeEvent args)
		{
			this.UpdateAppearance(component, args.Component, args.Sprite);
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x000945C8 File Offset: 0x000927C8
		[NullableContext(2)]
		private void UpdateAppearance([Nullable(1)] BarSignComponent sign, AppearanceComponent appearance = null, SpriteComponent sprite = null)
		{
			if (!base.Resolve<AppearanceComponent, SpriteComponent>(sign.Owner, ref appearance, ref sprite, true))
			{
				return;
			}
			bool flag;
			this.AppearanceSystem.TryGetData<bool>(sign.Owner, PowerDeviceVisuals.Powered, ref flag, appearance);
			BarSignPrototype barSignPrototype;
			if (flag && sign.CurrentSign != null && this._prototypeManager.TryIndex<BarSignPrototype>(sign.CurrentSign, ref barSignPrototype))
			{
				sprite.LayerSetState(0, barSignPrototype.Icon);
				sprite.LayerSetShader(0, "unshaded");
				return;
			}
			sprite.LayerSetState(0, "empty");
			sprite.LayerSetShader(0, null, null);
		}

		// Token: 0x04000D1F RID: 3359
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;
	}
}
