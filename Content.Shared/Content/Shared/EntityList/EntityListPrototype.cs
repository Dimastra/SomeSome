using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.EntityList
{
	// Token: 0x020004B7 RID: 1207
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("entityList", 1)]
	public sealed class EntityListPrototype : IPrototype
	{
		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06000E9C RID: 3740 RVA: 0x0002F230 File Offset: 0x0002D430
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06000E9D RID: 3741 RVA: 0x0002F238 File Offset: 0x0002D438
		[DataField("entities", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		public ImmutableList<string> EntityIds { get; } = ImmutableList<string>.Empty;

		// Token: 0x06000E9E RID: 3742 RVA: 0x0002F240 File Offset: 0x0002D440
		public IEnumerable<EntityPrototype> Entities([Nullable(2)] IPrototypeManager prototypeManager = null)
		{
			if (prototypeManager == null)
			{
				prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			}
			foreach (string entityId in this.EntityIds)
			{
				yield return prototypeManager.Index<EntityPrototype>(entityId);
			}
			ImmutableList<string>.Enumerator enumerator = default(ImmutableList<string>.Enumerator);
			yield break;
			yield break;
		}
	}
}
