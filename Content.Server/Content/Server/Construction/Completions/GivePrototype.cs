using System;
using System.Runtime.CompilerServices;
using Content.Server.Stack;
using Content.Shared.Construction;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Prototypes;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Construction.Completions
{
	// Token: 0x0200061A RID: 1562
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class GivePrototype : IGraphAction
	{
		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06002154 RID: 8532 RVA: 0x000AE6EB File Offset: 0x000AC8EB
		// (set) Token: 0x06002155 RID: 8533 RVA: 0x000AE6F3 File Offset: 0x000AC8F3
		[DataField("prototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype { get; private set; } = string.Empty;

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06002156 RID: 8534 RVA: 0x000AE6FC File Offset: 0x000AC8FC
		// (set) Token: 0x06002157 RID: 8535 RVA: 0x000AE704 File Offset: 0x000AC904
		[DataField("amount", false, 1, false, false, null)]
		public int Amount { get; private set; } = 1;

		// Token: 0x06002158 RID: 8536 RVA: 0x000AE710 File Offset: 0x000AC910
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (string.IsNullOrEmpty(this.Prototype))
			{
				return;
			}
			EntityCoordinates coordinates = entityManager.GetComponent<TransformComponent>(userUid ?? uid).Coordinates;
			if (EntityPrototypeHelpers.HasComponent<StackComponent>(this.Prototype, null, null))
			{
				EntityUid stackEnt = entityManager.SpawnEntity(this.Prototype, coordinates);
				StackComponent stack = entityManager.GetComponent<StackComponent>(stackEnt);
				entityManager.EntitySysManager.GetEntitySystem<StackSystem>().SetCount(stackEnt, this.Amount, stack);
				entityManager.EntitySysManager.GetEntitySystem<SharedHandsSystem>().PickupOrDrop(userUid, stackEnt, true, false, null, null);
				return;
			}
			for (int i = 0; i < this.Amount; i++)
			{
				EntityUid item = entityManager.SpawnEntity(this.Prototype, coordinates);
				entityManager.EntitySysManager.GetEntitySystem<SharedHandsSystem>().PickupOrDrop(userUid, item, true, false, null, null);
			}
		}
	}
}
