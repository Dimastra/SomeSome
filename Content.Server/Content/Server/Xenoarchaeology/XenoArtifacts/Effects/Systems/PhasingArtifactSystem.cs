using System;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x02000048 RID: 72
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PhasingArtifactSystem : EntitySystem
	{
		// Token: 0x060000E0 RID: 224 RVA: 0x0000647C File Offset: 0x0000467C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PhasingArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<PhasingArtifactComponent, ArtifactActivatedEvent>(this.OnActivate), null, null);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00006498 File Offset: 0x00004698
		private void OnActivate(EntityUid uid, PhasingArtifactComponent component, ArtifactActivatedEvent args)
		{
			FixturesComponent fixtures;
			if (!base.TryComp<FixturesComponent>(uid, ref fixtures))
			{
				return;
			}
			foreach (Fixture fixture in fixtures.Fixtures.Values)
			{
				this._physics.SetHard(uid, fixture, false, fixtures);
			}
		}

		// Token: 0x040000A1 RID: 161
		[Dependency]
		private readonly SharedPhysicsSystem _physics;
	}
}
