using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.ReactionEffects;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x02000043 RID: 67
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FoamArtifactSystem : EntitySystem
	{
		// Token: 0x060000CF RID: 207 RVA: 0x00006064 File Offset: 0x00004264
		public override void Initialize()
		{
			base.SubscribeLocalEvent<FoamArtifactComponent, ArtifactNodeEnteredEvent>(new ComponentEventHandler<FoamArtifactComponent, ArtifactNodeEnteredEvent>(this.OnNodeEntered), null, null);
			base.SubscribeLocalEvent<FoamArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<FoamArtifactComponent, ArtifactActivatedEvent>(this.OnActivated), null, null);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000608E File Offset: 0x0000428E
		private void OnNodeEntered(EntityUid uid, FoamArtifactComponent component, ArtifactNodeEnteredEvent args)
		{
			if (!component.Reagents.Any<string>())
			{
				return;
			}
			component.SelectedReagent = component.Reagents[args.RandomSeed % component.Reagents.Count];
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000060C4 File Offset: 0x000042C4
		private void OnActivated(EntityUid uid, FoamArtifactComponent component, ArtifactActivatedEvent args)
		{
			if (component.SelectedReagent == null)
			{
				return;
			}
			Solution sol = new Solution();
			TransformComponent xform = base.Transform(uid);
			sol.AddReagent(component.SelectedReagent, component.ReagentAmount, true);
			FoamAreaReactionEffect.SpawnFoam("Foam", xform.Coordinates, sol, this._random.Next(component.MinFoamAmount, component.MaxFoamAmount), component.Duration, component.SpreadDuration, component.SpreadDuration, null, this.EntityManager);
		}

		// Token: 0x04000099 RID: 153
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
