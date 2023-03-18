using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Popups;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Prototypes;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x0200079B RID: 1947
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasAnalyzerSystem : EntitySystem
	{
		// Token: 0x06002A20 RID: 10784 RVA: 0x000DDFB4 File Offset: 0x000DC1B4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasAnalyzerComponent, AfterInteractEvent>(new ComponentEventHandler<GasAnalyzerComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<GasAnalyzerComponent, SharedGasAnalyzerComponent.GasAnalyzerDisableMessage>(new ComponentEventHandler<GasAnalyzerComponent, SharedGasAnalyzerComponent.GasAnalyzerDisableMessage>(this.OnDisabledMessage), null, null);
			base.SubscribeLocalEvent<GasAnalyzerComponent, DroppedEvent>(new ComponentEventHandler<GasAnalyzerComponent, DroppedEvent>(this.OnDropped), null, null);
			base.SubscribeLocalEvent<GasAnalyzerComponent, UseInHandEvent>(new ComponentEventHandler<GasAnalyzerComponent, UseInHandEvent>(this.OnUseInHand), null, null);
		}

		// Token: 0x06002A21 RID: 10785 RVA: 0x000DE018 File Offset: 0x000DC218
		public override void Update(float frameTime)
		{
			foreach (ActiveGasAnalyzerComponent analyzer in base.EntityQuery<ActiveGasAnalyzerComponent>(false))
			{
				analyzer.AccumulatedFrametime += frameTime;
				if (analyzer.AccumulatedFrametime >= analyzer.UpdateInterval)
				{
					analyzer.AccumulatedFrametime -= analyzer.UpdateInterval;
					if (!this.UpdateAnalyzer(analyzer.Owner, null))
					{
						base.RemCompDeferred<ActiveGasAnalyzerComponent>(analyzer.Owner);
					}
				}
			}
		}

		// Token: 0x06002A22 RID: 10786 RVA: 0x000DE0AC File Offset: 0x000DC2AC
		private void OnAfterInteract(EntityUid uid, GasAnalyzerComponent component, AfterInteractEvent args)
		{
			if (!args.CanReach)
			{
				this._popup.PopupEntity(Loc.GetString("gas-analyzer-component-player-cannot-reach-message"), args.User, args.User, PopupType.Small);
				return;
			}
			this.ActivateAnalyzer(uid, component, args.User, args.Target);
			this.OpenUserInterface(args.User, component);
			args.Handled = true;
		}

		// Token: 0x06002A23 RID: 10787 RVA: 0x000DE10C File Offset: 0x000DC30C
		private void OnUseInHand(EntityUid uid, GasAnalyzerComponent component, UseInHandEvent args)
		{
			this.ActivateAnalyzer(uid, component, args.User, null);
			args.Handled = true;
		}

		// Token: 0x06002A24 RID: 10788 RVA: 0x000DE138 File Offset: 0x000DC338
		private void ActivateAnalyzer(EntityUid uid, GasAnalyzerComponent component, EntityUid user, EntityUid? target = null)
		{
			component.Target = target;
			component.User = user;
			if (target != null)
			{
				component.LastPosition = new EntityCoordinates?(base.Transform(target.Value).Coordinates);
			}
			else
			{
				component.LastPosition = null;
			}
			component.Enabled = true;
			base.Dirty(component, null);
			this.UpdateAppearance(component);
			if (!base.HasComp<ActiveGasAnalyzerComponent>(uid))
			{
				base.AddComp<ActiveGasAnalyzerComponent>(uid);
			}
			this.UpdateAnalyzer(uid, component);
		}

		// Token: 0x06002A25 RID: 10789 RVA: 0x000DE1B8 File Offset: 0x000DC3B8
		private void OnDropped(EntityUid uid, GasAnalyzerComponent component, DroppedEvent args)
		{
			EntityUid userId = args.User;
			if (component.Enabled)
			{
				this._popup.PopupEntity(Loc.GetString("gas-analyzer-shutoff"), userId, userId, PopupType.Small);
			}
			this.DisableAnalyzer(uid, component, new EntityUid?(args.User));
		}

		// Token: 0x06002A26 RID: 10790 RVA: 0x000DE200 File Offset: 0x000DC400
		[NullableContext(2)]
		private void DisableAnalyzer(EntityUid uid, GasAnalyzerComponent component = null, EntityUid? user = null)
		{
			if (!base.Resolve<GasAnalyzerComponent>(uid, ref component, true))
			{
				return;
			}
			ActorComponent actor;
			if (user != null && base.TryComp<ActorComponent>(user, ref actor))
			{
				this._userInterface.TryClose(uid, SharedGasAnalyzerComponent.GasAnalyzerUiKey.Key, actor.PlayerSession, null);
			}
			component.Enabled = false;
			base.Dirty(component, null);
			this.UpdateAppearance(component);
			base.RemCompDeferred<ActiveGasAnalyzerComponent>(uid);
		}

		// Token: 0x06002A27 RID: 10791 RVA: 0x000DE268 File Offset: 0x000DC468
		private void OnDisabledMessage(EntityUid uid, GasAnalyzerComponent component, SharedGasAnalyzerComponent.GasAnalyzerDisableMessage message)
		{
			EntityUid? attachedEntity = message.Session.AttachedEntity;
			if (attachedEntity == null || !attachedEntity.GetValueOrDefault().Valid)
			{
				return;
			}
			this.DisableAnalyzer(uid, component, null);
		}

		// Token: 0x06002A28 RID: 10792 RVA: 0x000DE2B0 File Offset: 0x000DC4B0
		private void OpenUserInterface(EntityUid user, GasAnalyzerComponent component)
		{
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(user, ref actor))
			{
				return;
			}
			this._userInterface.TryOpen(component.Owner, SharedGasAnalyzerComponent.GasAnalyzerUiKey.Key, actor.PlayerSession, null);
		}

		// Token: 0x06002A29 RID: 10793 RVA: 0x000DE2E8 File Offset: 0x000DC4E8
		[NullableContext(2)]
		private bool UpdateAnalyzer(EntityUid uid, GasAnalyzerComponent component = null)
		{
			if (!base.Resolve<GasAnalyzerComponent>(uid, ref component, true))
			{
				return false;
			}
			TransformComponent xform;
			if (!base.TryComp<TransformComponent>(component.User, ref xform))
			{
				this.DisableAnalyzer(uid, component, null);
				return false;
			}
			EntityCoordinates userPos = xform.Coordinates;
			if (component.LastPosition != null && !component.LastPosition.Value.InRange(this.EntityManager, userPos, 1.5f))
			{
				EntityUid userId = component.User;
				if (component.Enabled)
				{
					this._popup.PopupEntity(Loc.GetString("gas-analyzer-shutoff"), userId, userId, PopupType.Small);
				}
				this.DisableAnalyzer(uid, component, new EntityUid?(component.User));
				return false;
			}
			List<SharedGasAnalyzerComponent.GasMixEntry> gasMixList = new List<SharedGasAnalyzerComponent.GasMixEntry>();
			GasMixture tileMixture = this._atmo.GetContainingMixture(component.Owner, true, false, null);
			if (tileMixture != null)
			{
				gasMixList.Add(new SharedGasAnalyzerComponent.GasMixEntry(Loc.GetString("gas-analyzer-window-environment-tab-label"), tileMixture.Pressure, tileMixture.Temperature, this.GenerateGasEntryArray(tileMixture)));
			}
			else
			{
				gasMixList.Add(new SharedGasAnalyzerComponent.GasMixEntry(Loc.GetString("gas-analyzer-window-environment-tab-label"), 0f, 0f, null));
			}
			bool deviceFlipped = false;
			if (component.Target != null)
			{
				if (base.Deleted(component.Target))
				{
					component.Target = null;
					this.DisableAnalyzer(uid, component, new EntityUid?(component.User));
					return false;
				}
				GasAnalyzerScanEvent ev = new GasAnalyzerScanEvent();
				base.RaiseLocalEvent<GasAnalyzerScanEvent>(component.Target.Value, ev, false);
				NodeContainerComponent node;
				if (ev.GasMixtures != null)
				{
					foreach (KeyValuePair<string, GasMixture> mixes in ev.GasMixtures)
					{
						if (mixes.Value != null)
						{
							gasMixList.Add(new SharedGasAnalyzerComponent.GasMixEntry(mixes.Key, mixes.Value.Pressure, mixes.Value.Temperature, this.GenerateGasEntryArray(mixes.Value)));
						}
					}
					deviceFlipped = ev.DeviceFlipped;
				}
				else if (base.TryComp<NodeContainerComponent>(component.Target, ref node))
				{
					foreach (KeyValuePair<string, Node> pair in node.Nodes)
					{
						PipeNode pipeNode = pair.Value as PipeNode;
						if (pipeNode != null)
						{
							gasMixList.Add(new SharedGasAnalyzerComponent.GasMixEntry(pair.Key, pipeNode.Air.Pressure, pipeNode.Air.Temperature, this.GenerateGasEntryArray(pipeNode.Air)));
						}
					}
				}
			}
			if (gasMixList.Count == 0)
			{
				return false;
			}
			this._userInterface.TrySendUiMessage(component.Owner, SharedGasAnalyzerComponent.GasAnalyzerUiKey.Key, new SharedGasAnalyzerComponent.GasAnalyzerUserMessage(gasMixList.ToArray(), (component.Target != null) ? base.Name(component.Target.Value, null) : string.Empty, component.Target ?? EntityUid.Invalid, deviceFlipped, null), null);
			return true;
		}

		// Token: 0x06002A2A RID: 10794 RVA: 0x000DE60C File Offset: 0x000DC80C
		private void UpdateAppearance(GasAnalyzerComponent analyzer)
		{
			this._appearance.SetData(analyzer.Owner, GasAnalyzerVisuals.Enabled, analyzer.Enabled, null);
		}

		// Token: 0x06002A2B RID: 10795 RVA: 0x000DE634 File Offset: 0x000DC834
		private SharedGasAnalyzerComponent.GasEntry[] GenerateGasEntryArray([Nullable(2)] GasMixture mixture)
		{
			List<SharedGasAnalyzerComponent.GasEntry> gases = new List<SharedGasAnalyzerComponent.GasEntry>();
			for (int i = 0; i < 9; i++)
			{
				GasPrototype gas = this._atmo.GetGas(i);
				if ((mixture == null || mixture.Moles[i] > 5E-08f) && mixture != null)
				{
					string gasName = Loc.GetString(gas.Name);
					gases.Add(new SharedGasAnalyzerComponent.GasEntry(gasName, mixture.Moles[i], gas.Color));
				}
			}
			return gases.ToArray();
		}

		// Token: 0x04001A0F RID: 6671
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x04001A10 RID: 6672
		[Dependency]
		private readonly AtmosphereSystem _atmo;

		// Token: 0x04001A11 RID: 6673
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04001A12 RID: 6674
		[Dependency]
		private readonly UserInterfaceSystem _userInterface;
	}
}
