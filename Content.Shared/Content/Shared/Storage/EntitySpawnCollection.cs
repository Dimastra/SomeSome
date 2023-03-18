using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared.Storage
{
	// Token: 0x0200012B RID: 299
	[NullableContext(1)]
	[Nullable(0)]
	public static class EntitySpawnCollection
	{
		// Token: 0x0600036C RID: 876 RVA: 0x0000E7B8 File Offset: 0x0000C9B8
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public static List<string> GetSpawns(IEnumerable<EntitySpawnEntry> entries, [Nullable(2)] IRobustRandom random = null)
		{
			IoCManager.Resolve<IRobustRandom>(ref random);
			List<string> spawned = new List<string>();
			Dictionary<string, EntitySpawnCollection.OrGroup> orGroupedSpawns = new Dictionary<string, EntitySpawnCollection.OrGroup>();
			foreach (EntitySpawnEntry entry in entries)
			{
				if (!string.IsNullOrEmpty(entry.GroupId))
				{
					EntitySpawnCollection.OrGroup orGroup;
					if (!orGroupedSpawns.TryGetValue(entry.GroupId, out orGroup))
					{
						orGroup = new EntitySpawnCollection.OrGroup();
						orGroupedSpawns.Add(entry.GroupId, orGroup);
					}
					orGroup.Entries.Add(entry);
					orGroup.CumulativeProbability += entry.SpawnProbability;
				}
				else if (entry.SpawnProbability == 1f || RandomExtensions.Prob(random, entry.SpawnProbability))
				{
					int amount = entry.Amount;
					if (entry.MaxAmount > amount)
					{
						amount = random.Next(amount, entry.MaxAmount);
					}
					for (int i = 0; i < amount; i++)
					{
						spawned.Add(entry.PrototypeId);
					}
				}
			}
			foreach (EntitySpawnCollection.OrGroup spawnValue in orGroupedSpawns.Values)
			{
				double diceRoll = random.NextDouble() * (double)spawnValue.CumulativeProbability;
				double cumulative = 0.0;
				foreach (EntitySpawnEntry entry2 in spawnValue.Entries)
				{
					cumulative += (double)entry2.SpawnProbability;
					if (diceRoll <= cumulative)
					{
						int amount2 = entry2.Amount;
						if (entry2.MaxAmount > amount2)
						{
							amount2 = random.Next(amount2, entry2.MaxAmount);
						}
						for (int index = 0; index < amount2; index++)
						{
							spawned.Add(entry2.PrototypeId);
						}
						break;
					}
				}
			}
			return spawned;
		}

		// Token: 0x02000799 RID: 1945
		[Nullable(0)]
		private sealed class OrGroup
		{
			// Token: 0x170004EF RID: 1263
			// (get) Token: 0x060017D5 RID: 6101 RVA: 0x0004D1E5 File Offset: 0x0004B3E5
			// (set) Token: 0x060017D6 RID: 6102 RVA: 0x0004D1ED File Offset: 0x0004B3ED
			public List<EntitySpawnEntry> Entries { get; set; } = new List<EntitySpawnEntry>();

			// Token: 0x170004F0 RID: 1264
			// (get) Token: 0x060017D7 RID: 6103 RVA: 0x0004D1F6 File Offset: 0x0004B3F6
			// (set) Token: 0x060017D8 RID: 6104 RVA: 0x0004D1FE File Offset: 0x0004B3FE
			public float CumulativeProbability { get; set; }
		}
	}
}
