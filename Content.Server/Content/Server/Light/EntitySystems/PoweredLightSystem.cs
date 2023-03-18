using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.DoAfter;
using Content.Server.Ghost;
using Content.Server.Light.Components;
using Content.Server.MachineLinking.Events;
using Content.Server.MachineLinking.System;
using Content.Server.Power.Components;
using Content.Server.Temperature.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Audio;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Light;
using Content.Shared.Light.Component;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server.Light.EntitySystems
{
	// Token: 0x02000412 RID: 1042
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PoweredLightSystem : EntitySystem
	{
		// Token: 0x0600152D RID: 5421 RVA: 0x0006F114 File Offset: 0x0006D314
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PoweredLightComponent, ComponentInit>(new ComponentEventHandler<PoweredLightComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<PoweredLightComponent, MapInitEvent>(new ComponentEventHandler<PoweredLightComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<PoweredLightComponent, InteractUsingEvent>(new ComponentEventHandler<PoweredLightComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<PoweredLightComponent, InteractHandEvent>(new ComponentEventHandler<PoweredLightComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<PoweredLightComponent, GhostBooEvent>(new ComponentEventHandler<PoweredLightComponent, GhostBooEvent>(this.OnGhostBoo), null, null);
			base.SubscribeLocalEvent<PoweredLightComponent, DamageChangedEvent>(new ComponentEventHandler<PoweredLightComponent, DamageChangedEvent>(this.HandleLightDamaged), null, null);
			base.SubscribeLocalEvent<PoweredLightComponent, SignalReceivedEvent>(new ComponentEventHandler<PoweredLightComponent, SignalReceivedEvent>(this.OnSignalReceived), null, null);
			base.SubscribeLocalEvent<PoweredLightComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<PoweredLightComponent, DeviceNetworkPacketEvent>(this.OnPacketReceived), null, null);
			base.SubscribeLocalEvent<PoweredLightComponent, PowerChangedEvent>(new ComponentEventRefHandler<PoweredLightComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<PoweredLightComponent, DoAfterEvent>(new ComponentEventHandler<PoweredLightComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x0006F1F0 File Offset: 0x0006D3F0
		private void OnInit(EntityUid uid, PoweredLightComponent light, ComponentInit args)
		{
			light.LightBulbContainer = this._containerSystem.EnsureContainer<ContainerSlot>(uid, "light_bulb", null);
			this._signalSystem.EnsureReceiverPorts(uid, new string[]
			{
				light.OnPort,
				light.OffPort,
				light.TogglePort
			});
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x0006F244 File Offset: 0x0006D444
		private void OnMapInit(EntityUid uid, PoweredLightComponent light, MapInitEvent args)
		{
			if (light.HasLampOnSpawn != null)
			{
				EntityUid entity = this.EntityManager.SpawnEntity(light.HasLampOnSpawn, this.EntityManager.GetComponent<TransformComponent>(light.Owner).Coordinates);
				light.LightBulbContainer.Insert(entity, null, null, null, null, null);
			}
			this.UpdateLight(uid, light, null, null);
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x0006F29D File Offset: 0x0006D49D
		private void OnInteractUsing(EntityUid uid, PoweredLightComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = this.InsertBulb(uid, args.Used, component);
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x0006F2BC File Offset: 0x0006D4BC
		private void OnInteractHand(EntityUid uid, PoweredLightComponent light, InteractHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			EntityUid? bulbUid = this.GetBulb(uid, light);
			if (bulbUid == null)
			{
				return;
			}
			EntityUid userUid = args.User;
			HeatResistanceComponent heatResist;
			LightBulbComponent lightBulb;
			if (this.EntityManager.TryGetComponent<HeatResistanceComponent>(userUid, ref heatResist) && this.EntityManager.TryGetComponent<LightBulbComponent>(bulbUid.Value, ref lightBulb))
			{
				int res = heatResist.GetHeatResistance();
				if (light.CurrentLit && res < lightBulb.BurningTemperature)
				{
					string burnMsg = Loc.GetString("powered-light-component-burn-hand");
					this._popupSystem.PopupEntity(burnMsg, uid, userUid, PopupType.Small);
					DamageSpecifier damage = this._damageableSystem.TryChangeDamage(new EntityUid?(userUid), light.Damage, false, true, null, new EntityUid?(userUid));
					if (damage != null)
					{
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.Damaged;
						LogStringHandler logStringHandler = new LogStringHandler(43, 3);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "user", "ToPrettyString(args.User)");
						logStringHandler.AppendLiteral(" burned their hand on ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Target), "target", "ToPrettyString(args.Target)");
						logStringHandler.AppendLiteral(" and received ");
						logStringHandler.AppendFormatted<FixedPoint2>(damage.Total, "damage", "damage.Total");
						logStringHandler.AppendLiteral(" damage");
						adminLogger.Add(type, ref logStringHandler);
					}
					this._audio.Play(light.BurnHandSound, Filter.Pvs(uid, 2f, null, null, null), uid, true, null);
					args.Handled = true;
					return;
				}
			}
			LightBulbComponent bulb;
			EntityUid? target;
			if (base.TryComp<LightBulbComponent>(bulbUid.Value, ref bulb) && bulb.State != LightBulbState.Normal)
			{
				target = this.EjectBulb(uid, new EntityUid?(userUid), light);
				args.Handled = (target != null);
				return;
			}
			SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
			EntityUid user = userUid;
			float ejectBulbDelay = light.EjectBulbDelay;
			target = new EntityUid?(uid);
			doAfterSystem.DoAfter(new DoAfterEventArgs(user, ejectBulbDelay, default(CancellationToken), target, null)
			{
				BreakOnUserMove = true,
				BreakOnDamage = true,
				BreakOnStun = true
			});
			args.Handled = true;
		}

		// Token: 0x06001532 RID: 5426 RVA: 0x0006F4CC File Offset: 0x0006D6CC
		[NullableContext(2)]
		public bool InsertBulb(EntityUid uid, EntityUid bulbUid, PoweredLightComponent light = null)
		{
			if (!base.Resolve<PoweredLightComponent>(uid, ref light, true))
			{
				return false;
			}
			if (this.GetBulb(uid, light) != null)
			{
				return false;
			}
			LightBulbComponent lightBulb;
			if (!this.EntityManager.TryGetComponent<LightBulbComponent>(bulbUid, ref lightBulb))
			{
				return false;
			}
			if (lightBulb.Type != light.BulbType)
			{
				return false;
			}
			if (!light.LightBulbContainer.Insert(bulbUid, null, null, null, null, null))
			{
				return false;
			}
			this.UpdateLight(uid, light, null, null);
			return true;
		}

		// Token: 0x06001533 RID: 5427 RVA: 0x0006F540 File Offset: 0x0006D740
		[NullableContext(2)]
		public EntityUid? EjectBulb(EntityUid uid, EntityUid? userUid = null, PoweredLightComponent light = null)
		{
			if (!base.Resolve<PoweredLightComponent>(uid, ref light, true))
			{
				return null;
			}
			EntityUid? bulb2 = this.GetBulb(uid, light);
			if (bulb2 != null)
			{
				EntityUid bulb = bulb2.GetValueOrDefault();
				if (bulb.Valid)
				{
					if (!light.LightBulbContainer.Remove(bulb, null, null, null, true, false, null, null))
					{
						return null;
					}
					this._handsSystem.PickupOrDrop(userUid, bulb, true, false, null, null);
					this.UpdateLight(uid, light, null, null);
					return new EntityUid?(bulb);
				}
			}
			return null;
		}

		// Token: 0x06001534 RID: 5428 RVA: 0x0006F5E0 File Offset: 0x0006D7E0
		[NullableContext(2)]
		public bool ReplaceBulb(EntityUid uid, EntityUid bulb, PoweredLightComponent light = null)
		{
			this.EjectBulb(uid, null, light);
			return this.InsertBulb(uid, bulb, light);
		}

		// Token: 0x06001535 RID: 5429 RVA: 0x0006F608 File Offset: 0x0006D808
		[NullableContext(2)]
		public EntityUid? GetBulb(EntityUid uid, PoweredLightComponent light = null)
		{
			if (!base.Resolve<PoweredLightComponent>(uid, ref light, true))
			{
				return null;
			}
			return light.LightBulbContainer.ContainedEntity;
		}

		// Token: 0x06001536 RID: 5430 RVA: 0x0006F638 File Offset: 0x0006D838
		[NullableContext(2)]
		public void TryDestroyBulb(EntityUid uid, PoweredLightComponent light = null)
		{
			EntityUid? bulbUid = this.GetBulb(uid, light);
			LightBulbComponent lightBulb;
			if (bulbUid == null || !this.EntityManager.TryGetComponent<LightBulbComponent>(bulbUid.Value, ref lightBulb))
			{
				return;
			}
			if (lightBulb.State == LightBulbState.Broken)
			{
				return;
			}
			this._bulbSystem.SetState(bulbUid.Value, LightBulbState.Broken, lightBulb);
			this._bulbSystem.PlayBreakSound(bulbUid.Value, lightBulb);
			this.UpdateLight(uid, light, null, null);
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x0006F6AC File Offset: 0x0006D8AC
		[NullableContext(2)]
		private void UpdateLight(EntityUid uid, PoweredLightComponent light = null, ApcPowerReceiverComponent powerReceiver = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<PoweredLightComponent, ApcPowerReceiverComponent>(uid, ref light, ref powerReceiver, true))
			{
				return;
			}
			base.Resolve<AppearanceComponent>(uid, ref appearance, false);
			EntityUid? bulbUid = this.GetBulb(uid, light);
			LightBulbComponent lightBulb;
			if (bulbUid == null || !this.EntityManager.TryGetComponent<LightBulbComponent>(bulbUid.Value, ref lightBulb))
			{
				bool value = false;
				PoweredLightComponent light2 = light;
				this.SetLight(uid, value, null, light2, null, null, null);
				powerReceiver.Load = 0f;
				this._appearance.SetData(uid, PoweredLightVisuals.BulbState, PoweredLightState.Empty, appearance);
				return;
			}
			switch (lightBulb.State)
			{
			case LightBulbState.Normal:
				if (powerReceiver.Powered && light.On)
				{
					this.SetLight(uid, true, new Color?(lightBulb.Color), light, new float?(lightBulb.LightRadius), new float?(lightBulb.LightEnergy), new float?(lightBulb.LightSoftness));
					this._appearance.SetData(uid, PoweredLightVisuals.BulbState, PoweredLightState.On, appearance);
					TimeSpan time = this._gameTiming.CurTime;
					if (time > light.LastThunk + PoweredLightSystem.ThunkDelay)
					{
						light.LastThunk = time;
						this._audio.Play(light.TurnOnSound, Filter.Pvs(uid, 2f, null, null, null), uid, true, new AudioParams?(AudioParams.Default.WithVolume(-10f)));
					}
				}
				else
				{
					bool value2 = false;
					PoweredLightComponent light2 = light;
					this.SetLight(uid, value2, null, light2, null, null, null);
					this._appearance.SetData(uid, PoweredLightVisuals.BulbState, PoweredLightState.Off, appearance);
				}
				break;
			case LightBulbState.Broken:
			{
				bool value3 = false;
				PoweredLightComponent light2 = light;
				this.SetLight(uid, value3, null, light2, null, null, null);
				this._appearance.SetData(uid, PoweredLightVisuals.BulbState, PoweredLightState.Broken, appearance);
				break;
			}
			case LightBulbState.Burned:
			{
				bool value4 = false;
				PoweredLightComponent light2 = light;
				this.SetLight(uid, value4, null, light2, null, null, null);
				this._appearance.SetData(uid, PoweredLightVisuals.BulbState, PoweredLightState.Burned, appearance);
				break;
			}
			}
			powerReceiver.Load = (float)((light.On && lightBulb.State == LightBulbState.Normal) ? lightBulb.PowerUse : 0);
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x0006F94F File Offset: 0x0006DB4F
		public void HandleLightDamaged(EntityUid uid, PoweredLightComponent component, DamageChangedEvent args)
		{
			if (args.DamageIncreased)
			{
				this.TryDestroyBulb(uid, component);
			}
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x0006F964 File Offset: 0x0006DB64
		private void OnGhostBoo(EntityUid uid, PoweredLightComponent light, GhostBooEvent args)
		{
			if (light.IgnoreGhostsBoo)
			{
				return;
			}
			TimeSpan time = this._gameTiming.CurTime;
			if (light.LastGhostBlink != null)
			{
				TimeSpan t = time;
				if (t <= light.LastGhostBlink + light.GhostBlinkingCooldown)
				{
					return;
				}
			}
			light.LastGhostBlink = new TimeSpan?(time);
			this.ToggleBlinkingLight(uid, light, true);
			TimerExtensions.SpawnTimer(light.Owner, light.GhostBlinkingTime, delegate()
			{
				this.ToggleBlinkingLight(uid, light, false);
			}, default(CancellationToken));
			args.Handled = true;
		}

		// Token: 0x0600153A RID: 5434 RVA: 0x0006FA75 File Offset: 0x0006DC75
		private void OnPowerChanged(EntityUid uid, PoweredLightComponent component, ref PowerChangedEvent args)
		{
			this.UpdateLight(uid, component, null, null);
		}

		// Token: 0x0600153B RID: 5435 RVA: 0x0006FA84 File Offset: 0x0006DC84
		public void ToggleBlinkingLight(EntityUid uid, PoweredLightComponent light, bool isNowBlinking)
		{
			if (light.IsBlinking == isNowBlinking)
			{
				return;
			}
			light.IsBlinking = isNowBlinking;
			AppearanceComponent appearance;
			if (!this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, PoweredLightVisuals.Blinking, isNowBlinking, appearance);
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x0006FACC File Offset: 0x0006DCCC
		private void OnSignalReceived(EntityUid uid, PoweredLightComponent component, SignalReceivedEvent args)
		{
			if (args.Port == component.OffPort)
			{
				this.SetState(uid, false, component);
				return;
			}
			if (args.Port == component.OnPort)
			{
				this.SetState(uid, true, component);
				return;
			}
			if (args.Port == component.TogglePort)
			{
				this.ToggleLight(uid, component);
			}
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x0006FB30 File Offset: 0x0006DD30
		private void OnPacketReceived(EntityUid uid, PoweredLightComponent component, DeviceNetworkPacketEvent args)
		{
			string command;
			if (!args.Data.TryGetValue<string>("command", out command) || command != "set_state")
			{
				return;
			}
			bool enabled;
			if (!args.Data.TryGetValue<bool>("state_enabled", out enabled))
			{
				return;
			}
			this.SetState(uid, enabled, component);
		}

		// Token: 0x0600153E RID: 5438 RVA: 0x0006FB80 File Offset: 0x0006DD80
		[NullableContext(2)]
		private void SetLight(EntityUid uid, bool value, Color? color = null, PoweredLightComponent light = null, float? radius = null, float? energy = null, float? softness = null)
		{
			if (!base.Resolve<PoweredLightComponent>(uid, ref light, true))
			{
				return;
			}
			light.CurrentLit = value;
			this._ambientSystem.SetAmbience(uid, value, null);
			PointLightComponent pointLight;
			if (this.EntityManager.TryGetComponent<PointLightComponent>(uid, ref pointLight))
			{
				pointLight.Enabled = value;
				if (color != null)
				{
					pointLight.Color = color.Value;
				}
				if (radius != null)
				{
					pointLight.Radius = radius.Value;
				}
				if (energy != null)
				{
					pointLight.Energy = energy.Value;
				}
				if (softness != null)
				{
					pointLight.Softness = softness.Value;
				}
			}
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x0006FC22 File Offset: 0x0006DE22
		[NullableContext(2)]
		public void ToggleLight(EntityUid uid, PoweredLightComponent light = null)
		{
			if (!base.Resolve<PoweredLightComponent>(uid, ref light, true))
			{
				return;
			}
			light.On = !light.On;
			this.UpdateLight(uid, light, null, null);
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x0006FC4A File Offset: 0x0006DE4A
		[NullableContext(2)]
		public void SetState(EntityUid uid, bool state, PoweredLightComponent light = null)
		{
			if (!base.Resolve<PoweredLightComponent>(uid, ref light, true))
			{
				return;
			}
			light.On = state;
			this.UpdateLight(uid, light, null, null);
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x0006FC6C File Offset: 0x0006DE6C
		private void OnDoAfter(EntityUid uid, PoweredLightComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled || args.Args.Target == null)
			{
				return;
			}
			this.EjectBulb(args.Args.Target.Value, new EntityUid?(args.Args.User), component);
			args.Handled = true;
		}

		// Token: 0x04000D16 RID: 3350
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000D17 RID: 3351
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x04000D18 RID: 3352
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSystem;

		// Token: 0x04000D19 RID: 3353
		[Dependency]
		private readonly LightBulbSystem _bulbSystem;

		// Token: 0x04000D1A RID: 3354
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000D1B RID: 3355
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000D1C RID: 3356
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000D1D RID: 3357
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;

		// Token: 0x04000D1E RID: 3358
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04000D1F RID: 3359
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000D20 RID: 3360
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000D21 RID: 3361
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000D22 RID: 3362
		private static readonly TimeSpan ThunkDelay = TimeSpan.FromSeconds(2.0);

		// Token: 0x04000D23 RID: 3363
		public const string LightBulbContainer = "light_bulb";
	}
}
