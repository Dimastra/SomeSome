using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Disease;
using Content.Server.Disease.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Disease;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x02000042 RID: 66
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiseaseArtifactSystem : EntitySystem
	{
		// Token: 0x060000CB RID: 203 RVA: 0x00005F00 File Offset: 0x00004100
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DiseaseArtifactComponent, ArtifactNodeEnteredEvent>(new ComponentEventHandler<DiseaseArtifactComponent, ArtifactNodeEnteredEvent>(this.OnNodeEntered), null, null);
			base.SubscribeLocalEvent<DiseaseArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<DiseaseArtifactComponent, ArtifactActivatedEvent>(this.OnActivate), null, null);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00005F30 File Offset: 0x00004130
		private void OnNodeEntered(EntityUid uid, DiseaseArtifactComponent component, ArtifactNodeEnteredEvent args)
		{
			if (component.SpawnDisease != null || !component.DiseasePrototypes.Any<string>())
			{
				return;
			}
			string diseaseName = component.DiseasePrototypes[args.RandomSeed % component.DiseasePrototypes.Count];
			DiseasePrototype disease;
			if (!this._prototypeManager.TryIndex<DiseasePrototype>(diseaseName, ref disease))
			{
				Logger.ErrorS("disease", "Invalid disease " + diseaseName + " selected from random diseases.");
				return;
			}
			component.SpawnDisease = disease;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00005FA4 File Offset: 0x000041A4
		private void OnActivate(EntityUid uid, DiseaseArtifactComponent component, ArtifactActivatedEvent args)
		{
			if (component.SpawnDisease == null)
			{
				return;
			}
			TransformComponent xform = base.Transform(uid);
			EntityQuery<DiseaseCarrierComponent> carrierQuery = base.GetEntityQuery<DiseaseCarrierComponent>();
			foreach (EntityUid entity in this._lookup.GetEntitiesInRange(xform.Coordinates, component.Range, 46))
			{
				DiseaseCarrierComponent carrier;
				if (carrierQuery.TryGetComponent(entity, ref carrier) && this._interactionSystem.InRangeUnobstructed(uid, entity, component.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
				{
					this._disease.TryInfect(carrier, component.SpawnDisease, 0.7f, true);
				}
			}
		}

		// Token: 0x04000095 RID: 149
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000096 RID: 150
		[Dependency]
		private readonly DiseaseSystem _disease;

		// Token: 0x04000097 RID: 151
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04000098 RID: 152
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;
	}
}
