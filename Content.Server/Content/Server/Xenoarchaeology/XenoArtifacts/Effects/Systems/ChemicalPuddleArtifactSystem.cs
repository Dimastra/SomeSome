using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Fluids.EntitySystems;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x02000040 RID: 64
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChemicalPuddleArtifactSystem : EntitySystem
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x00005CFC File Offset: 0x00003EFC
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ChemicalPuddleArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<ChemicalPuddleArtifactComponent, ArtifactActivatedEvent>(this.OnActivated), null, null);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00005D14 File Offset: 0x00003F14
		private void OnActivated(EntityUid uid, ChemicalPuddleArtifactComponent component, ArtifactActivatedEvent args)
		{
			ArtifactComponent artifact;
			if (!base.TryComp<ArtifactComponent>(uid, ref artifact))
			{
				return;
			}
			List<string> chemicalList;
			if (!this._artifact.TryGetNodeData<List<string>>(uid, "nodeDataSpawnAmount", out chemicalList, artifact))
			{
				chemicalList = new List<string>();
				for (int i = 0; i < component.ChemAmount; i++)
				{
					string chemProto = RandomExtensions.Pick<string>(this._random, component.PossibleChemicals);
					chemicalList.Add(chemProto);
				}
				this._artifact.SetNodeData(uid, "nodeDataSpawnAmount", chemicalList, artifact);
			}
			FixedPoint2 amountPerChem = component.ChemicalSolution.MaxVolume / (float)component.ChemAmount;
			foreach (string reagent in chemicalList)
			{
				component.ChemicalSolution.AddReagent(reagent, amountPerChem, true);
			}
			this._spillable.SpillAt(uid, component.ChemicalSolution, component.PuddlePrototype, true, true, null);
		}

		// Token: 0x0400008E RID: 142
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400008F RID: 143
		[Dependency]
		private readonly ArtifactSystem _artifact;

		// Token: 0x04000090 RID: 144
		[Dependency]
		private readonly SpillableSystem _spillable;

		// Token: 0x04000091 RID: 145
		public const string NodeDataChemicalList = "nodeDataSpawnAmount";
	}
}
