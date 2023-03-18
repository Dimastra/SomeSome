using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Light.Components;
using Content.Server.Storage.Components;
using Content.Shared.Interaction;
using Content.Shared.Light.Component;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Light.EntitySystems
{
	// Token: 0x0200040E RID: 1038
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LightReplacerSystem : EntitySystem
	{
		// Token: 0x06001516 RID: 5398 RVA: 0x0006E7B0 File Offset: 0x0006C9B0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LightReplacerComponent, ComponentInit>(new ComponentEventHandler<LightReplacerComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<LightReplacerComponent, InteractUsingEvent>(new ComponentEventHandler<LightReplacerComponent, InteractUsingEvent>(this.HandleInteract), null, null);
			base.SubscribeLocalEvent<LightReplacerComponent, AfterInteractEvent>(new ComponentEventHandler<LightReplacerComponent, AfterInteractEvent>(this.HandleAfterInteract), null, null);
		}

		// Token: 0x06001517 RID: 5399 RVA: 0x0006E7FF File Offset: 0x0006C9FF
		private void OnInit(EntityUid uid, LightReplacerComponent replacer, ComponentInit args)
		{
			replacer.InsertedBulbs = ContainerHelpers.EnsureContainer<Container>(replacer.Owner, "light_replacer_storage", null);
		}

		// Token: 0x06001518 RID: 5400 RVA: 0x0006E818 File Offset: 0x0006CA18
		private void HandleAfterInteract(EntityUid uid, LightReplacerComponent component, AfterInteractEvent eventArgs)
		{
			if (eventArgs.Handled)
			{
				return;
			}
			if (!eventArgs.CanReach)
			{
				return;
			}
			if (eventArgs.Target != null)
			{
				EntityUid targetUid = eventArgs.Target.Value;
				PoweredLightComponent fixture;
				if (this.EntityManager.TryGetComponent<PoweredLightComponent>(targetUid, ref fixture))
				{
					eventArgs.Handled = this.TryReplaceBulb(uid, targetUid, new EntityUid?(eventArgs.User), component, fixture);
					return;
				}
				LightBulbComponent bulb;
				if (this.EntityManager.TryGetComponent<LightBulbComponent>(targetUid, ref bulb))
				{
					eventArgs.Handled = this.TryInsertBulb(uid, targetUid, new EntityUid?(eventArgs.User), true, component, bulb);
				}
			}
		}

		// Token: 0x06001519 RID: 5401 RVA: 0x0006E8B0 File Offset: 0x0006CAB0
		private void HandleInteract(EntityUid uid, LightReplacerComponent component, InteractUsingEvent eventArgs)
		{
			if (eventArgs.Handled)
			{
				return;
			}
			EntityUid usedUid = eventArgs.Used;
			LightBulbComponent bulb;
			if (this.EntityManager.TryGetComponent<LightBulbComponent>(usedUid, ref bulb))
			{
				eventArgs.Handled = this.TryInsertBulb(uid, usedUid, new EntityUid?(eventArgs.User), true, component, bulb);
				return;
			}
			ServerStorageComponent storage;
			if (this.EntityManager.TryGetComponent<ServerStorageComponent>(usedUid, ref storage))
			{
				eventArgs.Handled = this.TryInsertBulbsFromStorage(uid, usedUid, new EntityUid?(eventArgs.User), component, storage);
			}
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x0006E928 File Offset: 0x0006CB28
		[NullableContext(2)]
		public bool TryReplaceBulb(EntityUid replacerUid, EntityUid fixtureUid, EntityUid? userUid = null, LightReplacerComponent replacer = null, PoweredLightComponent fixture = null)
		{
			if (!base.Resolve<LightReplacerComponent>(replacerUid, ref replacer, true))
			{
				return false;
			}
			if (!base.Resolve<PoweredLightComponent>(fixtureUid, ref fixture, true))
			{
				return false;
			}
			EntityUid? fixtureBulbUid = this._poweredLight.GetBulb(fixture.Owner, fixture);
			if (fixtureBulbUid != null)
			{
				LightBulbComponent fixtureBulb;
				if (!this.EntityManager.TryGetComponent<LightBulbComponent>(fixtureBulbUid.Value, ref fixtureBulb))
				{
					return false;
				}
				if (fixtureBulb.State == LightBulbState.Normal)
				{
					return false;
				}
			}
			EntityUid bulb = replacer.InsertedBulbs.ContainedEntities.FirstOrDefault(delegate(EntityUid e)
			{
				LightBulbComponent componentOrNull = EntityManagerExt.GetComponentOrNull<LightBulbComponent>(this.EntityManager, e);
				LightBulbType? lightBulbType = (componentOrNull != null) ? new LightBulbType?(componentOrNull.Type) : null;
				LightBulbType bulbType = fixture.BulbType;
				return lightBulbType.GetValueOrDefault() == bulbType & lightBulbType != null;
			});
			if (bulb.Valid)
			{
				if (!replacer.InsertedBulbs.Remove(bulb, null, null, null, true, false, null, null))
				{
					return false;
				}
			}
			else
			{
				LightReplacerComponent.LightReplacerEntity bulbEnt = replacer.Contents.FirstOrDefault((LightReplacerComponent.LightReplacerEntity e) => e.Type == fixture.BulbType && e.Amount > 0);
				if (bulbEnt == null)
				{
					if (userUid != null)
					{
						string msg = Loc.GetString("comp-light-replacer-missing-light", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("light-replacer", replacer.Owner)
						});
						this._popupSystem.PopupEntity(msg, replacerUid, userUid.Value, PopupType.Small);
					}
					return false;
				}
				bulb = this.EntityManager.SpawnEntity(bulbEnt.PrototypeName, this.EntityManager.GetComponent<TransformComponent>(replacer.Owner).Coordinates);
				bulbEnt.Amount--;
			}
			bool flag = this._poweredLight.ReplaceBulb(fixtureUid, bulb, fixture);
			if (flag)
			{
				SoundSystem.Play(replacer.Sound.GetSound(null, null), Filter.Pvs(replacerUid, 2f, null, null, null), replacerUid, new AudioParams?(AudioParams.Default.WithVolume(-4f)));
			}
			return flag;
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x0006EAFC File Offset: 0x0006CCFC
		[NullableContext(2)]
		public bool TryInsertBulb(EntityUid replacerUid, EntityUid bulbUid, EntityUid? userUid = null, bool showTooltip = false, LightReplacerComponent replacer = null, LightBulbComponent bulb = null)
		{
			if (!base.Resolve<LightReplacerComponent>(replacerUid, ref replacer, true))
			{
				return false;
			}
			if (!base.Resolve<LightBulbComponent>(bulbUid, ref bulb, true))
			{
				return false;
			}
			if (bulb.State != LightBulbState.Normal)
			{
				if (showTooltip && userUid != null)
				{
					string msg = Loc.GetString("comp-light-replacer-insert-broken-light");
					this._popupSystem.PopupEntity(msg, replacerUid, userUid.Value, PopupType.Small);
				}
				return false;
			}
			bool hasInsert = replacer.InsertedBulbs.Insert(bulb.Owner, null, null, null, null, null);
			if (hasInsert && showTooltip && userUid != null)
			{
				string msg2 = Loc.GetString("comp-light-replacer-insert-light", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("light-replacer", replacer.Owner),
					new ValueTuple<string, object>("bulb", bulb.Owner)
				});
				this._popupSystem.PopupEntity(msg2, replacerUid, userUid.Value, PopupType.Medium);
			}
			return hasInsert;
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x0006EBE8 File Offset: 0x0006CDE8
		[NullableContext(2)]
		public bool TryInsertBulbsFromStorage(EntityUid replacerUid, EntityUid storageUid, EntityUid? userUid = null, LightReplacerComponent replacer = null, ServerStorageComponent storage = null)
		{
			if (!base.Resolve<LightReplacerComponent>(replacerUid, ref replacer, true))
			{
				return false;
			}
			if (!base.Resolve<ServerStorageComponent>(storageUid, ref storage, true))
			{
				return false;
			}
			if (storage.StoredEntities == null)
			{
				return false;
			}
			int insertedBulbs = 0;
			foreach (EntityUid ent in storage.StoredEntities.ToArray<EntityUid>())
			{
				LightBulbComponent bulb;
				if (this.EntityManager.TryGetComponent<LightBulbComponent>(ent, ref bulb) && this.TryInsertBulb(replacerUid, ent, userUid, false, replacer, bulb))
				{
					insertedBulbs++;
				}
			}
			if (insertedBulbs > 0 && userUid != null)
			{
				string msg = Loc.GetString("comp-light-replacer-refill-from-storage", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("light-replacer", storage.Owner)
				});
				this._popupSystem.PopupEntity(msg, replacerUid, userUid.Value, PopupType.Medium);
			}
			return insertedBulbs > 0;
		}

		// Token: 0x04000D0E RID: 3342
		[Dependency]
		private readonly PoweredLightSystem _poweredLight;

		// Token: 0x04000D0F RID: 3343
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;
	}
}
