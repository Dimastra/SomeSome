using System;
using System.Runtime.CompilerServices;
using Content.Shared.Ensnaring;
using Content.Shared.Ensnaring.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Ensnaring.Visualizers
{
	// Token: 0x02000331 RID: 817
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EnsnareableSystem : SharedEnsnareableSystem
	{
		// Token: 0x06001478 RID: 5240 RVA: 0x00078331 File Offset: 0x00076531
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EnsnareableComponent, ComponentInit>(new ComponentEventHandler<EnsnareableComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<EnsnareableComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<EnsnareableComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x00078364 File Offset: 0x00076564
		private void OnComponentInit(EntityUid uid, EnsnareableComponent component, ComponentInit args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			spriteComponent.LayerMapReserveBlank(EnsnaredVisualLayers.Ensnared);
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x0007838C File Offset: 0x0007658C
		private void OnAppearanceChange(EntityUid uid, EnsnareableComponent component, ref AppearanceChangeEvent args)
		{
			int num;
			if (args.Sprite == null || !args.Sprite.LayerMapTryGet(EnsnaredVisualLayers.Ensnared, ref num, false))
			{
				return;
			}
			bool flag;
			if (this._appearance.TryGetData<bool>(uid, EnsnareableVisuals.IsEnsnared, ref flag, args.Component) && component.Sprite != null)
			{
				args.Sprite.LayerSetRSI(num, component.Sprite);
				args.Sprite.LayerSetState(num, component.State);
				args.Sprite.LayerSetVisible(num, flag);
			}
		}

		// Token: 0x04000A79 RID: 2681
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
