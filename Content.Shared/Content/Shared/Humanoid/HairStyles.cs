using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;

namespace Content.Shared.Humanoid
{
	// Token: 0x02000403 RID: 1027
	[NullableContext(1)]
	[Nullable(0)]
	public static class HairStyles
	{
		// Token: 0x04000BEB RID: 3051
		public const string DefaultHairStyle = "HairBald";

		// Token: 0x04000BEC RID: 3052
		public const string DefaultFacialHairStyle = "FacialHairShaved";

		// Token: 0x04000BED RID: 3053
		public static readonly IReadOnlyList<Color> RealisticHairColors = new List<Color>
		{
			Color.Yellow,
			Color.Black,
			Color.SandyBrown,
			Color.Brown,
			Color.Wheat,
			Color.Gray
		};
	}
}
