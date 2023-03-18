using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Audio;
using Content.Server.Chat.Managers;
using Content.Server.Power.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Gravity;
using Content.Shared.Interaction;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Players;

namespace Content.Server.Gravity
{
	// Token: 0x02000489 RID: 1161
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GravityGeneratorSystem : EntitySystem
	{
		// Token: 0x0600173E RID: 5950 RVA: 0x00079E38 File Offset: 0x00078038
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GravityGeneratorComponent, ComponentInit>(new ComponentEventHandler<GravityGeneratorComponent, ComponentInit>(this.OnCompInit), null, null);
			base.SubscribeLocalEvent<GravityGeneratorComponent, ComponentShutdown>(new ComponentEventHandler<GravityGeneratorComponent, ComponentShutdown>(this.OnComponentShutdown), null, null);
			base.SubscribeLocalEvent<GravityGeneratorComponent, EntParentChangedMessage>(new ComponentEventRefHandler<GravityGeneratorComponent, EntParentChangedMessage>(this.OnParentChanged), null, null);
			base.SubscribeLocalEvent<GravityGeneratorComponent, InteractHandEvent>(new ComponentEventHandler<GravityGeneratorComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<GravityGeneratorComponent, SharedGravityGeneratorComponent.SwitchGeneratorMessage>(new ComponentEventHandler<GravityGeneratorComponent, SharedGravityGeneratorComponent.SwitchGeneratorMessage>(this.OnSwitchGenerator), null, null);
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x00079EB0 File Offset: 0x000780B0
		private void OnParentChanged(EntityUid uid, GravityGeneratorComponent component, ref EntParentChangedMessage args)
		{
			GravityComponent gravity;
			if (component.GravityActive && base.TryComp<GravityComponent>(args.OldParent, ref gravity))
			{
				this._gravitySystem.RefreshGravity(args.OldParent.Value, gravity);
			}
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x00079EF0 File Offset: 0x000780F0
		private void OnComponentShutdown(EntityUid uid, GravityGeneratorComponent component, ComponentShutdown args)
		{
			TransformComponent xform;
			GravityComponent gravity;
			if (component.GravityActive && base.TryComp<TransformComponent>(uid, ref xform) && base.TryComp<GravityComponent>(xform.ParentUid, ref gravity))
			{
				component.GravityActive = false;
				this._gravitySystem.RefreshGravity(xform.ParentUid, gravity);
			}
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x00079F3C File Offset: 0x0007813C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<GravityGeneratorComponent, ApcPowerReceiverComponent> valueTuple in this.EntityManager.EntityQuery<GravityGeneratorComponent, ApcPowerReceiverComponent>(false))
			{
				GravityGeneratorComponent gravGen = valueTuple.Item1;
				ApcPowerReceiverComponent powerReceiver = valueTuple.Item2;
				if (gravGen.Intact)
				{
					float chargeRate;
					if (gravGen.SwitchedOn)
					{
						if (powerReceiver.Powered)
						{
							chargeRate = gravGen.ChargeRate;
						}
						else
						{
							float receiving = powerReceiver.PowerReceived;
							float mainSystemPower = Math.Max(0f, receiving - gravGen.IdlePowerUse);
							chargeRate = -((1f - mainSystemPower / (gravGen.ActivePowerUse - gravGen.IdlePowerUse)) * gravGen.ChargeRate);
						}
					}
					else
					{
						chargeRate = -gravGen.ChargeRate;
					}
					bool gravityActive = gravGen.GravityActive;
					float charge = gravGen.Charge;
					gravGen.Charge = Math.Clamp(gravGen.Charge + frameTime * chargeRate, 0f, 1f);
					if (chargeRate > 0f)
					{
						if (MathHelper.CloseTo(gravGen.Charge, 1f, 1E-07f) && !gravGen.GravityActive)
						{
							gravGen.GravityActive = true;
						}
					}
					else if (MathHelper.CloseTo(gravGen.Charge, 0f, 1E-07f) && gravGen.GravityActive)
					{
						gravGen.GravityActive = false;
					}
					bool updateUI = gravGen.NeedUIUpdate;
					if (!MathHelper.CloseTo(charge, gravGen.Charge, 1E-07f))
					{
						this.UpdateState(gravGen, powerReceiver);
						updateUI = true;
					}
					if (updateUI)
					{
						this.UpdateUI(gravGen, powerReceiver, chargeRate);
					}
					TransformComponent xform;
					GravityComponent gravity;
					if (gravityActive != gravGen.GravityActive && base.TryComp<TransformComponent>(gravGen.Owner, ref xform) && base.TryComp<GravityComponent>(xform.ParentUid, ref gravity))
					{
						if (gravGen.GravityActive)
						{
							this._gravitySystem.EnableGravity(xform.ParentUid, gravity);
						}
						else
						{
							this._gravitySystem.RefreshGravity(xform.ParentUid, gravity);
						}
					}
				}
			}
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x0007A128 File Offset: 0x00078328
		[NullableContext(2)]
		private void SetSwitchedOn(EntityUid uid, [Nullable(1)] GravityGeneratorComponent component, bool on, ApcPowerReceiverComponent powerReceiver = null, ICommonSession session = null)
		{
			if (!base.Resolve<ApcPowerReceiverComponent>(uid, ref powerReceiver, true))
			{
				return;
			}
			if (session != null && session.AttachedEntity != null)
			{
				EntityUid player = session.AttachedEntity.Value;
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = on ? LogImpact.Medium : LogImpact.High;
				LogStringHandler logStringHandler = new LogStringHandler(10, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "player", "ToPrettyString(player)");
				logStringHandler.AppendLiteral(" set $");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "target", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" to ");
				logStringHandler.AppendFormatted(on ? "on" : "off");
				adminLogger.Add(type, impact, ref logStringHandler);
				this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-gravity-generator-turned", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("player", base.ToPrettyString(player)),
					new ValueTuple<string, object>("gravgen", base.ToPrettyString(uid)),
					new ValueTuple<string, object>("status", on ? "on" : "off")
				}));
			}
			component.SwitchedOn = on;
			GravityGeneratorSystem.UpdatePowerState(component, powerReceiver);
			component.NeedUIUpdate = true;
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x0007A276 File Offset: 0x00078476
		private static void UpdatePowerState(GravityGeneratorComponent component, ApcPowerReceiverComponent powerReceiver)
		{
			powerReceiver.Load = (component.SwitchedOn ? component.ActivePowerUse : component.IdlePowerUse);
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x0007A294 File Offset: 0x00078494
		private void UpdateUI(GravityGeneratorComponent component, ApcPowerReceiverComponent powerReceiver, float chargeRate)
		{
			if (!this._uiSystem.IsUiOpen(component.Owner, SharedGravityGeneratorComponent.GravityGeneratorUiKey.Key, null))
			{
				return;
			}
			int chargeTarget = (chargeRate < 0f) ? 0 : 1;
			bool atTarget = false;
			short chargeEta;
			if (MathHelper.CloseTo(component.Charge, (float)chargeTarget, 1E-07f))
			{
				chargeEta = short.MinValue;
				atTarget = true;
			}
			else
			{
				chargeEta = (short)Math.Abs(((float)chargeTarget - component.Charge) / chargeRate);
			}
			GravityGeneratorPowerStatus gravityGeneratorPowerStatus;
			if (chargeRate <= 0f)
			{
				if (chargeRate >= 0f)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (atTarget)
				{
					gravityGeneratorPowerStatus = GravityGeneratorPowerStatus.Off;
				}
				else
				{
					gravityGeneratorPowerStatus = GravityGeneratorPowerStatus.Discharging;
				}
			}
			else if (atTarget)
			{
				gravityGeneratorPowerStatus = GravityGeneratorPowerStatus.FullyCharged;
			}
			else
			{
				gravityGeneratorPowerStatus = GravityGeneratorPowerStatus.Charging;
			}
			GravityGeneratorPowerStatus status = gravityGeneratorPowerStatus;
			SharedGravityGeneratorComponent.GeneratorState state = new SharedGravityGeneratorComponent.GeneratorState(component.SwitchedOn, (byte)(component.Charge * 255f), status, (short)Math.Round((double)powerReceiver.PowerReceived), (short)Math.Round((double)powerReceiver.Load), chargeEta);
			this._uiSystem.TrySetUiState(component.Owner, SharedGravityGeneratorComponent.GravityGeneratorUiKey.Key, state, null, null, true);
			component.NeedUIUpdate = false;
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x0007A390 File Offset: 0x00078590
		private void OnCompInit(EntityUid uid, GravityGeneratorComponent component, ComponentInit args)
		{
			ApcPowerReceiverComponent powerReceiver = null;
			if (!base.Resolve<ApcPowerReceiverComponent>(uid, ref powerReceiver, false))
			{
				return;
			}
			GravityGeneratorSystem.UpdatePowerState(component, powerReceiver);
			this.UpdateState(component, powerReceiver);
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x0007A3BC File Offset: 0x000785BC
		private void OnInteractHand(EntityUid uid, GravityGeneratorComponent component, InteractHandEvent args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			ApcPowerReceiverComponent powerReceiver = null;
			if (!base.Resolve<ApcPowerReceiverComponent>(uid, ref powerReceiver, true))
			{
				return;
			}
			if (!component.Intact || powerReceiver.PowerReceived < component.IdlePowerUse)
			{
				return;
			}
			this._uiSystem.TryOpen(uid, SharedGravityGeneratorComponent.GravityGeneratorUiKey.Key, actor.PlayerSession, null);
			component.NeedUIUpdate = true;
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x0007A428 File Offset: 0x00078628
		public void UpdateState(GravityGeneratorComponent grav, ApcPowerReceiverComponent powerReceiver)
		{
			EntityUid uid = grav.Owner;
			AppearanceComponent appearance = EntityManagerExt.GetComponentOrNull<AppearanceComponent>(this.EntityManager, uid);
			this._appearance.SetData(uid, GravityGeneratorVisuals.Charge, grav.Charge, appearance);
			PointLightComponent pointLight;
			if (this.EntityManager.TryGetComponent<PointLightComponent>(uid, ref pointLight))
			{
				pointLight.Enabled = (grav.Charge > 0f);
				pointLight.Radius = MathHelper.Lerp(grav.LightRadiusMin, grav.LightRadiusMax, grav.Charge);
			}
			if (!grav.Intact)
			{
				this.MakeBroken(uid, grav, appearance);
				return;
			}
			if (powerReceiver.PowerReceived < grav.IdlePowerUse)
			{
				this.MakeUnpowered(uid, grav, appearance);
				return;
			}
			if (!grav.SwitchedOn)
			{
				this.MakeOff(uid, grav, appearance);
				return;
			}
			this.MakeOn(uid, grav, appearance);
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x0007A4EC File Offset: 0x000786EC
		private void MakeBroken(EntityUid uid, GravityGeneratorComponent component, [Nullable(2)] AppearanceComponent appearance)
		{
			this._ambientSoundSystem.SetAmbience(component.Owner, false, null);
			this._appearance.SetData(uid, GravityGeneratorVisuals.State, GravityGeneratorStatus.Broken, null);
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x0007A51A File Offset: 0x0007871A
		private void MakeUnpowered(EntityUid uid, GravityGeneratorComponent component, [Nullable(2)] AppearanceComponent appearance)
		{
			this._ambientSoundSystem.SetAmbience(component.Owner, false, null);
			this._appearance.SetData(uid, GravityGeneratorVisuals.State, GravityGeneratorStatus.Unpowered, appearance);
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x0007A548 File Offset: 0x00078748
		private void MakeOff(EntityUid uid, GravityGeneratorComponent component, [Nullable(2)] AppearanceComponent appearance)
		{
			this._ambientSoundSystem.SetAmbience(component.Owner, false, null);
			this._appearance.SetData(uid, GravityGeneratorVisuals.State, GravityGeneratorStatus.Off, appearance);
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x0007A576 File Offset: 0x00078776
		private void MakeOn(EntityUid uid, GravityGeneratorComponent component, [Nullable(2)] AppearanceComponent appearance)
		{
			this._ambientSoundSystem.SetAmbience(component.Owner, true, null);
			this._appearance.SetData(uid, GravityGeneratorVisuals.State, GravityGeneratorStatus.On, appearance);
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x0007A5A4 File Offset: 0x000787A4
		private void OnSwitchGenerator(EntityUid uid, GravityGeneratorComponent component, SharedGravityGeneratorComponent.SwitchGeneratorMessage args)
		{
			this.SetSwitchedOn(uid, component, args.On, null, args.Session);
		}

		// Token: 0x04000E8C RID: 3724
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000E8D RID: 3725
		[Dependency]
		private readonly AmbientSoundSystem _ambientSoundSystem;

		// Token: 0x04000E8E RID: 3726
		[Dependency]
		private readonly GravitySystem _gravitySystem;

		// Token: 0x04000E8F RID: 3727
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000E90 RID: 3728
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x04000E91 RID: 3729
		[Dependency]
		private readonly IChatManager _chatManager;
	}
}
