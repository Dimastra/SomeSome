using System;
using System.Runtime.CompilerServices;
using Content.Shared.Light;
using Content.Shared.PDA;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.PDA
{
	// Token: 0x020001C4 RID: 452
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PDASystem : SharedPDASystem
	{
		// Token: 0x06000BE0 RID: 3040 RVA: 0x000449DB File Offset: 0x00042BDB
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PDAComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<PDAComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x000449F8 File Offset: 0x00042BF8
		private void OnAppearanceChange(EntityUid uid, PDAComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			if (this._appearance.TryGetData<bool>(uid, UnpoweredFlashlightVisuals.LightOn, ref flag, args.Component))
			{
				args.Sprite.LayerSetVisible(PDAVisualLayers.Flashlight, flag);
			}
			bool flag2;
			if (this._appearance.TryGetData<bool>(uid, PDAVisuals.IDCardInserted, ref flag2, args.Component))
			{
				args.Sprite.LayerSetVisible(PDAVisualLayers.IDLight, flag2);
			}
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x00044A6C File Offset: 0x00042C6C
		protected override void OnComponentInit(EntityUid uid, PDAComponent component, ComponentInit args)
		{
			base.OnComponentInit(uid, component, args);
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			if (component.State != null)
			{
				spriteComponent.LayerSetState(PDAVisualLayers.Base, component.State);
			}
			spriteComponent.LayerSetVisible(PDAVisualLayers.Flashlight, component.FlashlightOn);
			spriteComponent.LayerSetVisible(PDAVisualLayers.IDLight, component.IdSlot.StartingItem != null);
		}
	}
}
