using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Nutrition
{
	// Token: 0x02000306 RID: 774
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("flavor", 1)]
	public sealed class FlavorPrototype : IPrototype
	{
		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x000513B2 File Offset: 0x0004F5B2
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000FF5 RID: 4085 RVA: 0x000513BA File Offset: 0x0004F5BA
		[DataField("flavorType", false, 1, false, false, null)]
		public FlavorType FlavorType { get; }

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06000FF6 RID: 4086 RVA: 0x000513C2 File Offset: 0x0004F5C2
		[DataField("description", false, 1, false, false, null)]
		public string FlavorDescription { get; }
	}
}
