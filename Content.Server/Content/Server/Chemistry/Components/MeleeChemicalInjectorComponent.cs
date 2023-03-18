using System;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006A8 RID: 1704
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class MeleeChemicalInjectorComponent : Component
	{
		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06002388 RID: 9096 RVA: 0x000B9AA5 File Offset: 0x000B7CA5
		// (set) Token: 0x06002389 RID: 9097 RVA: 0x000B9AAD File Offset: 0x000B7CAD
		[ViewVariables]
		[DataField("transferAmount", false, 1, false, false, null)]
		public FixedPoint2 TransferAmount { get; set; } = FixedPoint2.New(1);

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x0600238A RID: 9098 RVA: 0x000B9AB6 File Offset: 0x000B7CB6
		// (set) Token: 0x0600238B RID: 9099 RVA: 0x000B9ABE File Offset: 0x000B7CBE
		[ViewVariables]
		public float TransferEfficiency
		{
			get
			{
				return this._transferEfficiency;
			}
			set
			{
				this._transferEfficiency = Math.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x0600238C RID: 9100 RVA: 0x000B9AD6 File Offset: 0x000B7CD6
		// (set) Token: 0x0600238D RID: 9101 RVA: 0x000B9ADE File Offset: 0x000B7CDE
		[ViewVariables]
		[DataField("solution", false, 1, false, false, null)]
		public string Solution { get; set; } = "default";

		// Token: 0x040015EB RID: 5611
		[DataField("transferEfficiency", false, 1, false, false, null)]
		private float _transferEfficiency = 1f;
	}
}
