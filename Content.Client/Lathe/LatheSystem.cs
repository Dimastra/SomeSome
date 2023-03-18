using System;
using System.Runtime.CompilerServices;
using Content.Client.Power;
using Content.Shared.Lathe;
using Content.Shared.Power;
using Content.Shared.Research.Prototypes;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Lathe
{
	// Token: 0x0200027D RID: 637
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LatheSystem : SharedLatheSystem
	{
		// Token: 0x06001046 RID: 4166 RVA: 0x000612FC File Offset: 0x0005F4FC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LatheComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<LatheComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x00061318 File Offset: 0x0005F518
		private void OnAppearanceChange(EntityUid uid, LatheComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			int num;
			if (this._appearance.TryGetData<bool>(uid, PowerDeviceVisuals.Powered, ref flag, args.Component) && args.Sprite.LayerMapTryGet(PowerDeviceVisualLayers.Powered, ref num, false))
			{
				args.Sprite.LayerSetVisible(PowerDeviceVisualLayers.Powered, flag);
			}
			bool flag2;
			if (this._appearance.TryGetData<bool>(uid, LatheVisuals.IsRunning, ref flag2, args.Component))
			{
				string text = flag2 ? component.RunningState : component.IdleState;
				args.Sprite.LayerSetAnimationTime(LatheVisualLayers.IsRunning, 0f);
				args.Sprite.LayerSetState(LatheVisualLayers.IsRunning, text);
			}
		}

		// Token: 0x06001048 RID: 4168 RVA: 0x00003C56 File Offset: 0x00001E56
		protected override bool HasRecipe(EntityUid uid, LatheRecipePrototype recipe, LatheComponent component)
		{
			return true;
		}

		// Token: 0x04000806 RID: 2054
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
