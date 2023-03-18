using System;
using System.Runtime.CompilerServices;
using Content.Server.Climbing;
using Content.Server.Cloning;
using Content.Server.Cloning.Components;
using Content.Server.Construction;
using Content.Server.MachineLinking.Events;
using Content.Server.MachineLinking.System;
using Content.Server.Medical.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.ActionBlocker;
using Content.Shared.Body.Components;
using Content.Shared.Destructible;
using Content.Shared.DragDrop;
using Content.Shared.MedicalScanner;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Verbs;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Medical
{
	// Token: 0x020003B1 RID: 945
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MedicalScannerSystem : EntitySystem
	{
		// Token: 0x0600136E RID: 4974 RVA: 0x0006461C File Offset: 0x0006281C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MedicalScannerComponent, ComponentInit>(new ComponentEventHandler<MedicalScannerComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<MedicalScannerComponent, ContainerRelayMovementEntityEvent>(new ComponentEventRefHandler<MedicalScannerComponent, ContainerRelayMovementEntityEvent>(this.OnRelayMovement), null, null);
			base.SubscribeLocalEvent<MedicalScannerComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<MedicalScannerComponent, GetVerbsEvent<InteractionVerb>>(this.AddInsertOtherVerb), null, null);
			base.SubscribeLocalEvent<MedicalScannerComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<MedicalScannerComponent, GetVerbsEvent<AlternativeVerb>>(this.AddAlternativeVerbs), null, null);
			base.SubscribeLocalEvent<MedicalScannerComponent, DestructionEventArgs>(new ComponentEventHandler<MedicalScannerComponent, DestructionEventArgs>(this.OnDestroyed), null, null);
			base.SubscribeLocalEvent<MedicalScannerComponent, DragDropTargetEvent>(new ComponentEventRefHandler<MedicalScannerComponent, DragDropTargetEvent>(this.OnDragDropOn), null, null);
			base.SubscribeLocalEvent<MedicalScannerComponent, PortDisconnectedEvent>(new ComponentEventHandler<MedicalScannerComponent, PortDisconnectedEvent>(this.OnPortDisconnected), null, null);
			base.SubscribeLocalEvent<MedicalScannerComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<MedicalScannerComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
			base.SubscribeLocalEvent<MedicalScannerComponent, RefreshPartsEvent>(new ComponentEventHandler<MedicalScannerComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<MedicalScannerComponent, UpgradeExamineEvent>(new ComponentEventHandler<MedicalScannerComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<MedicalScannerComponent, CanDropTargetEvent>(new ComponentEventRefHandler<MedicalScannerComponent, CanDropTargetEvent>(this.OnCanDragDropOn), null, null);
		}

		// Token: 0x0600136F RID: 4975 RVA: 0x0006470B File Offset: 0x0006290B
		private void OnCanDragDropOn(EntityUid uid, MedicalScannerComponent component, ref CanDropTargetEvent args)
		{
			args.Handled = true;
			args.CanDrop |= this.CanScannerInsert(uid, args.Dragged, component);
		}

		// Token: 0x06001370 RID: 4976 RVA: 0x0006472C File Offset: 0x0006292C
		[NullableContext(2)]
		public bool CanScannerInsert(EntityUid uid, EntityUid target, MedicalScannerComponent component = null)
		{
			return base.Resolve<MedicalScannerComponent>(uid, ref component, true) && base.HasComp<BodyComponent>(target);
		}

		// Token: 0x06001371 RID: 4977 RVA: 0x00064743 File Offset: 0x00062943
		private void OnComponentInit(EntityUid uid, MedicalScannerComponent scannerComponent, ComponentInit args)
		{
			base.Initialize();
			scannerComponent.BodyContainer = this._containerSystem.EnsureContainer<ContainerSlot>(uid, "scanner-bodyContainer", null);
			this._signalSystem.EnsureReceiverPorts(uid, new string[]
			{
				"MedicalScannerReceiver"
			});
		}

		// Token: 0x06001372 RID: 4978 RVA: 0x0006477D File Offset: 0x0006297D
		private void OnRelayMovement(EntityUid uid, MedicalScannerComponent scannerComponent, ref ContainerRelayMovementEntityEvent args)
		{
			if (!this._blocker.CanInteract(args.Entity, new EntityUid?(uid)))
			{
				return;
			}
			this.EjectBody(uid, scannerComponent);
		}

		// Token: 0x06001373 RID: 4979 RVA: 0x000647A4 File Offset: 0x000629A4
		private void AddInsertOtherVerb(EntityUid uid, MedicalScannerComponent component, GetVerbsEvent<InteractionVerb> args)
		{
			if (args.Using == null || !args.CanAccess || !args.CanInteract || this.IsOccupied(component) || !this.CanScannerInsert(uid, args.Using.Value, component))
			{
				return;
			}
			string name = "Unknown";
			MetaDataComponent metadata;
			if (base.TryComp<MetaDataComponent>(args.Using.Value, ref metadata))
			{
				name = metadata.EntityName;
			}
			InteractionVerb verb = new InteractionVerb
			{
				Act = delegate()
				{
					this.InsertBody(uid, args.Target, component);
				},
				Category = VerbCategory.Insert,
				Text = name
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06001374 RID: 4980 RVA: 0x00064898 File Offset: 0x00062A98
		private void AddAlternativeVerbs(EntityUid uid, MedicalScannerComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			if (this.IsOccupied(component))
			{
				AlternativeVerb verb = new AlternativeVerb();
				verb.Act = delegate()
				{
					this.EjectBody(uid, component);
				};
				verb.Category = VerbCategory.Eject;
				verb.Text = Loc.GetString("medical-scanner-verb-noun-occupant");
				verb.Priority = 1;
				args.Verbs.Add(verb);
			}
			if (!this.IsOccupied(component) && this.CanScannerInsert(uid, args.User, component) && this._blocker.CanMove(args.User, null))
			{
				AlternativeVerb verb2 = new AlternativeVerb();
				verb2.Act = delegate()
				{
					this.InsertBody(uid, args.User, component);
				};
				verb2.Text = Loc.GetString("medical-scanner-verb-enter");
				args.Verbs.Add(verb2);
			}
		}

		// Token: 0x06001375 RID: 4981 RVA: 0x000649BC File Offset: 0x00062BBC
		private void OnDestroyed(EntityUid uid, MedicalScannerComponent scannerComponent, DestructionEventArgs args)
		{
			this.EjectBody(uid, scannerComponent);
		}

		// Token: 0x06001376 RID: 4982 RVA: 0x000649C6 File Offset: 0x00062BC6
		private void OnDragDropOn(EntityUid uid, MedicalScannerComponent scannerComponent, ref DragDropTargetEvent args)
		{
			this.InsertBody(uid, args.Dragged, scannerComponent);
		}

		// Token: 0x06001377 RID: 4983 RVA: 0x000649D6 File Offset: 0x00062BD6
		private void OnPortDisconnected(EntityUid uid, MedicalScannerComponent component, PortDisconnectedEvent args)
		{
			component.ConnectedConsole = null;
		}

		// Token: 0x06001378 RID: 4984 RVA: 0x000649E4 File Offset: 0x00062BE4
		private void OnAnchorChanged(EntityUid uid, MedicalScannerComponent component, ref AnchorStateChangedEvent args)
		{
			CloningConsoleComponent console;
			if (component.ConnectedConsole == null || !base.TryComp<CloningConsoleComponent>(component.ConnectedConsole, ref console))
			{
				return;
			}
			if (args.Anchored)
			{
				this._cloningConsoleSystem.RecheckConnections(component.ConnectedConsole.Value, console.CloningPod, new EntityUid?(uid), console);
				return;
			}
			this._cloningConsoleSystem.UpdateUserInterface(console);
		}

		// Token: 0x06001379 RID: 4985 RVA: 0x00064A48 File Offset: 0x00062C48
		private SharedMedicalScannerComponent.MedicalScannerStatus GetStatus(EntityUid uid, MedicalScannerComponent scannerComponent)
		{
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return SharedMedicalScannerComponent.MedicalScannerStatus.Off;
			}
			EntityUid? body = scannerComponent.BodyContainer.ContainedEntity;
			if (body == null)
			{
				return SharedMedicalScannerComponent.MedicalScannerStatus.Open;
			}
			MobStateComponent state;
			if (!base.TryComp<MobStateComponent>(body.Value, ref state))
			{
				return SharedMedicalScannerComponent.MedicalScannerStatus.Open;
			}
			return this.GetStatusFromDamageState(body.Value, state);
		}

		// Token: 0x0600137A RID: 4986 RVA: 0x00064AA0 File Offset: 0x00062CA0
		public bool IsOccupied(MedicalScannerComponent scannerComponent)
		{
			return scannerComponent.BodyContainer.ContainedEntity != null;
		}

		// Token: 0x0600137B RID: 4987 RVA: 0x00064AC0 File Offset: 0x00062CC0
		private SharedMedicalScannerComponent.MedicalScannerStatus GetStatusFromDamageState(EntityUid uid, MobStateComponent state)
		{
			if (this._mobStateSystem.IsAlive(uid, state))
			{
				return SharedMedicalScannerComponent.MedicalScannerStatus.Green;
			}
			if (this._mobStateSystem.IsCritical(uid, state))
			{
				return SharedMedicalScannerComponent.MedicalScannerStatus.Red;
			}
			if (this._mobStateSystem.IsDead(uid, state))
			{
				return SharedMedicalScannerComponent.MedicalScannerStatus.Death;
			}
			return SharedMedicalScannerComponent.MedicalScannerStatus.Yellow;
		}

		// Token: 0x0600137C RID: 4988 RVA: 0x00064AF8 File Offset: 0x00062CF8
		private void UpdateAppearance(EntityUid uid, MedicalScannerComponent scannerComponent)
		{
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, SharedMedicalScannerComponent.MedicalScannerVisuals.Status, this.GetStatus(uid, scannerComponent), appearance);
			}
		}

		// Token: 0x0600137D RID: 4989 RVA: 0x00064B30 File Offset: 0x00062D30
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this._updateDif += frameTime;
			if (this._updateDif < 1f)
			{
				return;
			}
			this._updateDif -= 1f;
			foreach (MedicalScannerComponent scanner in base.EntityQuery<MedicalScannerComponent>(false))
			{
				this.UpdateAppearance(scanner.Owner, scanner);
			}
		}

		// Token: 0x0600137E RID: 4990 RVA: 0x00064BBC File Offset: 0x00062DBC
		[NullableContext(2)]
		public void InsertBody(EntityUid uid, EntityUid user, MedicalScannerComponent scannerComponent)
		{
			if (!base.Resolve<MedicalScannerComponent>(uid, ref scannerComponent, true))
			{
				return;
			}
			if (scannerComponent.BodyContainer.ContainedEntity != null)
			{
				return;
			}
			if (!base.HasComp<MobStateComponent>(user))
			{
				return;
			}
			scannerComponent.BodyContainer.Insert(user, null, null, null, null, null);
			this.UpdateAppearance(uid, scannerComponent);
		}

		// Token: 0x0600137F RID: 4991 RVA: 0x00064C10 File Offset: 0x00062E10
		[NullableContext(2)]
		public void EjectBody(EntityUid uid, MedicalScannerComponent scannerComponent)
		{
			if (!base.Resolve<MedicalScannerComponent>(uid, ref scannerComponent, true))
			{
				return;
			}
			EntityUid? containedEntity = scannerComponent.BodyContainer.ContainedEntity;
			if (containedEntity != null)
			{
				EntityUid contained = containedEntity.GetValueOrDefault();
				if (contained.Valid)
				{
					scannerComponent.BodyContainer.Remove(contained, null, null, null, true, false, null, null);
					this._climbSystem.ForciblySetClimbing(contained, uid, null);
					this.UpdateAppearance(uid, scannerComponent);
					return;
				}
			}
		}

		// Token: 0x06001380 RID: 4992 RVA: 0x00064C8C File Offset: 0x00062E8C
		private void OnRefreshParts(EntityUid uid, MedicalScannerComponent component, RefreshPartsEvent args)
		{
			float ratingFail = args.PartRatings[component.MachinePartCloningFailChance];
			component.CloningFailChanceMultiplier = MathF.Pow(component.PartRatingFailMultiplier, ratingFail - 1f);
		}

		// Token: 0x06001381 RID: 4993 RVA: 0x00064CC3 File Offset: 0x00062EC3
		private void OnUpgradeExamine(EntityUid uid, MedicalScannerComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("medical-scanner-upgrade-cloning", component.CloningFailChanceMultiplier);
		}

		// Token: 0x04000BD5 RID: 3029
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;

		// Token: 0x04000BD6 RID: 3030
		[Dependency]
		private readonly ActionBlockerSystem _blocker;

		// Token: 0x04000BD7 RID: 3031
		[Dependency]
		private readonly ClimbSystem _climbSystem;

		// Token: 0x04000BD8 RID: 3032
		[Dependency]
		private readonly CloningConsoleSystem _cloningConsoleSystem;

		// Token: 0x04000BD9 RID: 3033
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000BDA RID: 3034
		[Dependency]
		private readonly ContainerSystem _containerSystem;

		// Token: 0x04000BDB RID: 3035
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000BDC RID: 3036
		private const float UpdateRate = 1f;

		// Token: 0x04000BDD RID: 3037
		private float _updateDif;
	}
}
