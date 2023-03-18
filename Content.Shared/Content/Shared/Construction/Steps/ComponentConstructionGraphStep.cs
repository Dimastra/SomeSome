using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Steps
{
	// Token: 0x02000570 RID: 1392
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ComponentConstructionGraphStep : ArbitraryInsertConstructionGraphStep
	{
		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06001100 RID: 4352 RVA: 0x0003811D File Offset: 0x0003631D
		[DataField("component", false, 1, false, false, null)]
		public string Component { get; } = string.Empty;

		// Token: 0x06001101 RID: 4353 RVA: 0x00038128 File Offset: 0x00036328
		public override bool EntityValid(EntityUid uid, IEntityManager entityManager, IComponentFactory compFactory)
		{
			foreach (IComponent component in entityManager.GetComponents(uid))
			{
				if (compFactory.GetComponentName(component.GetType()) == this.Component)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x00038190 File Offset: 0x00036390
		public override void DoExamine(ExaminedEvent examinedEvent)
		{
			examinedEvent.Message.AddMarkup(string.IsNullOrEmpty(base.Name) ? Loc.GetString("construction-insert-entity-with-component", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("componentName", this.Component)
			}) : Loc.GetString("construction-insert-exact-entity", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("entityName", base.Name)
			}));
		}
	}
}
