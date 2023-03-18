using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Prototypes;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005AD RID: 1453
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public sealed class SpawnEntitiesBehavior : IThresholdBehavior
	{
		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06001E21 RID: 7713 RVA: 0x0009F530 File Offset: 0x0009D730
		// (set) Token: 0x06001E22 RID: 7714 RVA: 0x0009F538 File Offset: 0x0009D738
		[DataField("spawn", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<MinMax, EntityPrototype>))]
		public Dictionary<string, MinMax> Spawn { get; set; } = new Dictionary<string, MinMax>();

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06001E23 RID: 7715 RVA: 0x0009F541 File Offset: 0x0009D741
		// (set) Token: 0x06001E24 RID: 7716 RVA: 0x0009F549 File Offset: 0x0009D749
		[DataField("offset", false, 1, false, false, null)]
		public float Offset { get; set; } = 0.5f;

		// Token: 0x06001E25 RID: 7717 RVA: 0x0009F554 File Offset: 0x0009D754
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			MapCoordinates position = system.EntityManager.GetComponent<TransformComponent>(owner).MapPosition;
			Func<Vector2> getRandomVector = () => new Vector2(system.Random.NextFloat(-this.Offset, this.Offset), system.Random.NextFloat(-this.Offset, this.Offset));
			foreach (KeyValuePair<string, MinMax> keyValuePair in this.Spawn)
			{
				string text;
				MinMax minMax2;
				keyValuePair.Deconstruct(out text, out minMax2);
				string entityId = text;
				MinMax minMax = minMax2;
				int count = (minMax.Min >= minMax.Max) ? minMax.Min : system.Random.Next(minMax.Min, minMax.Max + 1);
				if (count != 0)
				{
					if (EntityPrototypeHelpers.HasComponent<StackComponent>(entityId, system.PrototypeManager, system.ComponentFactory))
					{
						EntityUid spawned = system.EntityManager.SpawnEntity(entityId, position.Offset(getRandomVector()));
						system.StackSystem.SetCount(spawned, count, null);
					}
					else
					{
						for (int i = 0; i < count; i++)
						{
							system.EntityManager.SpawnEntity(entityId, position.Offset(getRandomVector()));
						}
					}
				}
			}
		}
	}
}
