using System;
using System.Runtime.CompilerServices;
using Content.Shared.Emag.Systems;
using Content.Shared.Medical.Cryogenics;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Medical.Cryogenics
{
	// Token: 0x02000235 RID: 565
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CryoPodSystem : SharedCryoPodSystem
	{
		// Token: 0x06000E71 RID: 3697 RVA: 0x00057208 File Offset: 0x00055408
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CryoPodComponent, ComponentInit>(new ComponentEventHandler<CryoPodComponent, ComponentInit>(base.OnComponentInit), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<CryoPodComponent, GetVerbsEvent<AlternativeVerb>>(base.AddAlternativeVerbs), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, GotEmaggedEvent>(new ComponentEventRefHandler<CryoPodComponent, GotEmaggedEvent>(base.OnEmagged), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, SharedCryoPodSystem.CryoPodPryFinished>(new ComponentEventHandler<CryoPodComponent, SharedCryoPodSystem.CryoPodPryFinished>(base.OnCryoPodPryFinished), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, SharedCryoPodSystem.CryoPodPryInterrupted>(new ComponentEventHandler<CryoPodComponent, SharedCryoPodSystem.CryoPodPryInterrupted>(base.OnCryoPodPryInterrupted), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<CryoPodComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
			base.SubscribeLocalEvent<InsideCryoPodComponent, ComponentStartup>(new ComponentEventHandler<InsideCryoPodComponent, ComponentStartup>(this.OnCryoPodInsertion), null, null);
			base.SubscribeLocalEvent<InsideCryoPodComponent, ComponentRemove>(new ComponentEventHandler<InsideCryoPodComponent, ComponentRemove>(this.OnCryoPodRemoval), null, null);
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x000572BC File Offset: 0x000554BC
		private void OnCryoPodInsertion(EntityUid uid, InsideCryoPodComponent component, ComponentStartup args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			component.PreviousOffset = spriteComponent.Offset;
			spriteComponent.Offset = new Vector2(0f, 1f);
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x000572F8 File Offset: 0x000554F8
		private void OnCryoPodRemoval(EntityUid uid, InsideCryoPodComponent component, ComponentRemove args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			spriteComponent.Offset = component.PreviousOffset;
		}

		// Token: 0x06000E74 RID: 3700 RVA: 0x00057320 File Offset: 0x00055520
		private void OnAppearanceChange(EntityUid uid, SharedCryoPodComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			bool flag2;
			if (!this._appearance.TryGetData<bool>(uid, SharedCryoPodComponent.CryoPodVisuals.ContainsEntity, ref flag, args.Component) || !this._appearance.TryGetData<bool>(uid, SharedCryoPodComponent.CryoPodVisuals.IsOn, ref flag2, args.Component))
			{
				return;
			}
			if (flag)
			{
				args.Sprite.LayerSetState(CryoPodVisualLayers.Base, "pod-open");
				args.Sprite.LayerSetVisible(CryoPodVisualLayers.Cover, false);
				args.Sprite.DrawDepth = 0;
				return;
			}
			args.Sprite.DrawDepth = 4;
			args.Sprite.LayerSetState(CryoPodVisualLayers.Base, flag2 ? "pod-on" : "pod-off");
			args.Sprite.LayerSetState(CryoPodVisualLayers.Cover, flag2 ? "cover-on" : "cover-off");
			args.Sprite.LayerSetVisible(CryoPodVisualLayers.Cover, true);
		}

		// Token: 0x0400072F RID: 1839
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
