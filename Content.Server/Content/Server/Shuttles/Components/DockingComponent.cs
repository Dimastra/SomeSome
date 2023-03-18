using System;
using System.Runtime.CompilerServices;
using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Shuttles.Components
{
	// Token: 0x02000202 RID: 514
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DockingComponent : SharedDockingComponent
	{
		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000A5F RID: 2655 RVA: 0x00036F1C File Offset: 0x0003511C
		[ViewVariables]
		public override bool Docked
		{
			get
			{
				return this.DockedWith != null;
			}
		}

		// Token: 0x0400063B RID: 1595
		[DataField("dockedWith", false, 1, false, false, null)]
		public EntityUid? DockedWith;

		// Token: 0x0400063C RID: 1596
		[ViewVariables]
		public Joint DockJoint;

		// Token: 0x0400063D RID: 1597
		[DataField("dockJointId", false, 1, false, false, null)]
		public string DockJointId;

		// Token: 0x0400063E RID: 1598
		[ViewVariables]
		[DataField("radarColor", false, 1, false, false, null)]
		public Color RadarColor = Color.DarkViolet;

		// Token: 0x0400063F RID: 1599
		[ViewVariables]
		[DataField("highlightedRadarColor", false, 1, false, false, null)]
		public Color HighlightedRadarColor = Color.Magenta;

		// Token: 0x04000640 RID: 1600
		[ViewVariables]
		public int PathfindHandle = -1;
	}
}
