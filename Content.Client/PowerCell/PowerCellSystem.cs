using System;
using System.Runtime.CompilerServices;
using Content.Shared.PowerCell;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.PowerCell
{
	// Token: 0x02000193 RID: 403
	public sealed class PowerCellSystem : SharedPowerCellSystem
	{
		// Token: 0x06000AD1 RID: 2769 RVA: 0x0003EDE0 File Offset: 0x0003CFE0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PowerCellVisualsComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<PowerCellVisualsComponent, AppearanceChangeEvent>(this.OnPowerCellVisualsChange), null, null);
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x0003EDFC File Offset: 0x0003CFFC
		[NullableContext(1)]
		private void OnPowerCellVisualsChange(EntityUid uid, PowerCellVisualsComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			SpriteComponent.Layer layer;
			if (!args.Sprite.TryGetLayer(1, ref layer, false))
			{
				return;
			}
			byte b;
			if (this._appearance.TryGetData<byte>(uid, PowerCellVisuals.ChargeLevel, ref b, args.Component))
			{
				if (b == 0)
				{
					layer.Visible = false;
					return;
				}
				layer.Visible = true;
				SpriteComponent sprite = args.Sprite;
				object obj = PowerCellSystem.PowerCellVisualLayers.Unshaded;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler.AppendLiteral("o");
				defaultInterpolatedStringHandler.AppendFormatted<byte>(b);
				sprite.LayerSetState(obj, defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x0400053F RID: 1343
		[Nullable(1)]
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x02000194 RID: 404
		private enum PowerCellVisualLayers : byte
		{
			// Token: 0x04000541 RID: 1345
			Base,
			// Token: 0x04000542 RID: 1346
			Unshaded
		}
	}
}
