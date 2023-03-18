using System;
using System.Runtime.CompilerServices;
using Content.Server.Stack;
using Content.Shared.Construction;
using Content.Shared.Prototypes;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000624 RID: 1572
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class SpawnPrototype : IGraphAction
	{
		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x0600217B RID: 8571 RVA: 0x000AEB83 File Offset: 0x000ACD83
		// (set) Token: 0x0600217C RID: 8572 RVA: 0x000AEB8B File Offset: 0x000ACD8B
		[DataField("prototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype { get; private set; } = string.Empty;

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x0600217D RID: 8573 RVA: 0x000AEB94 File Offset: 0x000ACD94
		// (set) Token: 0x0600217E RID: 8574 RVA: 0x000AEB9C File Offset: 0x000ACD9C
		[DataField("amount", false, 1, false, false, null)]
		public int Amount { get; private set; } = 1;

		// Token: 0x0600217F RID: 8575 RVA: 0x000AEBA8 File Offset: 0x000ACDA8
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (string.IsNullOrEmpty(this.Prototype))
			{
				return;
			}
			EntityCoordinates coordinates = entityManager.GetComponent<TransformComponent>(uid).Coordinates;
			if (EntityPrototypeHelpers.HasComponent<StackComponent>(this.Prototype, null, null))
			{
				EntityUid stackEnt = entityManager.SpawnEntity(this.Prototype, coordinates);
				StackComponent stack = entityManager.GetComponent<StackComponent>(stackEnt);
				entityManager.EntitySysManager.GetEntitySystem<StackSystem>().SetCount(stackEnt, this.Amount, stack);
				return;
			}
			for (int i = 0; i < this.Amount; i++)
			{
				entityManager.SpawnEntity(this.Prototype, coordinates);
			}
		}
	}
}
