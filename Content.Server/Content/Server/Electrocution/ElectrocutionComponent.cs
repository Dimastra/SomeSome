using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Electrocution
{
	// Token: 0x02000532 RID: 1330
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ElectrocutionSystem)
	})]
	public sealed class ElectrocutionComponent : Component
	{
		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06001BC0 RID: 7104 RVA: 0x00093D9E File Offset: 0x00091F9E
		// (set) Token: 0x06001BC1 RID: 7105 RVA: 0x00093DA6 File Offset: 0x00091FA6
		[DataField("timeLeft", false, 1, false, false, null)]
		public float TimeLeft { get; set; }

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06001BC2 RID: 7106 RVA: 0x00093DAF File Offset: 0x00091FAF
		// (set) Token: 0x06001BC3 RID: 7107 RVA: 0x00093DB7 File Offset: 0x00091FB7
		[DataField("electrocuting", false, 1, false, false, null)]
		public EntityUid Electrocuting { get; set; }

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06001BC4 RID: 7108 RVA: 0x00093DC0 File Offset: 0x00091FC0
		// (set) Token: 0x06001BC5 RID: 7109 RVA: 0x00093DC8 File Offset: 0x00091FC8
		[DataField("accumDamage", false, 1, false, false, null)]
		public float AccumulatedDamage { get; set; }

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06001BC6 RID: 7110 RVA: 0x00093DD1 File Offset: 0x00091FD1
		// (set) Token: 0x06001BC7 RID: 7111 RVA: 0x00093DD9 File Offset: 0x00091FD9
		[DataField("source", false, 1, false, false, null)]
		public EntityUid Source { get; set; }
	}
}
