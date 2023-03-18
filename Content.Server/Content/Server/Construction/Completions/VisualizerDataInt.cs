using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000628 RID: 1576
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class VisualizerDataInt : IGraphAction
	{
		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x06002192 RID: 8594 RVA: 0x000AEE02 File Offset: 0x000AD002
		// (set) Token: 0x06002193 RID: 8595 RVA: 0x000AEE0A File Offset: 0x000AD00A
		[DataField("key", false, 1, false, false, null)]
		public string Key { get; private set; } = string.Empty;

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06002194 RID: 8596 RVA: 0x000AEE13 File Offset: 0x000AD013
		// (set) Token: 0x06002195 RID: 8597 RVA: 0x000AEE1B File Offset: 0x000AD01B
		[DataField("data", false, 1, false, false, null)]
		public int Data { get; private set; }

		// Token: 0x06002196 RID: 8598 RVA: 0x000AEE24 File Offset: 0x000AD024
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (string.IsNullOrEmpty(this.Key))
			{
				return;
			}
			AppearanceComponent appearance;
			Enum @enum;
			if (entityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance) && IoCManager.Resolve<IReflectionManager>().TryParseEnumReference(this.Key, ref @enum, true))
			{
				entityManager.System<AppearanceSystem>().SetData(uid, @enum, this.Data, appearance);
			}
		}
	}
}
