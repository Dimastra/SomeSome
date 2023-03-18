using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Mind.Components
{
	// Token: 0x020003A8 RID: 936
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class VisitingMindComponent : Component
	{
		// Token: 0x170002BF RID: 703
		// (get) Token: 0x0600132D RID: 4909 RVA: 0x00062F54 File Offset: 0x00061154
		// (set) Token: 0x0600132E RID: 4910 RVA: 0x00062F5C File Offset: 0x0006115C
		[ViewVariables]
		public Mind Mind { get; set; }

		// Token: 0x0600132F RID: 4911 RVA: 0x00062F65 File Offset: 0x00061165
		protected override void OnRemove()
		{
			base.OnRemove();
			Mind mind = this.Mind;
			if (mind == null)
			{
				return;
			}
			mind.UnVisit();
		}
	}
}
