using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.Chemistry.ReactionEffects;
using Content.Server.Station.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000193 RID: 403
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VentClog : StationEventSystem
	{
		// Token: 0x17000160 RID: 352
		// (get) Token: 0x060007F7 RID: 2039 RVA: 0x00027C0B File Offset: 0x00025E0B
		public override string Prototype
		{
			get
			{
				return "VentClog";
			}
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x00027C14 File Offset: 0x00025E14
		public override void Started()
		{
			base.Started();
			if (this.StationSystem.Stations.Count == 0)
			{
				return;
			}
			EntityUid chosenStation = RandomExtensions.Pick<EntityUid>(this.RobustRandom, this.StationSystem.Stations.ToList<EntityUid>());
			List<string> allReagents = (from x in this.PrototypeManager.EnumeratePrototypes<ReagentPrototype>()
			where !x.Abstract
			select x.ID).ToList<string>();
			SoundPathSpecifier sound = new SoundPathSpecifier("/Audio/Effects/extinguish.ogg", null);
			float mod = (float)Math.Sqrt((double)base.GetSeverityModifier());
			foreach (ValueTuple<GasVentPumpComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<GasVentPumpComponent, TransformComponent>(false))
			{
				TransformComponent transform = valueTuple.Item2;
				StationMemberComponent stationMemberComponent = base.CompOrNull<StationMemberComponent>(transform.GridUid);
				if (stationMemberComponent != null && !(stationMemberComponent.Station != chosenStation))
				{
					Solution solution = new Solution();
					if (RandomExtensions.Prob(this.RobustRandom, Math.Min(0.33f * mod, 1f)))
					{
						if (RandomExtensions.Prob(this.RobustRandom, Math.Min(0.05f * mod, 1f)))
						{
							solution.AddReagent(RandomExtensions.Pick<string>(this.RobustRandom, allReagents), 100, true);
						}
						else
						{
							solution.AddReagent(RandomExtensions.Pick<string>(this.RobustRandom, this.SafeishVentChemicals), 100, true);
						}
						FoamAreaReactionEffect.SpawnFoam("Foam", transform.Coordinates, solution, (int)((float)this.RobustRandom.Next(2, 6) * mod), 20f, 1f, 1f, sound, this.EntityManager);
					}
				}
			}
		}

		// Token: 0x040004E0 RID: 1248
		public readonly IReadOnlyList<string> SafeishVentChemicals = new string[]
		{
			"Water",
			"Blood",
			"Slime",
			"Acetone",
			"SpaceDrugs",
			"SpaceCleaner",
			"Nutriment",
			"Sugar",
			"SpaceLube",
			"Ethanol",
			"Ephedrine",
			"WeldingFuel",
			"VentCrud",
			"Ale",
			"Beer"
		};
	}
}
