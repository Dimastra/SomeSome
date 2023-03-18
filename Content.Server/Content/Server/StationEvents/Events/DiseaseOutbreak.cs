using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Disease;
using Content.Server.Disease.Components;
using Content.Shared.Disease;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000186 RID: 390
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiseaseOutbreak : StationEventSystem
	{
		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060007B7 RID: 1975 RVA: 0x00026298 File Offset: 0x00024498
		public override string Prototype
		{
			get
			{
				return "DiseaseOutbreak";
			}
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x000262A0 File Offset: 0x000244A0
		public override void Started()
		{
			base.Started();
			HashSet<EntityUid> stationsToNotify = new HashSet<EntityUid>();
			List<DiseaseCarrierComponent> aliveList = new List<DiseaseCarrierComponent>();
			foreach (ValueTuple<DiseaseCarrierComponent, MobStateComponent> valueTuple in this.EntityManager.EntityQuery<DiseaseCarrierComponent, MobStateComponent>(false))
			{
				DiseaseCarrierComponent carrier = valueTuple.Item1;
				MobStateComponent mobState = valueTuple.Item2;
				if (!this._mobStateSystem.IsDead(mobState.Owner, mobState))
				{
					aliveList.Add(carrier);
				}
			}
			this.RobustRandom.Shuffle<DiseaseCarrierComponent>(aliveList);
			int toInfect = this.RobustRandom.Next(2, 5);
			string diseaseName = RandomExtensions.Pick<string>(this.RobustRandom, this.NotTooSeriousDiseases);
			DiseasePrototype disease;
			if (!this.PrototypeManager.TryIndex<DiseasePrototype>(diseaseName, ref disease))
			{
				return;
			}
			foreach (DiseaseCarrierComponent target in aliveList)
			{
				if (toInfect-- == 0)
				{
					break;
				}
				this._diseaseSystem.TryAddDisease(target.Owner, disease, target);
				EntityUid? station = this.StationSystem.GetOwningStation(target.Owner, null);
				if (station != null)
				{
					stationsToNotify.Add(station.Value);
				}
			}
		}

		// Token: 0x040004A7 RID: 1191
		[Dependency]
		private readonly DiseaseSystem _diseaseSystem;

		// Token: 0x040004A8 RID: 1192
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040004A9 RID: 1193
		public readonly IReadOnlyList<string> NotTooSeriousDiseases = new string[]
		{
			"SpaceCold",
			"VanAusdallsRobovirus",
			"VentCough",
			"AMIV",
			"SpaceFlu",
			"BirdFlew",
			"TongueTwister"
		};
	}
}
