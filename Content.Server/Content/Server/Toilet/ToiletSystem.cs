using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Server.Buckle.Systems;
using Content.Server.Popups;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Content.Shared.Buckle.Components;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Toilet;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Toilet
{
	// Token: 0x0200011C RID: 284
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ToiletSystem : EntitySystem
	{
		// Token: 0x0600051F RID: 1311 RVA: 0x00018D50 File Offset: 0x00016F50
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ToiletComponent, ComponentInit>(new ComponentEventHandler<ToiletComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<ToiletComponent, MapInitEvent>(new ComponentEventHandler<ToiletComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<ToiletComponent, InteractUsingEvent>(new ComponentEventHandler<ToiletComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<ToiletComponent, InteractHandEvent>(new ComponentEventHandler<ToiletComponent, InteractHandEvent>(this.OnInteractHand), new Type[]
			{
				typeof(BuckleSystem)
			}, null);
			base.SubscribeLocalEvent<ToiletComponent, ExaminedEvent>(new ComponentEventHandler<ToiletComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<ToiletComponent, SuicideEvent>(new ComponentEventHandler<ToiletComponent, SuicideEvent>(this.OnSuicide), null, null);
			base.SubscribeLocalEvent<ToiletPryFinished>(new EntityEventHandler<ToiletPryFinished>(this.OnToiletPried), null, null);
			base.SubscribeLocalEvent<ToiletPryInterrupted>(new EntityEventHandler<ToiletPryInterrupted>(this.OnToiletInterrupt), null, null);
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x00018E18 File Offset: 0x00017018
		private void OnSuicide(EntityUid uid, ToiletComponent component, SuicideEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			BodyComponent body;
			if (this.EntityManager.TryGetComponent<BodyComponent>(args.Victim, ref body) && this._bodySystem.BodyHasChildOfType(new EntityUid?(args.Victim), BodyPartType.Head, body))
			{
				string othersMessage = Loc.GetString("toilet-component-suicide-head-message-others", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("victim", Identity.Entity(args.Victim, this.EntityManager)),
					new ValueTuple<string, object>("owner", uid)
				});
				this._popupSystem.PopupEntity(othersMessage, uid, Filter.PvsExcept(args.Victim, 2f, null), true, PopupType.MediumCaution);
				string selfMessage = Loc.GetString("toilet-component-suicide-head-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("owner", uid)
				});
				this._popupSystem.PopupEntity(selfMessage, uid, args.Victim, PopupType.LargeCaution);
				args.SetHandled(SuicideKind.Asphyxiation);
				return;
			}
			string othersMessage2 = Loc.GetString("toilet-component-suicide-message-others", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("victim", Identity.Entity(args.Victim, this.EntityManager)),
				new ValueTuple<string, object>("owner", uid)
			});
			this._popupSystem.PopupEntity(othersMessage2, uid, Filter.PvsExcept(uid, 2f, null), true, PopupType.MediumCaution);
			string selfMessage2 = Loc.GetString("toilet-component-suicide-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("owner", uid)
			});
			this._popupSystem.PopupEntity(selfMessage2, uid, args.Victim, PopupType.LargeCaution);
			args.SetHandled(SuicideKind.Blunt);
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x00018FC6 File Offset: 0x000171C6
		private void OnInit(EntityUid uid, ToiletComponent component, ComponentInit args)
		{
			this.EntityManager.EnsureComponent<SecretStashComponent>(uid);
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x00018FD5 File Offset: 0x000171D5
		private void OnMapInit(EntityUid uid, ToiletComponent component, MapInitEvent args)
		{
			component.IsSeatUp = RandomExtensions.Prob(this._random, 0.5f);
			this.UpdateSprite(uid, component);
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x00018FF8 File Offset: 0x000171F8
		private void OnInteractUsing(EntityUid uid, ToiletComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			ToolComponent tool;
			if (!this.EntityManager.TryGetComponent<ToolComponent>(args.Used, ref tool) || !tool.Qualities.Contains(component.PryingQuality))
			{
				if (component.LidOpen)
				{
					args.Handled = this._secretStash.TryHideItem(uid, args.User, args.Used, null, null, null, null);
				}
				return;
			}
			if (component.IsPrying)
			{
				return;
			}
			component.IsPrying = true;
			ToolEventData toolEvData = new ToolEventData(new ToiletPryFinished(uid), 0f, null, null);
			if (!this._toolSystem.UseTool(args.Used, args.User, new EntityUid?(uid), component.PryLidTime, new string[]
			{
				component.PryingQuality
			}, toolEvData, 0f, null, null, null))
			{
				component.IsPrying = false;
				return;
			}
			args.Handled = true;
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x000190DC File Offset: 0x000172DC
		private void OnInteractHand(EntityUid uid, ToiletComponent component, InteractHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (component.LidOpen && this._secretStash.TryGetItem(uid, args.User, null, null))
			{
				args.Handled = true;
				return;
			}
			StrapComponent strap;
			if (this.EntityManager.TryGetComponent<StrapComponent>(uid, ref strap) && strap.BuckledEntities.Count != 0)
			{
				return;
			}
			this.ToggleToiletSeat(uid, component);
			args.Handled = true;
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x00019148 File Offset: 0x00017348
		private void OnExamine(EntityUid uid, ToiletComponent component, ExaminedEvent args)
		{
			if (args.IsInDetailsRange && component.LidOpen && this._secretStash.HasItemInside(uid, null))
			{
				string msg = Loc.GetString("toilet-component-on-examine-found-hidden-item");
				args.PushMarkup(msg);
			}
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x00019188 File Offset: 0x00017388
		private void OnToiletInterrupt(ToiletPryInterrupted ev)
		{
			ToiletComponent toilet;
			if (!this.EntityManager.TryGetComponent<ToiletComponent>(ev.Uid, ref toilet))
			{
				return;
			}
			toilet.IsPrying = false;
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x000191B4 File Offset: 0x000173B4
		private void OnToiletPried(ToiletPryFinished ev)
		{
			ToiletComponent toilet;
			if (!this.EntityManager.TryGetComponent<ToiletComponent>(ev.Uid, ref toilet))
			{
				return;
			}
			toilet.IsPrying = false;
			toilet.LidOpen = !toilet.LidOpen;
			this.UpdateSprite(ev.Uid, toilet);
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x000191FC File Offset: 0x000173FC
		[NullableContext(2)]
		public void ToggleToiletSeat(EntityUid uid, ToiletComponent component = null)
		{
			if (!base.Resolve<ToiletComponent>(uid, ref component, true))
			{
				return;
			}
			component.IsSeatUp = !component.IsSeatUp;
			this._audio.PlayPvs(component.ToggleSound, uid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
			this.UpdateSprite(uid, component);
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x0001925C File Offset: 0x0001745C
		private void UpdateSprite(EntityUid uid, ToiletComponent component)
		{
			AppearanceComponent appearance;
			if (!this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, ToiletVisuals.LidOpen, component.LidOpen, appearance);
			this._appearance.SetData(uid, ToiletVisuals.SeatUp, component.IsSeatUp, appearance);
		}

		// Token: 0x0400030C RID: 780
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400030D RID: 781
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400030E RID: 782
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x0400030F RID: 783
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000310 RID: 784
		[Dependency]
		private readonly SecretStashSystem _secretStash;

		// Token: 0x04000311 RID: 785
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000312 RID: 786
		[Dependency]
		private readonly SharedToolSystem _toolSystem;
	}
}
