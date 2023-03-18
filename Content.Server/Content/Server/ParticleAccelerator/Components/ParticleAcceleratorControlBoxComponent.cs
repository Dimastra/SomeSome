using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Managers;
using Content.Server.Mind.Components;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.UserInterface;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Singularity.Components;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Server.ParticleAccelerator.Components
{
	// Token: 0x020002E4 RID: 740
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ParticleAcceleratorControlBoxComponent : ParticleAcceleratorPartComponent
	{
		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06000F28 RID: 3880 RVA: 0x0004DA68 File Offset: 0x0004BC68
		[ViewVariables]
		private BoundUserInterface UserInterface
		{
			get
			{
				return base.Owner.GetUIOrNull(ParticleAcceleratorControlBoxUiKey.Key);
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000F29 RID: 3881 RVA: 0x0004DA7B File Offset: 0x0004BC7B
		[ViewVariables]
		private bool ConsolePowered
		{
			get
			{
				ApcPowerReceiverComponent apcPowerReceiverComponent = this._apcPowerReceiverComponent;
				return apcPowerReceiverComponent == null || apcPowerReceiverComponent.Powered;
			}
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x0004DA8E File Offset: 0x0004BC8E
		public ParticleAcceleratorControlBoxComponent()
		{
			this.Master = this;
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000F2B RID: 3883 RVA: 0x0004DACE File Offset: 0x0004BCCE
		private ParticleAcceleratorPowerState MaxPower
		{
			get
			{
				if (!this._wireLimiterCut)
				{
					return ParticleAcceleratorPowerState.Level2;
				}
				return ParticleAcceleratorPowerState.Level3;
			}
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x0004DADC File Offset: 0x0004BCDC
		protected override void Initialize()
		{
			base.Initialize();
			if (this.UserInterface != null)
			{
				this.UserInterface.OnReceiveMessage += this.UserInterfaceOnOnReceiveMessage;
			}
			ComponentExt.EnsureComponent<ApcPowerReceiverComponent>(base.Owner, ref this._apcPowerReceiverComponent);
			this._apcPowerReceiverComponent.Load = 250f;
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x0004DB30 File Offset: 0x0004BD30
		public void OnPowerStateChanged(PowerChangedEvent e)
		{
			this.UpdateAppearance();
			if (!e.Powered)
			{
				BoundUserInterface userInterface = this.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.CloseAll();
			}
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x0004DB50 File Offset: 0x0004BD50
		[NullableContext(1)]
		private void UserInterfaceOnOnReceiveMessage(ServerBoundUserInterfaceMessage obj)
		{
			if (!this.ConsolePowered)
			{
				return;
			}
			if (this._wireInterfaceBlocked)
			{
				return;
			}
			BoundUserInterfaceMessage message = obj.Message;
			ParticleAcceleratorSetEnableMessage enableMessage = message as ParticleAcceleratorSetEnableMessage;
			if (enableMessage == null)
			{
				ParticleAcceleratorSetPowerStateMessage stateMessage = message as ParticleAcceleratorSetPowerStateMessage;
				if (stateMessage == null)
				{
					if (message is ParticleAcceleratorRescanPartsMessage)
					{
						this.RescanParts(obj.Session);
					}
				}
				else
				{
					this.SetStrength(stateMessage.State, obj.Session);
				}
			}
			else if (enableMessage.Enabled)
			{
				this.SwitchOn(obj.Session);
			}
			else
			{
				this.SwitchOff(obj.Session, false);
			}
			this.UpdateUI();
		}

		// Token: 0x06000F2F RID: 3887 RVA: 0x0004DBE0 File Offset: 0x0004BDE0
		public void UpdateUI()
		{
			float draw = 0f;
			float receive = 0f;
			if (this._isEnabled)
			{
				draw = this._partPowerBox.PowerConsumerComponent.DrawRate;
				receive = this._partPowerBox.PowerConsumerComponent.ReceivedPower;
			}
			ParticleAcceleratorUIState state = new ParticleAcceleratorUIState(this._isAssembled, this._isEnabled, this._selectedStrength, (int)draw, (int)receive, this._partEmitterLeft != null, this._partEmitterCenter != null, this._partEmitterRight != null, this._partPowerBox != null, this._partFuelChamber != null, this._partEndCap != null, this._wireInterfaceBlocked, this.MaxPower, this._wirePowerBlocked);
			BoundUserInterface userInterface = this.UserInterface;
			if (userInterface == null)
			{
				return;
			}
			userInterface.SetState(state, null, true);
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x0004DC9C File Offset: 0x0004BE9C
		protected override void OnRemove()
		{
			CancellationTokenSource fireCancelTokenSrc = this._fireCancelTokenSrc;
			if (fireCancelTokenSrc != null)
			{
				fireCancelTokenSrc.Cancel();
			}
			this._fireCancelTokenSrc = null;
			this.Master = null;
			foreach (ParticleAcceleratorPartComponent particleAcceleratorPartComponent in this.AllParts())
			{
				particleAcceleratorPartComponent.Master = null;
			}
			base.OnRemove();
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x0004DD0C File Offset: 0x0004BF0C
		public void RescanParts(IPlayerSession playerSession = null)
		{
			this.SwitchOff(playerSession, true);
			foreach (ParticleAcceleratorPartComponent particleAcceleratorPartComponent in this.AllParts())
			{
				particleAcceleratorPartComponent.Master = null;
			}
			this._isAssembled = false;
			this._partFuelChamber = null;
			this._partEndCap = null;
			this._partPowerBox = null;
			this._partEmitterLeft = null;
			this._partEmitterCenter = null;
			this._partEmitterRight = null;
			TransformComponent xform = this._entMan.GetComponent<TransformComponent>(base.Owner);
			MapGridComponent grid;
			if (xform.Anchored && this._entMan.TryGetComponent<MapGridComponent>(xform.GridUid, ref grid))
			{
				foreach (EntityUid maybeFuel in grid.GetCardinalNeighborCells(xform.Coordinates))
				{
					if (this._entMan.TryGetComponent<ParticleAcceleratorFuelChamberComponent>(maybeFuel, ref this._partFuelChamber))
					{
						break;
					}
				}
			}
			if (this._partFuelChamber == null)
			{
				this.UpdateUI();
				return;
			}
			xform.LocalRotation = this._entMan.GetComponent<TransformComponent>(this._partFuelChamber.Owner).LocalRotation;
			Vector2i vector2i = new ValueTuple<int, int>(1, 1);
			Vector2i offsetEndCap = this.<RescanParts>g__RotateOffset|35_0(vector2i);
			vector2i = new ValueTuple<int, int>(1, -1);
			Vector2i offsetPowerBox = this.<RescanParts>g__RotateOffset|35_0(vector2i);
			vector2i = new ValueTuple<int, int>(0, -2);
			Vector2i offsetEmitterLeft = this.<RescanParts>g__RotateOffset|35_0(vector2i);
			vector2i = new ValueTuple<int, int>(1, -2);
			Vector2i offsetEmitterCenter = this.<RescanParts>g__RotateOffset|35_0(vector2i);
			vector2i = new ValueTuple<int, int>(2, -2);
			Vector2i offsetEmitterRight = this.<RescanParts>g__RotateOffset|35_0(vector2i);
			this.ScanPart<ParticleAcceleratorEndCapComponent>(offsetEndCap, out this._partEndCap);
			this.ScanPart<ParticleAcceleratorPowerBoxComponent>(offsetPowerBox, out this._partPowerBox);
			if (!this.ScanPart<ParticleAcceleratorEmitterComponent>(offsetEmitterCenter, out this._partEmitterCenter) || this._partEmitterCenter.Type != ParticleAcceleratorEmitterType.Center)
			{
				this._partEmitterCenter = null;
			}
			if (this.ScanPart<ParticleAcceleratorEmitterComponent>(offsetEmitterLeft, out this._partEmitterLeft) && this._partEmitterLeft.Type != ParticleAcceleratorEmitterType.Left)
			{
				this._partEmitterLeft = null;
			}
			if (this.ScanPart<ParticleAcceleratorEmitterComponent>(offsetEmitterRight, out this._partEmitterRight) && this._partEmitterRight.Type != ParticleAcceleratorEmitterType.Right)
			{
				this._partEmitterRight = null;
			}
			this._isAssembled = (this._partFuelChamber != null && this._partPowerBox != null && this._partEmitterCenter != null && this._partEmitterLeft != null && this._partEmitterRight != null && this._partEndCap != null);
			foreach (ParticleAcceleratorPartComponent particleAcceleratorPartComponent2 in this.AllParts())
			{
				particleAcceleratorPartComponent2.Master = this;
			}
			this.UpdateUI();
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x0004DFD0 File Offset: 0x0004C1D0
		[NullableContext(0)]
		private bool ScanPart<T>(Vector2i offset, [Nullable(2)] [NotNullWhen(true)] out T part) where T : ParticleAcceleratorPartComponent
		{
			TransformComponent xform = this._entMan.GetComponent<TransformComponent>(base.Owner);
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(xform.GridUid, ref grid))
			{
				part = default(T);
				return false;
			}
			EntityCoordinates coords = xform.Coordinates;
			foreach (EntityUid ent in grid.GetOffset(coords, offset))
			{
				if (this._entMan.TryGetComponent<T>(ent, ref part) && !part.Deleted)
				{
					return true;
				}
			}
			part = default(T);
			return false;
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x0004E080 File Offset: 0x0004C280
		[NullableContext(1)]
		private IEnumerable<ParticleAcceleratorPartComponent> AllParts()
		{
			if (this._partFuelChamber != null)
			{
				yield return this._partFuelChamber;
			}
			if (this._partEndCap != null)
			{
				yield return this._partEndCap;
			}
			if (this._partPowerBox != null)
			{
				yield return this._partPowerBox;
			}
			if (this._partEmitterLeft != null)
			{
				yield return this._partEmitterLeft;
			}
			if (this._partEmitterCenter != null)
			{
				yield return this._partEmitterCenter;
			}
			if (this._partEmitterRight != null)
			{
				yield return this._partEmitterRight;
			}
			yield break;
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x0004E090 File Offset: 0x0004C290
		public void SwitchOn(IPlayerSession playerSession = null)
		{
			if (this._isEnabled)
			{
				return;
			}
			MindComponent mindComponent;
			this._entMan.TryGetComponent<MindComponent>((playerSession != null) ? playerSession.AttachedEntity : null, ref mindComponent);
			if (mindComponent != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(15, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(mindComponent.Owner), "player", "_entMan.ToPrettyString(mindComponent.Owner)");
				logStringHandler.AppendLiteral(" has set ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(base.Owner), "_entMan.ToPrettyString(Owner)");
				logStringHandler.AppendLiteral(" to on");
				adminLogger.Add(type, impact, ref logStringHandler);
				this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-particle-accelerator-on", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("player", this._entMan.ToPrettyString(mindComponent.Owner))
				}));
			}
			this._isEnabled = true;
			this.UpdatePowerDraw();
			if (this._partPowerBox.PowerConsumerComponent.ReceivedPower >= this._partPowerBox.PowerConsumerComponent.DrawRate)
			{
				this.PowerOn();
			}
			this.UpdateUI();
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0004E1BC File Offset: 0x0004C3BC
		private void UpdatePowerDraw()
		{
			this._partPowerBox.PowerConsumerComponent.DrawRate = (float)this.PowerDrawFor(this._selectedStrength);
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x0004E1DC File Offset: 0x0004C3DC
		public void SwitchOff(IPlayerSession playerSession = null, bool rescan = false)
		{
			MindComponent mindComponent;
			this._entMan.TryGetComponent<MindComponent>((playerSession != null) ? playerSession.AttachedEntity : null, ref mindComponent);
			if (mindComponent != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(16, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(mindComponent.Owner), "player", "_entMan.ToPrettyString(mindComponent.Owner)");
				logStringHandler.AppendLiteral(" has set ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(base.Owner), "_entMan.ToPrettyString(Owner)");
				logStringHandler.AppendLiteral(" to off");
				logStringHandler.AppendFormatted(rescan ? " via rescan" : "");
				adminLogger.Add(type, impact, ref logStringHandler);
				this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-particle-accelerator-off", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("player", this._entMan.ToPrettyString(mindComponent.Owner))
				}));
			}
			this._isEnabled = false;
			this.PowerOff();
			this.UpdateUI();
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x0004E2ED File Offset: 0x0004C4ED
		private void PowerOn()
		{
			if (this._isPowered)
			{
				return;
			}
			this._isPowered = true;
			this.UpdateFiring();
			this.UpdatePartVisualStates();
			this.UpdateUI();
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x0004E311 File Offset: 0x0004C511
		private void PowerOff()
		{
			if (!this._isPowered)
			{
				return;
			}
			this._isPowered = false;
			this.UpdateFiring();
			this.UpdateUI();
			this.UpdatePartVisualStates();
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x0004E338 File Offset: 0x0004C538
		public void SetStrength(ParticleAcceleratorPowerState state, IPlayerSession playerSession = null)
		{
			if (this._wireStrengthCut)
			{
				return;
			}
			state = (ParticleAcceleratorPowerState)MathHelper.Clamp((int)state, 1, (int)this.MaxPower);
			this._selectedStrength = state;
			this.UpdateAppearance();
			this.UpdatePartVisualStates();
			MindComponent mindComponent;
			this._entMan.TryGetComponent<MindComponent>((playerSession != null) ? playerSession.AttachedEntity : null, ref mindComponent);
			LogImpact impact;
			switch (state)
			{
			default:
				impact = LogImpact.Low;
				break;
			case ParticleAcceleratorPowerState.Level1:
				impact = LogImpact.High;
				break;
			case ParticleAcceleratorPowerState.Level2:
			case ParticleAcceleratorPowerState.Level3:
				impact = LogImpact.Extreme;
				break;
			}
			if (mindComponent != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact2 = impact;
				LogStringHandler logStringHandler = new LogStringHandler(29, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(mindComponent.Owner), "player", "_entMan.ToPrettyString(mindComponent.Owner)");
				logStringHandler.AppendLiteral(" has set the strength of ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(base.Owner), "_entMan.ToPrettyString(Owner)");
				logStringHandler.AppendLiteral(" to ");
				logStringHandler.AppendFormatted<ParticleAcceleratorPowerState>(state, "state");
				adminLogger.Add(type, impact2, ref logStringHandler);
				this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-particle-strength-change", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("player", this._entMan.ToPrettyString(mindComponent.Owner)),
					new ValueTuple<string, object>("state", state)
				}));
			}
			if (this._isEnabled)
			{
				this.UpdatePowerDraw();
				this.UpdateFiring();
			}
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x0004E4AC File Offset: 0x0004C6AC
		private void UpdateAppearance()
		{
			AppearanceComponent appearance;
			if (this._entMan.TryGetComponent<AppearanceComponent>(base.Owner, ref appearance))
			{
				appearance.SetData(ParticleAcceleratorVisuals.VisualState, (ParticleAcceleratorVisualState)(this._apcPowerReceiverComponent.Powered ? this._selectedStrength : ((ParticleAcceleratorPowerState)0)));
			}
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x0004E4F5 File Offset: 0x0004C6F5
		private void UpdateFiring()
		{
			if (!this._isPowered || this._selectedStrength == ParticleAcceleratorPowerState.Standby)
			{
				this.StopFiring();
				return;
			}
			this.StartFiring();
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x0004E518 File Offset: 0x0004C718
		private void StartFiring()
		{
			CancellationTokenSource fireCancelTokenSrc = this._fireCancelTokenSrc;
			if (fireCancelTokenSrc != null)
			{
				fireCancelTokenSrc.Cancel();
			}
			this._fireCancelTokenSrc = new CancellationTokenSource();
			CancellationToken cancelToken = this._fireCancelTokenSrc.Token;
			Timer.SpawnRepeating(this._firingDelay, new Action(this.Fire), cancelToken);
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x0004E565 File Offset: 0x0004C765
		private void Fire()
		{
			this._partEmitterCenter.Fire(this._selectedStrength);
			this._partEmitterLeft.Fire(this._selectedStrength);
			this._partEmitterRight.Fire(this._selectedStrength);
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x0004E59A File Offset: 0x0004C79A
		[Conditional("DEBUG")]
		private void EverythingIsWellToFire()
		{
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x0004E59C File Offset: 0x0004C79C
		private void StopFiring()
		{
			CancellationTokenSource fireCancelTokenSrc = this._fireCancelTokenSrc;
			if (fireCancelTokenSrc != null)
			{
				fireCancelTokenSrc.Cancel();
			}
			this._fireCancelTokenSrc = null;
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x0004E5B8 File Offset: 0x0004C7B8
		private int PowerDrawFor(ParticleAcceleratorPowerState strength)
		{
			int num;
			switch (strength)
			{
			case ParticleAcceleratorPowerState.Standby:
				num = 0;
				break;
			case ParticleAcceleratorPowerState.Level0:
				num = 1;
				break;
			case ParticleAcceleratorPowerState.Level1:
				num = 3;
				break;
			case ParticleAcceleratorPowerState.Level2:
				num = 4;
				break;
			case ParticleAcceleratorPowerState.Level3:
				num = 5;
				break;
			default:
				num = 0;
				break;
			}
			return num * this._powerDrawMult + this._powerDrawBase;
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x0004E608 File Offset: 0x0004C808
		public void PowerBoxReceivedChanged(PowerConsumerReceivedChanged eventArgs)
		{
			if (!this._isEnabled)
			{
				return;
			}
			if (eventArgs.ReceivedPower >= eventArgs.DrawRate)
			{
				this.PowerOn();
			}
			else
			{
				this.PowerOff();
			}
			this.UpdateUI();
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x0004E63A File Offset: 0x0004C83A
		private void UpdatePartVisualStates()
		{
			this.UpdatePartVisualState(this._partFuelChamber);
			this.UpdatePartVisualState(this._partPowerBox);
			this.UpdatePartVisualState(this._partEmitterCenter);
			this.UpdatePartVisualState(this._partEmitterLeft);
			this.UpdatePartVisualState(this._partEmitterRight);
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x0004E678 File Offset: 0x0004C878
		private void UpdatePartVisualState(ParticleAcceleratorPartComponent component)
		{
			AppearanceComponent appearanceComponent;
			if (component == null || !this._entMan.TryGetComponent<AppearanceComponent>(component.Owner, ref appearanceComponent))
			{
				return;
			}
			ParticleAcceleratorVisualState state = (ParticleAcceleratorVisualState)(this._isPowered ? this._selectedStrength : ((ParticleAcceleratorPowerState)0));
			appearanceComponent.SetData(ParticleAcceleratorVisuals.VisualState, state);
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x0004E6C2 File Offset: 0x0004C8C2
		public override void Moved()
		{
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x0004E6C4 File Offset: 0x0004C8C4
		[CompilerGenerated]
		private Vector2i <RescanParts>g__RotateOffset|35_0(in Vector2i vec)
		{
			Angle rot;
			rot..ctor(this._entMan.GetComponent<TransformComponent>(base.Owner).LocalRotation);
			Vector2 vector = vec;
			return (Vector2i)rot.RotateVec(ref vector);
		}

		// Token: 0x040008E0 RID: 2272
		[Nullable(1)]
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x040008E1 RID: 2273
		[Nullable(1)]
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040008E2 RID: 2274
		[Nullable(1)]
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040008E3 RID: 2275
		[Nullable(1)]
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x040008E4 RID: 2276
		[Nullable(1)]
		[ViewVariables]
		private ApcPowerReceiverComponent _apcPowerReceiverComponent;

		// Token: 0x040008E5 RID: 2277
		[ViewVariables]
		private ParticleAcceleratorFuelChamberComponent _partFuelChamber;

		// Token: 0x040008E6 RID: 2278
		[ViewVariables]
		private ParticleAcceleratorEndCapComponent _partEndCap;

		// Token: 0x040008E7 RID: 2279
		[ViewVariables]
		private ParticleAcceleratorPowerBoxComponent _partPowerBox;

		// Token: 0x040008E8 RID: 2280
		[ViewVariables]
		private ParticleAcceleratorEmitterComponent _partEmitterLeft;

		// Token: 0x040008E9 RID: 2281
		[ViewVariables]
		private ParticleAcceleratorEmitterComponent _partEmitterCenter;

		// Token: 0x040008EA RID: 2282
		[ViewVariables]
		private ParticleAcceleratorEmitterComponent _partEmitterRight;

		// Token: 0x040008EB RID: 2283
		[ViewVariables]
		private ParticleAcceleratorPowerState _selectedStrength = ParticleAcceleratorPowerState.Standby;

		// Token: 0x040008EC RID: 2284
		[ViewVariables]
		private bool _isAssembled;

		// Token: 0x040008ED RID: 2285
		[ViewVariables]
		private bool _isEnabled;

		// Token: 0x040008EE RID: 2286
		[ViewVariables]
		private bool _isPowered;

		// Token: 0x040008EF RID: 2287
		[ViewVariables]
		private bool _wireInterfaceBlocked;

		// Token: 0x040008F0 RID: 2288
		[ViewVariables]
		private bool _wirePowerBlocked;

		// Token: 0x040008F1 RID: 2289
		[ViewVariables]
		private bool _wireLimiterCut;

		// Token: 0x040008F2 RID: 2290
		[ViewVariables]
		private bool _wireStrengthCut;

		// Token: 0x040008F3 RID: 2291
		[ViewVariables]
		private CancellationTokenSource _fireCancelTokenSrc;

		// Token: 0x040008F4 RID: 2292
		[ViewVariables]
		[DataField("fireDelay", false, 1, false, false, null)]
		private TimeSpan _firingDelay = TimeSpan.FromSeconds(6.0);

		// Token: 0x040008F5 RID: 2293
		[ViewVariables]
		[DataField("powerDrawBase", false, 1, false, false, null)]
		private int _powerDrawBase = 500;

		// Token: 0x040008F6 RID: 2294
		[ViewVariables]
		[DataField("powerDrawMult", false, 1, false, false, null)]
		private int _powerDrawMult = 1500;

		// Token: 0x02000956 RID: 2390
		[NullableContext(0)]
		public enum ParticleAcceleratorControlBoxWires
		{
			// Token: 0x04001FEA RID: 8170
			Toggle,
			// Token: 0x04001FEB RID: 8171
			Strength,
			// Token: 0x04001FEC RID: 8172
			Interface,
			// Token: 0x04001FED RID: 8173
			Limiter,
			// Token: 0x04001FEE RID: 8174
			Nothing
		}
	}
}
