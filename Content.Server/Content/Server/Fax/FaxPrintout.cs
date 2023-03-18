using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Fax
{
	// Token: 0x02000501 RID: 1281
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class FaxPrintout
	{
		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06001A5E RID: 6750 RVA: 0x0008ACBA File Offset: 0x00088EBA
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; }

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06001A5F RID: 6751 RVA: 0x0008ACC2 File Offset: 0x00088EC2
		[DataField("content", false, 1, false, false, null)]
		public string Content { get; }

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06001A60 RID: 6752 RVA: 0x0008ACCA File Offset: 0x00088ECA
		[DataField("prototypeId", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string PrototypeId { get; }

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06001A61 RID: 6753 RVA: 0x0008ACD2 File Offset: 0x00088ED2
		[Nullable(2)]
		[DataField("stampState", false, 1, false, false, null)]
		public string StampState { [NullableContext(2)] get; }

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06001A62 RID: 6754 RVA: 0x0008ACDA File Offset: 0x00088EDA
		[DataField("stampedBy", false, 1, false, false, null)]
		public List<string> StampedBy { get; }

		// Token: 0x06001A63 RID: 6755 RVA: 0x0008ACE2 File Offset: 0x00088EE2
		public FaxPrintout(string content, string name, [Nullable(2)] string prototypeId, [Nullable(2)] string stampState = null, [Nullable(new byte[]
		{
			2,
			1
		})] List<string> stampedBy = null)
		{
			this.Content = content;
			this.Name = name;
			this.PrototypeId = (prototypeId ?? "");
			this.StampState = stampState;
			this.StampedBy = (stampedBy ?? new List<string>());
		}
	}
}
