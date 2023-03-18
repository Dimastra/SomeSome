using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.Mind;
using Content.Server.Morgue.Components;
using Content.Server.Players;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Morgue;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Morgue
{
	// Token: 0x02000396 RID: 918
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CrematoriumSystem : EntitySystem
	{
		// Token: 0x060012CA RID: 4810 RVA: 0x00061354 File Offset: 0x0005F554
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CrematoriumComponent, ExaminedEvent>(new ComponentEventHandler<CrematoriumComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<CrematoriumComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<CrematoriumComponent, GetVerbsEvent<AlternativeVerb>>(this.AddCremateVerb), null, null);
			base.SubscribeLocalEvent<CrematoriumComponent, SuicideEvent>(new ComponentEventHandler<CrematoriumComponent, SuicideEvent>(this.OnSuicide), null, null);
			base.SubscribeLocalEvent<ActiveCrematoriumComponent, StorageOpenAttemptEvent>(new ComponentEventRefHandler<ActiveCrematoriumComponent, StorageOpenAttemptEvent>(this.OnAttemptOpen), null, null);
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x000613B8 File Offset: 0x0005F5B8
		private void OnExamine(EntityUid uid, CrematoriumComponent component, ExaminedEvent args)
		{
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			bool isBurning;
			if (this._appearance.TryGetData<bool>(uid, CrematoriumVisuals.Burning, ref isBurning, appearance) && isBurning)
			{
				args.PushMarkup(Loc.GetString("crematorium-entity-storage-component-on-examine-details-is-burning", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("owner", uid)
				}));
			}
			bool hasContents;
			if (this._appearance.TryGetData<bool>(uid, StorageVisuals.HasContents, ref hasContents, appearance) && hasContents)
			{
				args.PushMarkup(Loc.GetString("crematorium-entity-storage-component-on-examine-details-has-contents"));
				return;
			}
			args.PushMarkup(Loc.GetString("crematorium-entity-storage-component-on-examine-details-empty"));
		}

		// Token: 0x060012CC RID: 4812 RVA: 0x00061451 File Offset: 0x0005F651
		private void OnAttemptOpen(EntityUid uid, ActiveCrematoriumComponent component, ref StorageOpenAttemptEvent args)
		{
			args.Cancelled = true;
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x0006145C File Offset: 0x0005F65C
		private void AddCremateVerb(EntityUid uid, CrematoriumComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			EntityStorageComponent storage;
			if (!base.TryComp<EntityStorageComponent>(uid, ref storage))
			{
				return;
			}
			if (!args.CanAccess || !args.CanInteract || args.Hands == null || storage.Open)
			{
				return;
			}
			if (base.HasComp<ActiveCrematoriumComponent>(uid))
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Text = Loc.GetString("cremate-verb-get-data-text"),
				Act = delegate()
				{
					this.TryCremate(uid, component, storage);
				},
				Impact = LogImpact.Medium
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x0006150C File Offset: 0x0005F70C
		[NullableContext(2)]
		public bool Cremate(EntityUid uid, CrematoriumComponent component = null, EntityStorageComponent storage = null)
		{
			if (!base.Resolve<CrematoriumComponent, EntityStorageComponent>(uid, ref component, ref storage, true))
			{
				return false;
			}
			if (base.HasComp<ActiveCrematoriumComponent>(uid))
			{
				return false;
			}
			this._audio.PlayPvs(component.CremateStartSound, uid, null);
			this._appearance.SetData(uid, CrematoriumVisuals.Burning, true, null);
			this._audio.PlayPvs(component.CrematingSound, uid, null);
			base.AddComp<ActiveCrematoriumComponent>(uid);
			return true;
		}

		// Token: 0x060012CF RID: 4815 RVA: 0x0006158E File Offset: 0x0005F78E
		[NullableContext(2)]
		public bool TryCremate(EntityUid uid, CrematoriumComponent component = null, EntityStorageComponent storage = null)
		{
			return base.Resolve<CrematoriumComponent, EntityStorageComponent>(uid, ref component, ref storage, true) && !storage.Open && storage.Contents.ContainedEntities.Count >= 1 && this.Cremate(uid, component, storage);
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x000615C8 File Offset: 0x0005F7C8
		private void FinishCooking(EntityUid uid, CrematoriumComponent component, [Nullable(2)] EntityStorageComponent storage = null)
		{
			if (!base.Resolve<EntityStorageComponent>(uid, ref storage, true))
			{
				return;
			}
			this._appearance.SetData(uid, CrematoriumVisuals.Burning, false, null);
			base.RemComp<ActiveCrematoriumComponent>(uid);
			if (storage.Contents.ContainedEntities.Count > 0)
			{
				for (int i = storage.Contents.ContainedEntities.Count - 1; i >= 0; i--)
				{
					EntityUid item = storage.Contents.ContainedEntities[i];
					storage.Contents.Remove(item, null, null, null, true, false, null, null);
					this.EntityManager.DeleteEntity(item);
				}
				EntityUid ash = base.Spawn("Ash", base.Transform(uid).Coordinates);
				storage.Contents.Insert(ash, null, null, null, null, null);
			}
			this._entityStorage.OpenStorage(uid, storage);
			this._audio.PlayPvs(component.CremateFinishSound, uid, null);
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x000616D0 File Offset: 0x0005F8D0
		private void OnSuicide(EntityUid uid, CrematoriumComponent component, SuicideEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.SetHandled(SuicideKind.Heat);
			EntityUid victim = args.Victim;
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(victim, ref actor))
			{
				PlayerData playerData = actor.PlayerSession.ContentData();
				Mind mind = (playerData != null) ? playerData.Mind : null;
				if (mind != null)
				{
					this._ticker.OnGhostAttempt(mind, false, false);
					EntityUid? ownedEntity = mind.OwnedEntity;
					if (ownedEntity != null)
					{
						EntityUid entity = ownedEntity.GetValueOrDefault();
						if (entity.Valid)
						{
							this._popup.PopupEntity(Loc.GetString("crematorium-entity-storage-component-suicide-message"), entity, PopupType.Small);
						}
					}
				}
			}
			this._popup.PopupEntity(Loc.GetString("crematorium-entity-storage-component-suicide-message-others", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("victim", Identity.Entity(victim, this.EntityManager))
			}), victim, Filter.PvsExcept(victim, 2f, null), true, PopupType.LargeCaution);
			if (this._entityStorage.CanInsert(uid, null))
			{
				this._entityStorage.CloseStorage(uid, null);
				this._standing.Down(victim, false, true, null, null, null);
				this._entityStorage.Insert(victim, uid, null);
			}
			else
			{
				this.EntityManager.DeleteEntity(victim);
			}
			this._entityStorage.CloseStorage(uid, null);
			this.Cremate(uid, component, null);
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x00061810 File Offset: 0x0005FA10
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<ActiveCrematoriumComponent, CrematoriumComponent> valueTuple in base.EntityQuery<ActiveCrematoriumComponent, CrematoriumComponent>(false))
			{
				ActiveCrematoriumComponent act = valueTuple.Item1;
				CrematoriumComponent crem = valueTuple.Item2;
				act.Accumulator += frameTime;
				if (act.Accumulator >= (float)crem.CookTime)
				{
					this.FinishCooking(act.Owner, crem, null);
				}
			}
		}

		// Token: 0x04000B7D RID: 2941
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000B7E RID: 2942
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000B7F RID: 2943
		[Dependency]
		private readonly GameTicker _ticker;

		// Token: 0x04000B80 RID: 2944
		[Dependency]
		private readonly EntityStorageSystem _entityStorage;

		// Token: 0x04000B81 RID: 2945
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x04000B82 RID: 2946
		[Dependency]
		private readonly StandingStateSystem _standing;
	}
}
