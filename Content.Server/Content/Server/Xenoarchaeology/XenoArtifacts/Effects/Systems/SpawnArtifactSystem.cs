using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x0200004C RID: 76
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpawnArtifactSystem : EntitySystem
	{
		// Token: 0x060000EC RID: 236 RVA: 0x00006708 File Offset: 0x00004908
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpawnArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<SpawnArtifactComponent, ArtifactActivatedEvent>(this.OnActivate), null, null);
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00006724 File Offset: 0x00004924
		private void OnActivate(EntityUid uid, SpawnArtifactComponent component, ArtifactActivatedEvent args)
		{
			int amount;
			if (!this._artifact.TryGetNodeData<int>(uid, "nodeDataSpawnAmount", out amount, null))
			{
				amount = 0;
			}
			if (amount >= component.MaxSpawns)
			{
				return;
			}
			List<EntitySpawnEntry> spawns = component.Spawns;
			if (spawns == null)
			{
				return;
			}
			MapCoordinates artifactCord = base.Transform(uid).MapPosition;
			foreach (string spawn in EntitySpawnCollection.GetSpawns(spawns, this._random))
			{
				float dx = this._random.NextFloat(-component.Range, component.Range);
				float dy = this._random.NextFloat(-component.Range, component.Range);
				MapCoordinates spawnCord = artifactCord.Offset(new Vector2(dx, dy));
				this.EntityManager.SpawnEntity(spawn, spawnCord);
			}
			this._artifact.SetNodeData(uid, "nodeDataSpawnAmount", amount + 1, null);
		}

		// Token: 0x040000AA RID: 170
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040000AB RID: 171
		[Dependency]
		private readonly ArtifactSystem _artifact;

		// Token: 0x040000AC RID: 172
		public const string NodeDataSpawnAmount = "nodeDataSpawnAmount";
	}
}
