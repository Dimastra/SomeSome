using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Buckle.Components
{
	// Token: 0x02000642 RID: 1602
	[NetSerializable]
	[Serializable]
	public sealed class BuckleComponentState : ComponentState
	{
		// Token: 0x06001362 RID: 4962 RVA: 0x00040869 File Offset: 0x0003EA69
		public BuckleComponentState(bool buckled, EntityUid? lastEntityBuckledTo, bool dontCollide)
		{
			this.Buckled = buckled;
			this.LastEntityBuckledTo = lastEntityBuckledTo;
			this.DontCollide = dontCollide;
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06001363 RID: 4963 RVA: 0x00040886 File Offset: 0x0003EA86
		public bool Buckled { get; }

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06001364 RID: 4964 RVA: 0x0004088E File Offset: 0x0003EA8E
		public EntityUid? LastEntityBuckledTo { get; }

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06001365 RID: 4965 RVA: 0x00040896 File Offset: 0x0003EA96
		public bool DontCollide { get; }
	}
}
