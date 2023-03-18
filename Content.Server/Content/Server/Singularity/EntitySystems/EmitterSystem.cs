using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.Construction;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Projectiles;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.EntitySystems;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Singularity.EntitySystems
{
	// Token: 0x020001E9 RID: 489
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EmitterSystem : SharedEmitterSystem
	{
		// Token: 0x06000947 RID: 2375 RVA: 0x0002ED38 File Offset: 0x0002CF38
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EmitterComponent, PowerConsumerReceivedChanged>(new ComponentEventRefHandler<EmitterComponent, PowerConsumerReceivedChanged>(this.ReceivedChanged), null, null);
			base.SubscribeLocalEvent<EmitterComponent, PowerChangedEvent>(new ComponentEventRefHandler<EmitterComponent, PowerChangedEvent>(this.OnApcChanged), null, null);
			base.SubscribeLocalEvent<EmitterComponent, InteractHandEvent>(new ComponentEventHandler<EmitterComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<EmitterComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<EmitterComponent, GetVerbsEvent<Verb>>(this.OnGetVerb), null, null);
			base.SubscribeLocalEvent<EmitterComponent, ExaminedEvent>(new ComponentEventHandler<EmitterComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<EmitterComponent, RefreshPartsEvent>(new ComponentEventHandler<EmitterComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<EmitterComponent, UpgradeExamineEvent>(new ComponentEventHandler<EmitterComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<EmitterComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<EmitterComponent, AnchorStateChangedEvent>(this.OnAnchorStateChanged), null, null);
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0002EDEB File Offset: 0x0002CFEB
		private void OnAnchorStateChanged(EntityUid uid, EmitterComponent component, ref AnchorStateChangedEvent args)
		{
			if (args.Anchored)
			{
				return;
			}
			this.SwitchOff(component);
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x0002EE00 File Offset: 0x0002D000
		private void OnInteractHand(EntityUid uid, EmitterComponent component, InteractHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			LockComponent lockComp;
			if (this.EntityManager.TryGetComponent<LockComponent>(uid, ref lockComp) && lockComp.Locked)
			{
				this._popup.PopupEntity(Loc.GetString("comp-emitter-access-locked", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", component.Owner)
				}), uid, args.User, PopupType.Small);
				return;
			}
			PhysicsComponent phys;
			if (this.EntityManager.TryGetComponent<PhysicsComponent>(component.Owner, ref phys) && phys.BodyType == 4)
			{
				if (!component.IsOn)
				{
					this.SwitchOn(component);
					this._popup.PopupEntity(Loc.GetString("comp-emitter-turned-on", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("target", component.Owner)
					}), uid, args.User, PopupType.Small);
				}
				else
				{
					this.SwitchOff(component);
					this._popup.PopupEntity(Loc.GetString("comp-emitter-turned-off", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("target", component.Owner)
					}), uid, args.User, PopupType.Small);
				}
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Emitter;
				LogImpact impact = component.IsOn ? LogImpact.Medium : LogImpact.High;
				LogStringHandler logStringHandler = new LogStringHandler(9, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "player", "ToPrettyString(args.User)");
				logStringHandler.AppendLiteral(" toggled ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "emitter", "ToPrettyString(uid)");
				adminLogger.Add(type, impact, ref logStringHandler);
				args.Handled = true;
				return;
			}
			this._popup.PopupEntity(Loc.GetString("comp-emitter-not-anchored", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", component.Owner)
			}), uid, args.User, PopupType.Small);
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x0002EFD8 File Offset: 0x0002D1D8
		private void OnGetVerb(EntityUid uid, EmitterComponent component, GetVerbsEvent<Verb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Hands == null)
			{
				return;
			}
			LockComponent lockComp;
			if (base.TryComp<LockComponent>(uid, ref lockComp) && lockComp.Locked)
			{
				return;
			}
			if (component.SelectableTypes.Count < 2)
			{
				return;
			}
			using (List<string>.Enumerator enumerator = component.SelectableTypes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string type = enumerator.Current;
					EntityPrototype proto = this._prototype.Index<EntityPrototype>(type);
					Verb v = new Verb
					{
						Priority = 1,
						Category = VerbCategory.SelectType,
						Text = proto.Name,
						Disabled = (type == component.BoltType),
						Impact = LogImpact.Medium,
						DoContactInteraction = new bool?(true),
						Act = delegate()
						{
							component.BoltType = type;
							this._popup.PopupEntity(Loc.GetString("emitter-component-type-set", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("type", proto.Name)
							}), uid, PopupType.Small);
						}
					};
					args.Verbs.Add(v);
				}
			}
		}

		// Token: 0x0600094B RID: 2379 RVA: 0x0002F138 File Offset: 0x0002D338
		private void OnExamined(EntityUid uid, EmitterComponent component, ExaminedEvent args)
		{
			if (component.SelectableTypes.Count < 2)
			{
				return;
			}
			EntityPrototype proto = this._prototype.Index<EntityPrototype>(component.BoltType);
			args.PushMarkup(Loc.GetString("emitter-component-current-type", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("type", proto.Name)
			}));
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x0002F193 File Offset: 0x0002D393
		private void ReceivedChanged(EntityUid uid, EmitterComponent component, ref PowerConsumerReceivedChanged args)
		{
			if (!component.IsOn)
			{
				return;
			}
			if (args.ReceivedPower < args.DrawRate)
			{
				this.PowerOff(component);
				return;
			}
			this.PowerOn(component);
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x0002F1BB File Offset: 0x0002D3BB
		private void OnApcChanged(EntityUid uid, EmitterComponent component, ref PowerChangedEvent args)
		{
			if (!component.IsOn)
			{
				return;
			}
			if (!args.Powered)
			{
				this.PowerOff(component);
				return;
			}
			this.PowerOn(component);
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0002F1E0 File Offset: 0x0002D3E0
		private void OnRefreshParts(EntityUid uid, EmitterComponent component, RefreshPartsEvent args)
		{
			float powerUseRating = args.PartRatings[component.MachinePartPowerUse];
			float fireRateRating = args.PartRatings[component.MachinePartFireRate];
			component.PowerUseActive = (int)((float)component.BasePowerUseActive * MathF.Pow(component.PowerUseMultiplier, powerUseRating - 1f));
			component.FireInterval = component.BaseFireInterval * (double)MathF.Pow(component.FireRateMultiplier, fireRateRating - 1f);
			component.FireBurstDelayMin = component.BaseFireBurstDelayMin * (double)MathF.Pow(component.FireRateMultiplier, fireRateRating - 1f);
			component.FireBurstDelayMax = component.BaseFireBurstDelayMax * (double)MathF.Pow(component.FireRateMultiplier, fireRateRating - 1f);
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x0002F29E File Offset: 0x0002D49E
		private void OnUpgradeExamine(EntityUid uid, EmitterComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("emitter-component-upgrade-fire-rate", (float)(component.BaseFireInterval.TotalSeconds / component.FireInterval.TotalSeconds));
			args.AddPercentageUpgrade("upgrade-power-draw", (float)component.PowerUseActive / (float)component.BasePowerUseActive);
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x0002F2E0 File Offset: 0x0002D4E0
		public void SwitchOff(EmitterComponent component)
		{
			component.IsOn = false;
			PowerConsumerComponent powerConsumer;
			if (base.TryComp<PowerConsumerComponent>(component.Owner, ref powerConsumer))
			{
				powerConsumer.DrawRate = 1f;
			}
			ApcPowerReceiverComponent apcReceiever;
			if (base.TryComp<ApcPowerReceiverComponent>(component.Owner, ref apcReceiever))
			{
				apcReceiever.Load = 1f;
			}
			this.PowerOff(component);
			this.UpdateAppearance(component);
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x0002F338 File Offset: 0x0002D538
		public void SwitchOn(EmitterComponent component)
		{
			component.IsOn = true;
			PowerConsumerComponent powerConsumer;
			if (base.TryComp<PowerConsumerComponent>(component.Owner, ref powerConsumer))
			{
				powerConsumer.DrawRate = (float)component.PowerUseActive;
			}
			ApcPowerReceiverComponent apcReceiever;
			if (base.TryComp<ApcPowerReceiverComponent>(component.Owner, ref apcReceiever))
			{
				apcReceiever.Load = (float)component.PowerUseActive;
				this.PowerOn(component);
			}
			this.UpdateAppearance(component);
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x0002F394 File Offset: 0x0002D594
		public void PowerOff(EmitterComponent component)
		{
			if (!component.IsPowered)
			{
				return;
			}
			component.IsPowered = false;
			CancellationTokenSource timerCancel = component.TimerCancel;
			if (timerCancel != null)
			{
				timerCancel.Cancel();
			}
			this.UpdateAppearance(component);
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x0002F3C0 File Offset: 0x0002D5C0
		public void PowerOn(EmitterComponent component)
		{
			if (component.IsPowered)
			{
				return;
			}
			component.IsPowered = true;
			component.FireShotCounter = 0;
			component.TimerCancel = new CancellationTokenSource();
			Timer.Spawn(component.FireBurstDelayMax, delegate()
			{
				this.ShotTimerCallback(component);
			}, component.TimerCancel.Token);
			this.UpdateAppearance(component);
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x0002F450 File Offset: 0x0002D650
		private void ShotTimerCallback(EmitterComponent component)
		{
			if (component.Deleted)
			{
				return;
			}
			this.Fire(component);
			TimeSpan delay;
			if (component.FireShotCounter < component.FireBurstSize)
			{
				component.FireShotCounter++;
				delay = component.FireInterval;
			}
			else
			{
				component.FireShotCounter = 0;
				TimeSpan diff = component.FireBurstDelayMax - component.FireBurstDelayMin;
				delay = component.FireBurstDelayMin + (double)this._random.NextFloat() * diff;
			}
			Timer.Spawn(delay, delegate()
			{
				this.ShotTimerCallback(component);
			}, component.TimerCancel.Token);
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x0002F534 File Offset: 0x0002D734
		private void Fire(EmitterComponent component)
		{
			EntityUid uid = component.Owner;
			GunComponent guncomp;
			if (!base.TryComp<GunComponent>(uid, ref guncomp))
			{
				return;
			}
			TransformComponent xform = base.Transform(uid);
			EntityUid ent = base.Spawn(component.BoltType, xform.Coordinates);
			ProjectileComponent proj = base.EnsureComp<ProjectileComponent>(ent);
			this._projectile.SetShooter(proj, uid);
			EntityCoordinates targetPos;
			targetPos..ctor(uid, new ValueTuple<float, float>(0f, -1f));
			this._gun.Shoot(guncomp, ent, xform.Coordinates, targetPos, null);
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x0002F5C4 File Offset: 0x0002D7C4
		private void UpdateAppearance(EmitterComponent component)
		{
			EmitterVisualState state;
			if (component.IsPowered)
			{
				state = EmitterVisualState.On;
			}
			else if (component.IsOn)
			{
				state = EmitterVisualState.Underpowered;
			}
			else
			{
				state = EmitterVisualState.Off;
			}
			this._appearance.SetData(component.Owner, EmitterVisuals.VisualState, state, null);
		}

		// Token: 0x0400059F RID: 1439
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040005A0 RID: 1440
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x040005A1 RID: 1441
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040005A2 RID: 1442
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x040005A3 RID: 1443
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x040005A4 RID: 1444
		[Dependency]
		private readonly ProjectileSystem _projectile;

		// Token: 0x040005A5 RID: 1445
		[Dependency]
		private readonly GunSystem _gun;
	}
}
