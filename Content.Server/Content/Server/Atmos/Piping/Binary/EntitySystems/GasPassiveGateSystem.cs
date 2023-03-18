using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Binary.Components;
using Content.Server.Atmos.Piping.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Atmos.Piping.Binary.EntitySystems
{
	// Token: 0x02000768 RID: 1896
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasPassiveGateSystem : EntitySystem
	{
		// Token: 0x06002820 RID: 10272 RVA: 0x000D1F66 File Offset: 0x000D0166
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasPassiveGateComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasPassiveGateComponent, AtmosDeviceUpdateEvent>(this.OnPassiveGateUpdated), null, null);
			base.SubscribeLocalEvent<GasPassiveGateComponent, ExaminedEvent>(new ComponentEventHandler<GasPassiveGateComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x06002821 RID: 10273 RVA: 0x000D1F98 File Offset: 0x000D0198
		private void OnPassiveGateUpdated(EntityUid uid, GasPassiveGateComponent gate, AtmosDeviceUpdateEvent args)
		{
			NodeContainerComponent nodeContainer;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer))
			{
				return;
			}
			PipeNode inlet;
			PipeNode outlet;
			if (!nodeContainer.TryGetNode<PipeNode>(gate.InletName, out inlet) || !nodeContainer.TryGetNode<PipeNode>(gate.OutletName, out outlet))
			{
				return;
			}
			float n = inlet.Air.TotalMoles;
			float n2 = outlet.Air.TotalMoles;
			float P = inlet.Air.Pressure;
			float P2 = outlet.Air.Pressure;
			float V = inlet.Air.Volume;
			float V2 = outlet.Air.Volume;
			float T = inlet.Air.Temperature;
			float T2 = outlet.Air.Temperature;
			float num = P - P2;
			float dt = 1f / this._atmosphereSystem.AtmosTickRate;
			float dV = 0f;
			float denom = T * V2 + T2 * V;
			if (num > 0f && P > 0f && denom > 0f)
			{
				float transferMoles = n - (n + n2) * T2 * V / denom;
				dV = n * 8.314463f * T / P;
				this._atmosphereSystem.Merge(outlet.Air, inlet.Air.Remove(transferMoles));
			}
			int tau = 1;
			float a = dt / (float)tau;
			gate.FlowRate = a * dV / (float)tau + (1f - a) * gate.FlowRate;
		}

		// Token: 0x06002822 RID: 10274 RVA: 0x000D20EC File Offset: 0x000D02EC
		private void OnExamined(EntityUid uid, GasPassiveGateComponent gate, ExaminedEvent args)
		{
			if (!this.EntityManager.GetComponent<TransformComponent>(gate.Owner).Anchored || !args.IsInDetailsRange)
			{
				return;
			}
			string text = "gas-passive-gate-examined";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[1];
			int num = 0;
			string item = "flowRate";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(gate.FlowRate, "0.#");
			array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
			string str = Loc.GetString(text, array);
			args.PushMarkup(str);
		}

		// Token: 0x040018F5 RID: 6389
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;
	}
}
