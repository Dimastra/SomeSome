using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Construction.Steps
{
	// Token: 0x02000574 RID: 1396
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class MaterialConstructionGraphStep : EntityInsertConstructionGraphStep
	{
		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06001110 RID: 4368 RVA: 0x00038384 File Offset: 0x00036584
		[DataField("material", false, 1, true, false, typeof(PrototypeIdSerializer<StackPrototype>))]
		public string MaterialPrototypeId { get; } = "Steel";

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06001111 RID: 4369 RVA: 0x0003838C File Offset: 0x0003658C
		[DataField("amount", false, 1, false, false, null)]
		public int Amount { get; } = 1;

		// Token: 0x06001112 RID: 4370 RVA: 0x00038394 File Offset: 0x00036594
		public override void DoExamine(ExaminedEvent examinedEvent)
		{
			StackPrototype material = IoCManager.Resolve<IPrototypeManager>().Index<StackPrototype>(this.MaterialPrototypeId);
			examinedEvent.Message.AddMarkup(Loc.GetString("construction-insert-material-entity", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("amount", this.Amount),
				new ValueTuple<string, object>("materialName", material.Name)
			}));
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x00038400 File Offset: 0x00036600
		public override bool EntityValid(EntityUid uid, IEntityManager entityManager, IComponentFactory compFactory)
		{
			StackComponent stack;
			return entityManager.TryGetComponent<StackComponent>(uid, ref stack) && stack.StackTypeId == this.MaterialPrototypeId && stack.Count >= this.Amount;
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x00038440 File Offset: 0x00036640
		[NullableContext(2)]
		public bool EntityValid(EntityUid entity, [NotNullWhen(true)] out StackComponent stack)
		{
			StackComponent otherStack;
			if (IoCManager.Resolve<IEntityManager>().TryGetComponent<StackComponent>(entity, ref otherStack) && otherStack.StackTypeId == this.MaterialPrototypeId && otherStack.Count >= this.Amount)
			{
				stack = otherStack;
			}
			else
			{
				stack = null;
			}
			return stack != null;
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x0003848C File Offset: 0x0003668C
		public override ConstructionGuideEntry GenerateGuideEntry()
		{
			StackPrototype material = IoCManager.Resolve<IPrototypeManager>().Index<StackPrototype>(this.MaterialPrototypeId);
			return new ConstructionGuideEntry
			{
				Localization = "construction-presenter-material-step",
				Arguments = new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("amount", this.Amount),
					new ValueTuple<string, object>("material", material.Name)
				},
				Icon = material.Icon
			};
		}
	}
}
