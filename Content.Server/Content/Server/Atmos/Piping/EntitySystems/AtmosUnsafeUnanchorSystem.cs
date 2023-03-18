using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Popups;
using Content.Shared.Construction.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Atmos.Piping.EntitySystems
{
	// Token: 0x02000761 RID: 1889
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosUnsafeUnanchorSystem : EntitySystem
	{
		// Token: 0x06002807 RID: 10247 RVA: 0x000D1C32 File Offset: 0x000CFE32
		public override void Initialize()
		{
			base.SubscribeLocalEvent<AtmosUnsafeUnanchorComponent, BeforeUnanchoredEvent>(new ComponentEventHandler<AtmosUnsafeUnanchorComponent, BeforeUnanchoredEvent>(this.OnBeforeUnanchored), null, null);
			base.SubscribeLocalEvent<AtmosUnsafeUnanchorComponent, UnanchorAttemptEvent>(new ComponentEventHandler<AtmosUnsafeUnanchorComponent, UnanchorAttemptEvent>(this.OnUnanchorAttempt), null, null);
		}

		// Token: 0x06002808 RID: 10248 RVA: 0x000D1C5C File Offset: 0x000CFE5C
		private void OnUnanchorAttempt(EntityUid uid, AtmosUnsafeUnanchorComponent component, UnanchorAttemptEvent args)
		{
			NodeContainerComponent nodes;
			if (!component.Enabled || !this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodes))
			{
				return;
			}
			GasMixture environment = this._atmosphere.GetContainingMixture(uid, true, false, null);
			if (environment == null)
			{
				return;
			}
			foreach (Node node in nodes.Nodes.Values)
			{
				PipeNode pipe = node as PipeNode;
				if (pipe != null && pipe.Air.Pressure - environment.Pressure > 202.65f)
				{
					args.Delay += 2f;
					this._popup.PopupEntity(Loc.GetString("comp-atmos-unsafe-unanchor-warning"), pipe.Owner, args.User, PopupType.MediumCaution);
					break;
				}
			}
		}

		// Token: 0x06002809 RID: 10249 RVA: 0x000D1D34 File Offset: 0x000CFF34
		private void OnBeforeUnanchored(EntityUid uid, AtmosUnsafeUnanchorComponent component, BeforeUnanchoredEvent args)
		{
			NodeContainerComponent nodes;
			if (!component.Enabled || !this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodes))
			{
				return;
			}
			GasMixture environment = this._atmosphere.GetContainingMixture(uid, true, true, null);
			if (environment == null)
			{
				environment = GasMixture.SpaceGas;
			}
			float lost = 0f;
			int timesLost = 0;
			foreach (Node node in nodes.Nodes.Values)
			{
				PipeNode pipe = node as PipeNode;
				if (pipe != null)
				{
					float difference = pipe.Air.Pressure - environment.Pressure;
					lost += difference * environment.Volume / (environment.Temperature * 8.314463f);
					timesLost++;
				}
			}
			float sharedLoss = lost / (float)timesLost;
			GasMixture buffer = new GasMixture();
			foreach (Node node2 in nodes.Nodes.Values)
			{
				PipeNode pipe2 = node2 as PipeNode;
				if (pipe2 != null)
				{
					this._atmosphere.Merge(buffer, pipe2.Air.Remove(sharedLoss));
				}
			}
			this._atmosphere.Merge(environment, buffer);
		}

		// Token: 0x040018EC RID: 6380
		[Dependency]
		private readonly AtmosphereSystem _atmosphere;

		// Token: 0x040018ED RID: 6381
		[Dependency]
		private readonly PopupSystem _popup;
	}
}
