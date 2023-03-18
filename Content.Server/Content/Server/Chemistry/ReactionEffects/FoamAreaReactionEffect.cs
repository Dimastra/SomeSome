using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components;
using Content.Server.Coordinates.Helpers;
using Content.Shared.Chemistry.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReactionEffects
{
	// Token: 0x0200068E RID: 1678
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class FoamAreaReactionEffect : AreaReactionEffect
	{
		// Token: 0x060022D0 RID: 8912 RVA: 0x000B5277 File Offset: 0x000B3477
		protected override SolutionAreaEffectComponent GetAreaEffectComponent(EntityUid entity)
		{
			return EntityManagerExt.GetComponentOrNull<FoamSolutionAreaEffectComponent>(IoCManager.Resolve<IEntityManager>(), entity);
		}

		// Token: 0x060022D1 RID: 8913 RVA: 0x000B5284 File Offset: 0x000B3484
		public static void SpawnFoam([Nullable(1)] string entityPrototype, EntityCoordinates coords, Solution contents, int amount, float duration, float spreadDelay, float removeDelay, SoundSpecifier sound = null, IEntityManager entityManager = null)
		{
			if (entityManager == null)
			{
				entityManager = IoCManager.Resolve<IEntityManager>();
			}
			EntityUid ent = entityManager.SpawnEntity(entityPrototype, coords.SnapToGrid(null, null));
			FoamSolutionAreaEffectComponent areaEffectComponent = EntityManagerExt.GetComponentOrNull<FoamSolutionAreaEffectComponent>(entityManager, ent);
			if (areaEffectComponent == null)
			{
				Logger.Error("Couldn't get AreaEffectComponent from " + entityPrototype);
				IoCManager.Resolve<IEntityManager>().QueueDeleteEntity(ent);
				return;
			}
			if (contents != null)
			{
				areaEffectComponent.TryAddSolution(contents);
			}
			areaEffectComponent.Start(amount, duration, spreadDelay, removeDelay);
			entityManager.EntitySysManager.GetEntitySystem<AudioSystem>().PlayPvs(sound, ent, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.125f))));
		}
	}
}
