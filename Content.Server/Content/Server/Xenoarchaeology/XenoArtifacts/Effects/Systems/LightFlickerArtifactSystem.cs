using System;
using System.Runtime.CompilerServices;
using Content.Server.Ghost;
using Content.Server.Light.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x02000047 RID: 71
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LightFlickerArtifactSystem : EntitySystem
	{
		// Token: 0x060000DD RID: 221 RVA: 0x000063D1 File Offset: 0x000045D1
		public override void Initialize()
		{
			base.SubscribeLocalEvent<LightFlickerArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<LightFlickerArtifactComponent, ArtifactActivatedEvent>(this.OnActivated), null, null);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000063E8 File Offset: 0x000045E8
		private void OnActivated(EntityUid uid, LightFlickerArtifactComponent component, ArtifactActivatedEvent args)
		{
			EntityQuery<PoweredLightComponent> lights = base.GetEntityQuery<PoweredLightComponent>();
			foreach (EntityUid light in this._lookup.GetEntitiesInRange(uid, component.Radius, 12))
			{
				if (lights.HasComponent(light) && RandomExtensions.Prob(this._random, component.FlickerChance))
				{
					this._ghost.DoGhostBooEvent(light);
				}
			}
		}

		// Token: 0x0400009E RID: 158
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400009F RID: 159
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040000A0 RID: 160
		[Dependency]
		private readonly GhostSystem _ghost;
	}
}
