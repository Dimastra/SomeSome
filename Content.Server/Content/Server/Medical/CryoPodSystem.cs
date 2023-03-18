using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Climbing;
using Content.Server.DoAfter;
using Content.Server.Medical.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.UserInterface;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Medical.Cryogenics;
using Content.Shared.MedicalScanner;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Server.Medical
{
	// Token: 0x020003AE RID: 942
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CryoPodSystem : SharedCryoPodSystem
	{
		// Token: 0x0600134F RID: 4943 RVA: 0x0006347C File Offset: 0x0006167C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CryoPodComponent, CanDropTargetEvent>(new ComponentEventRefHandler<CryoPodComponent, CanDropTargetEvent>(base.OnCryoPodCanDropOn), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, ComponentInit>(new ComponentEventHandler<CryoPodComponent, ComponentInit>(base.OnComponentInit), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<CryoPodComponent, GetVerbsEvent<AlternativeVerb>>(base.AddAlternativeVerbs), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, GotEmaggedEvent>(new ComponentEventRefHandler<CryoPodComponent, GotEmaggedEvent>(base.OnEmagged), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, DoAfterEvent>(new ComponentEventHandler<CryoPodComponent, DoAfterEvent>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, SharedCryoPodSystem.CryoPodPryFinished>(new ComponentEventHandler<CryoPodComponent, SharedCryoPodSystem.CryoPodPryFinished>(base.OnCryoPodPryFinished), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, SharedCryoPodSystem.CryoPodPryInterrupted>(new ComponentEventHandler<CryoPodComponent, SharedCryoPodSystem.CryoPodPryInterrupted>(base.OnCryoPodPryInterrupted), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<CryoPodComponent, AtmosDeviceUpdateEvent>(this.OnCryoPodUpdateAtmosphere), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, DragDropTargetEvent>(new ComponentEventRefHandler<CryoPodComponent, DragDropTargetEvent>(this.HandleDragDropOn), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, InteractUsingEvent>(new ComponentEventHandler<CryoPodComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, ExaminedEvent>(new ComponentEventHandler<CryoPodComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, PowerChangedEvent>(new ComponentEventRefHandler<CryoPodComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, GasAnalyzerScanEvent>(new ComponentEventHandler<CryoPodComponent, GasAnalyzerScanEvent>(this.OnGasAnalyzed), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<CryoPodComponent, ActivatableUIOpenAttemptEvent>(this.OnActivateUIAttempt), null, null);
			base.SubscribeLocalEvent<CryoPodComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<CryoPodComponent, AfterActivatableUIOpenEvent>(this.OnActivateUI), null, null);
		}

		// Token: 0x06001350 RID: 4944 RVA: 0x000635BC File Offset: 0x000617BC
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			TimeSpan curTime = this._gameTiming.CurTime;
			EntityQuery<BloodstreamComponent> bloodStreamQuery = base.GetEntityQuery<BloodstreamComponent>();
			EntityQuery<MetaDataComponent> metaDataQuery = base.GetEntityQuery<MetaDataComponent>();
			EntityQuery<ItemSlotsComponent> itemSlotsQuery = base.GetEntityQuery<ItemSlotsComponent>();
			EntityQuery<FitsInDispenserComponent> fitsInDispenserQuery = base.GetEntityQuery<FitsInDispenserComponent>();
			EntityQuery<SolutionContainerManagerComponent> solutionContainerManagerQuery = base.GetEntityQuery<SolutionContainerManagerComponent>();
			foreach (ValueTuple<ActiveCryoPodComponent, CryoPodComponent> valueTuple in base.EntityQuery<ActiveCryoPodComponent, CryoPodComponent>(false))
			{
				CryoPodComponent cryoPod = valueTuple.Item2;
				MetaDataComponent metaDataComponent;
				metaDataQuery.TryGetComponent(cryoPod.Owner, ref metaDataComponent);
				TimeSpan t = curTime;
				if (!(t < cryoPod.NextInjectionTime + this._metaDataSystem.GetPauseTime(cryoPod.Owner, metaDataComponent)))
				{
					cryoPod.NextInjectionTime = new TimeSpan?(curTime + TimeSpan.FromSeconds((double)cryoPod.BeakerTransferTime));
					ItemSlotsComponent itemSlotsComponent;
					if (itemSlotsQuery.TryGetComponent(cryoPod.Owner, ref itemSlotsComponent))
					{
						EntityUid? container = this._itemSlotsSystem.GetItemOrNull(cryoPod.Owner, cryoPod.SolutionContainerName, itemSlotsComponent);
						EntityUid? patient = cryoPod.BodyContainer.ContainedEntity;
						FitsInDispenserComponent fitsInDispenserComponent;
						SolutionContainerManagerComponent solutionContainerManagerComponent;
						Solution containerSolution;
						BloodstreamComponent bloodstream;
						if (container != null && container.Value.Valid && patient != null && fitsInDispenserQuery.TryGetComponent(container, ref fitsInDispenserComponent) && solutionContainerManagerQuery.TryGetComponent(container, ref solutionContainerManagerComponent) && this._solutionContainerSystem.TryGetFitsInDispenser(container.Value, out containerSolution, fitsInDispenserComponent, solutionContainerManagerComponent) && bloodStreamQuery.TryGetComponent(patient, ref bloodstream))
						{
							Solution solutionToInject = this._solutionContainerSystem.SplitSolution(container.Value, containerSolution, cryoPod.BeakerTransferAmount);
							this._bloodstreamSystem.TryAddToChemicals(patient.Value, solutionToInject, bloodstream);
							this._reactiveSystem.DoEntityReaction(patient.Value, solutionToInject, ReactionMethod.Injection);
						}
					}
				}
			}
		}

		// Token: 0x06001351 RID: 4945 RVA: 0x000637F4 File Offset: 0x000619F4
		[NullableContext(2)]
		public override void EjectBody(EntityUid uid, SharedCryoPodComponent cryoPodComponent)
		{
			if (!base.Resolve<SharedCryoPodComponent>(uid, ref cryoPodComponent, true))
			{
				return;
			}
			EntityUid? containedEntity = cryoPodComponent.BodyContainer.ContainedEntity;
			if (containedEntity != null)
			{
				EntityUid contained = containedEntity.GetValueOrDefault();
				if (contained.Valid)
				{
					base.EjectBody(uid, cryoPodComponent);
					this._climbSystem.ForciblySetClimbing(contained, uid, null);
					return;
				}
			}
		}

		// Token: 0x06001352 RID: 4946 RVA: 0x0006384C File Offset: 0x00061A4C
		private void HandleDragDropOn(EntityUid uid, CryoPodComponent cryoPodComponent, ref DragDropTargetEvent args)
		{
			EntityUid? containedEntity = cryoPodComponent.BodyContainer.ContainedEntity;
			if (containedEntity != null)
			{
				return;
			}
			EntityUid user = args.User;
			float entryDelay = cryoPodComponent.EntryDelay;
			containedEntity = new EntityUid?(args.Dragged);
			EntityUid? used = new EntityUid?(uid);
			DoAfterEventArgs doAfterArgs = new DoAfterEventArgs(user, entryDelay, default(CancellationToken), containedEntity, used)
			{
				BreakOnDamage = true,
				BreakOnStun = true,
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				NeedHand = false
			};
			this._doAfterSystem.DoAfter(doAfterArgs);
			args.Handled = true;
		}

		// Token: 0x06001353 RID: 4947 RVA: 0x000638D8 File Offset: 0x00061AD8
		private void OnDoAfter(EntityUid uid, CryoPodComponent component, DoAfterEvent args)
		{
			if (args.Cancelled || args.Handled || args.Args.Target == null)
			{
				return;
			}
			base.InsertBody(uid, args.Args.Target.Value, component);
			args.Handled = true;
		}

		// Token: 0x06001354 RID: 4948 RVA: 0x00063928 File Offset: 0x00061B28
		private void OnActivateUIAttempt(EntityUid uid, CryoPodComponent cryoPodComponent, ActivatableUIOpenAttemptEvent args)
		{
			if (args.Cancelled)
			{
				return;
			}
			EntityUid? containedEntity = cryoPodComponent.BodyContainer.ContainedEntity;
			if (containedEntity != null)
			{
				EntityUid? entityUid = containedEntity;
				EntityUid user = args.User;
				if ((entityUid == null || (entityUid != null && !(entityUid.GetValueOrDefault() == user))) && base.HasComp<ActiveCryoPodComponent>(uid))
				{
					return;
				}
			}
			args.Cancel();
		}

		// Token: 0x06001355 RID: 4949 RVA: 0x00063992 File Offset: 0x00061B92
		private void OnActivateUI(EntityUid uid, CryoPodComponent cryoPodComponent, AfterActivatableUIOpenEvent args)
		{
			this._userInterfaceSystem.TrySendUiMessage(uid, SharedHealthAnalyzerComponent.HealthAnalyzerUiKey.Key, new SharedHealthAnalyzerComponent.HealthAnalyzerScannedUserMessage(cryoPodComponent.BodyContainer.ContainedEntity), null);
		}

		// Token: 0x06001356 RID: 4950 RVA: 0x000639B8 File Offset: 0x00061BB8
		private void OnInteractUsing(EntityUid uid, CryoPodComponent cryoPodComponent, InteractUsingEvent args)
		{
			if (args.Handled || !cryoPodComponent.Locked || cryoPodComponent.BodyContainer.ContainedEntity == null)
			{
				return;
			}
			ToolComponent tool;
			if (base.TryComp<ToolComponent>(args.Used, ref tool) && tool.Qualities.Contains("Prying"))
			{
				if (cryoPodComponent.IsPrying)
				{
					return;
				}
				cryoPodComponent.IsPrying = true;
				ToolEventData toolEvData = new ToolEventData(new SharedCryoPodSystem.CryoPodPryFinished(), 0f, null, new EntityUid?(uid));
				this._toolSystem.UseTool(args.Used, args.User, new EntityUid?(uid), cryoPodComponent.PryDelay, new string[]
				{
					"Prying"
				}, toolEvData, 0f, null, null, null);
				args.Handled = true;
			}
		}

		// Token: 0x06001357 RID: 4951 RVA: 0x00063A78 File Offset: 0x00061C78
		private void OnExamined(EntityUid uid, CryoPodComponent component, ExaminedEvent args)
		{
			EntityUid? container = this._itemSlotsSystem.GetItemOrNull(uid, component.SolutionContainerName, null);
			Solution containerSolution;
			if (args.IsInDetailsRange && container != null && this._solutionContainerSystem.TryGetFitsInDispenser(container.Value, out containerSolution, null, null))
			{
				args.PushMarkup(Loc.GetString("cryo-pod-examine", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("beaker", base.Name(container.Value, null))
				}));
				if (containerSolution.Volume == 0)
				{
					args.PushMarkup(Loc.GetString("cryo-pod-empty-beaker"));
				}
			}
		}

		// Token: 0x06001358 RID: 4952 RVA: 0x00063B18 File Offset: 0x00061D18
		private void OnPowerChanged(EntityUid uid, CryoPodComponent component, ref PowerChangedEvent args)
		{
			if (base.Terminating(uid, null))
			{
				return;
			}
			if (args.Powered)
			{
				base.EnsureComp<ActiveCryoPodComponent>(uid);
			}
			else
			{
				base.RemComp<ActiveCryoPodComponent>(uid);
				this._uiSystem.TryCloseAll(uid, SharedHealthAnalyzerComponent.HealthAnalyzerUiKey.Key, null);
			}
			base.UpdateAppearance(uid, component, null);
		}

		// Token: 0x06001359 RID: 4953 RVA: 0x00063B68 File Offset: 0x00061D68
		private void OnCryoPodUpdateAtmosphere(EntityUid uid, CryoPodComponent cryoPod, AtmosDeviceUpdateEvent args)
		{
			NodeContainerComponent nodeContainer;
			if (!base.TryComp<NodeContainerComponent>(uid, ref nodeContainer))
			{
				return;
			}
			PortablePipeNode portNode;
			if (!nodeContainer.TryGetNode<PortablePipeNode>(cryoPod.PortName, out portNode))
			{
				return;
			}
			this._atmosphereSystem.React(cryoPod.Air, portNode);
			PipeNet net = portNode.NodeGroup as PipeNet;
			if (net != null && net.NodeCount > 1)
			{
				this._gasCanisterSystem.MixContainerWithPipeNet(cryoPod.Air, net.Air);
			}
		}

		// Token: 0x0600135A RID: 4954 RVA: 0x00063BD4 File Offset: 0x00061DD4
		private void OnGasAnalyzed(EntityUid uid, CryoPodComponent component, GasAnalyzerScanEvent args)
		{
			Dictionary<string, GasMixture> gasMixDict = new Dictionary<string, GasMixture>
			{
				{
					base.Name(uid, null),
					component.Air
				}
			};
			NodeContainerComponent nodeContainer;
			PipeNode port;
			if (base.TryComp<NodeContainerComponent>(uid, ref nodeContainer) && nodeContainer.TryGetNode<PipeNode>(component.PortName, out port))
			{
				gasMixDict.Add(component.PortName, port.Air);
			}
			args.GasMixtures = gasMixDict;
		}

		// Token: 0x0600135B RID: 4955 RVA: 0x00063C30 File Offset: 0x00061E30
		public override void InitializeInsideCryoPod()
		{
			base.InitializeInsideCryoPod();
			base.SubscribeLocalEvent<InsideCryoPodComponent, InhaleLocationEvent>(new ComponentEventHandler<InsideCryoPodComponent, InhaleLocationEvent>(this.OnInhaleLocation), null, null);
			base.SubscribeLocalEvent<InsideCryoPodComponent, ExhaleLocationEvent>(new ComponentEventHandler<InsideCryoPodComponent, ExhaleLocationEvent>(this.OnExhaleLocation), null, null);
			base.SubscribeLocalEvent<InsideCryoPodComponent, AtmosExposedGetAirEvent>(new ComponentEventRefHandler<InsideCryoPodComponent, AtmosExposedGetAirEvent>(this.OnGetAir), null, null);
		}

		// Token: 0x0600135C RID: 4956 RVA: 0x00063C80 File Offset: 0x00061E80
		private void OnGetAir(EntityUid uid, InsideCryoPodComponent component, ref AtmosExposedGetAirEvent args)
		{
			CryoPodComponent cryoPodComponent;
			if (base.TryComp<CryoPodComponent>(base.Transform(uid).ParentUid, ref cryoPodComponent))
			{
				args.Gas = cryoPodComponent.Air;
				args.Handled = true;
			}
		}

		// Token: 0x0600135D RID: 4957 RVA: 0x00063CB8 File Offset: 0x00061EB8
		private void OnInhaleLocation(EntityUid uid, InsideCryoPodComponent component, InhaleLocationEvent args)
		{
			CryoPodComponent cryoPodComponent;
			if (base.TryComp<CryoPodComponent>(base.Transform(uid).ParentUid, ref cryoPodComponent))
			{
				args.Gas = cryoPodComponent.Air;
			}
		}

		// Token: 0x0600135E RID: 4958 RVA: 0x00063CE8 File Offset: 0x00061EE8
		private void OnExhaleLocation(EntityUid uid, InsideCryoPodComponent component, ExhaleLocationEvent args)
		{
			CryoPodComponent cryoPodComponent;
			if (base.TryComp<CryoPodComponent>(base.Transform(uid).ParentUid, ref cryoPodComponent))
			{
				args.Gas = cryoPodComponent.Air;
			}
		}

		// Token: 0x04000BB9 RID: 3001
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04000BBA RID: 3002
		[Dependency]
		private readonly GasCanisterSystem _gasCanisterSystem;

		// Token: 0x04000BBB RID: 3003
		[Dependency]
		private readonly ClimbSystem _climbSystem;

		// Token: 0x04000BBC RID: 3004
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;

		// Token: 0x04000BBD RID: 3005
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x04000BBE RID: 3006
		[Dependency]
		private readonly BloodstreamSystem _bloodstreamSystem;

		// Token: 0x04000BBF RID: 3007
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x04000BC0 RID: 3008
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000BC1 RID: 3009
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x04000BC2 RID: 3010
		[Dependency]
		private readonly SharedToolSystem _toolSystem;

		// Token: 0x04000BC3 RID: 3011
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000BC4 RID: 3012
		[Dependency]
		private readonly MetaDataSystem _metaDataSystem;

		// Token: 0x04000BC5 RID: 3013
		[Dependency]
		private readonly ReactiveSystem _reactiveSystem;
	}
}
