using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Pinpointer
{
	// Token: 0x02000273 RID: 627
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedPinpointerSystem)
	})]
	public sealed class PinpointerComponent : Component
	{
		// Token: 0x17000169 RID: 361
		// (get) Token: 0x0600072F RID: 1839 RVA: 0x000188DD File Offset: 0x00016ADD
		public bool HasTarget
		{
			get
			{
				return this.DistanceToTarget > Distance.Unknown;
			}
		}

		// Token: 0x0400070B RID: 1803
		[Nullable(2)]
		[DataField("component", false, 1, false, false, null)]
		public string Component;

		// Token: 0x0400070C RID: 1804
		[DataField("mediumDistance", false, 1, false, false, null)]
		public float MediumDistance = 16f;

		// Token: 0x0400070D RID: 1805
		[DataField("closeDistance", false, 1, false, false, null)]
		public float CloseDistance = 8f;

		// Token: 0x0400070E RID: 1806
		[DataField("reachedDistance", false, 1, false, false, null)]
		public float ReachedDistance = 1f;

		// Token: 0x0400070F RID: 1807
		[DataField("precision", false, 1, false, false, null)]
		public double Precision = 0.09;

		// Token: 0x04000710 RID: 1808
		public EntityUid? Target;

		// Token: 0x04000711 RID: 1809
		public bool IsActive;

		// Token: 0x04000712 RID: 1810
		public Angle ArrowAngle;

		// Token: 0x04000713 RID: 1811
		public Distance DistanceToTarget;
	}
}
