using System;
using System.Runtime.CompilerServices;
using Content.Server.Plants.Components;
using Content.Server.Popups;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Audio;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Plants.Systems
{
	// Token: 0x020002D6 RID: 726
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PottedPlantHideSystem : EntitySystem
	{
		// Token: 0x06000EC0 RID: 3776 RVA: 0x0004AB48 File Offset: 0x00048D48
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PottedPlantHideComponent, ComponentInit>(new ComponentEventHandler<PottedPlantHideComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<PottedPlantHideComponent, InteractUsingEvent>(new ComponentEventHandler<PottedPlantHideComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<PottedPlantHideComponent, InteractHandEvent>(new ComponentEventHandler<PottedPlantHideComponent, InteractHandEvent>(this.OnInteractHand), null, null);
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x0004AB97 File Offset: 0x00048D97
		private void OnInit(EntityUid uid, PottedPlantHideComponent component, ComponentInit args)
		{
			this.EntityManager.EnsureComponent<SecretStashComponent>(uid);
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x0004ABA8 File Offset: 0x00048DA8
		private void OnInteractUsing(EntityUid uid, PottedPlantHideComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.Rustle(uid, component);
			args.Handled = this._stashSystem.TryHideItem(uid, args.User, args.Used, null, null, null, null);
		}

		// Token: 0x06000EC3 RID: 3779 RVA: 0x0004ABE8 File Offset: 0x00048DE8
		private void OnInteractHand(EntityUid uid, PottedPlantHideComponent component, InteractHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.Rustle(uid, component);
			bool gotItem = this._stashSystem.TryGetItem(uid, args.User, null, null);
			if (!gotItem)
			{
				string msg = Loc.GetString("potted-plant-hide-component-interact-hand-got-no-item-message");
				this._popupSystem.PopupEntity(msg, uid, args.User, PopupType.Small);
			}
			args.Handled = gotItem;
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x0004AC44 File Offset: 0x00048E44
		[NullableContext(2)]
		private void Rustle(EntityUid uid, PottedPlantHideComponent component = null)
		{
			if (!base.Resolve<PottedPlantHideComponent>(uid, ref component, true))
			{
				return;
			}
			SoundSystem.Play(component.RustleSound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, new AudioParams?(AudioHelpers.WithVariation(0.25f)));
		}

		// Token: 0x040008AB RID: 2219
		[Dependency]
		private readonly SecretStashSystem _stashSystem;

		// Token: 0x040008AC RID: 2220
		[Dependency]
		private readonly PopupSystem _popupSystem;
	}
}
