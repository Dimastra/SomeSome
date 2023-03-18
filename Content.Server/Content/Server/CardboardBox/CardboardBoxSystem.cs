using System;
using System.Runtime.CompilerServices;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Shared.CardboardBox;
using Content.Shared.CardboardBox.Components;
using Content.Shared.Damage;
using Content.Shared.Interaction;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Stealth;
using Content.Shared.Stealth.Components;
using Content.Shared.Storage.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server.CardboardBox
{
	// Token: 0x020006F0 RID: 1776
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CardboardBoxSystem : SharedCardboardBoxSystem
	{
		// Token: 0x0600251A RID: 9498 RVA: 0x000C211C File Offset: 0x000C031C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CardboardBoxComponent, StorageAfterOpenEvent>(new ComponentEventRefHandler<CardboardBoxComponent, StorageAfterOpenEvent>(this.AfterStorageOpen), null, null);
			base.SubscribeLocalEvent<CardboardBoxComponent, StorageAfterCloseEvent>(new ComponentEventRefHandler<CardboardBoxComponent, StorageAfterCloseEvent>(this.AfterStorageClosed), null, null);
			base.SubscribeLocalEvent<CardboardBoxComponent, InteractedNoHandEvent>(new ComponentEventHandler<CardboardBoxComponent, InteractedNoHandEvent>(this.OnNoHandInteracted), null, null);
			base.SubscribeLocalEvent<CardboardBoxComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<CardboardBoxComponent, EntInsertedIntoContainerMessage>(this.OnEntInserted), null, null);
			base.SubscribeLocalEvent<CardboardBoxComponent, DamageChangedEvent>(new ComponentEventHandler<CardboardBoxComponent, DamageChangedEvent>(this.OnDamage), null, null);
		}

		// Token: 0x0600251B RID: 9499 RVA: 0x000C2194 File Offset: 0x000C0394
		private void OnNoHandInteracted(EntityUid uid, CardboardBoxComponent component, InteractedNoHandEvent args)
		{
			EntityStorageComponent box;
			if (!base.TryComp<EntityStorageComponent>(uid, ref box) || box.Open || !box.Contents.Contains(args.User))
			{
				return;
			}
			this._storage.OpenStorage(uid, null);
		}

		// Token: 0x0600251C RID: 9500 RVA: 0x000C21D8 File Offset: 0x000C03D8
		private void AfterStorageOpen(EntityUid uid, CardboardBoxComponent component, ref StorageAfterOpenEvent args)
		{
			if (component.Mover != null)
			{
				base.RemComp<RelayInputMoverComponent>(component.Mover.Value);
				if (this._timing.CurTime > component.EffectCooldown)
				{
					base.RaiseNetworkEvent(new PlayBoxEffectMessage(component.Owner, component.Mover.Value), Filter.PvsExcept(component.Owner, 2f, null), true);
					this._audio.PlayPvs(component.EffectSound, component.Owner, null);
					component.EffectCooldown = this._timing.CurTime + CardboardBoxComponent.MaxEffectCooldown;
				}
			}
			component.Mover = null;
			this._stealth.SetEnabled(uid, false, null);
		}

		// Token: 0x0600251D RID: 9501 RVA: 0x000C22A4 File Offset: 0x000C04A4
		private void AfterStorageClosed(EntityUid uid, CardboardBoxComponent component, ref StorageAfterCloseEvent args)
		{
			StealthComponent stealth;
			if (base.TryComp<StealthComponent>(uid, ref stealth))
			{
				this._stealth.SetVisibility(uid, stealth.MaxVisibility, stealth);
				this._stealth.SetEnabled(uid, true, stealth);
			}
		}

		// Token: 0x0600251E RID: 9502 RVA: 0x000C22DD File Offset: 0x000C04DD
		private void OnDamage(EntityUid uid, CardboardBoxComponent component, DamageChangedEvent args)
		{
			if (args.DamageDelta != null && args.DamageIncreased)
			{
				this._damageable.TryChangeDamage(component.Mover, args.DamageDelta, false, true, null, args.Origin);
			}
		}

		// Token: 0x0600251F RID: 9503 RVA: 0x000C2310 File Offset: 0x000C0510
		private void OnEntInserted(EntityUid uid, CardboardBoxComponent component, EntInsertedIntoContainerMessage args)
		{
			MobMoverComponent mover;
			if (!base.TryComp<MobMoverComponent>(args.Entity, ref mover))
			{
				return;
			}
			if (component.Mover != null)
			{
				if (base.HasComp<ActorComponent>(component.Mover) || !base.HasComp<ActorComponent>(args.Entity))
				{
					return;
				}
				base.RemComp<RelayInputMoverComponent>(component.Mover.Value);
			}
			RelayInputMoverComponent relay = base.EnsureComp<RelayInputMoverComponent>(args.Entity);
			this._mover.SetRelay(args.Entity, uid, relay);
			component.Mover = new EntityUid?(args.Entity);
		}

		// Token: 0x040016CD RID: 5837
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040016CE RID: 5838
		[Dependency]
		private readonly SharedMoverController _mover;

		// Token: 0x040016CF RID: 5839
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040016D0 RID: 5840
		[Dependency]
		private readonly SharedStealthSystem _stealth;

		// Token: 0x040016D1 RID: 5841
		[Dependency]
		private readonly DamageableSystem _damageable;

		// Token: 0x040016D2 RID: 5842
		[Dependency]
		private readonly EntityStorageSystem _storage;
	}
}
