using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weather
{
	// Token: 0x02000041 RID: 65
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("weather", 1)]
	public sealed class WeatherPrototype : IPrototype
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00003187 File Offset: 0x00001387
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x040000B2 RID: 178
		[ViewVariables]
		public TimeSpan DurationMinimum = TimeSpan.FromSeconds(120.0);

		// Token: 0x040000B3 RID: 179
		[ViewVariables]
		public TimeSpan DurationMaximum = TimeSpan.FromSeconds(300.0);

		// Token: 0x040000B4 RID: 180
		[ViewVariables]
		[DataField("startupTime", false, 1, false, false, null)]
		public TimeSpan StartupTime = TimeSpan.FromSeconds(30.0);

		// Token: 0x040000B5 RID: 181
		[ViewVariables]
		[DataField("endTime", false, 1, false, false, null)]
		public TimeSpan ShutdownTime = TimeSpan.FromSeconds(30.0);

		// Token: 0x040000B6 RID: 182
		[ViewVariables]
		[DataField("sprite", false, 1, true, false, null)]
		public SpriteSpecifier Sprite;

		// Token: 0x040000B7 RID: 183
		[ViewVariables]
		[DataField("color", false, 1, false, false, null)]
		public Color? Color;

		// Token: 0x040000B8 RID: 184
		[Nullable(2)]
		[ViewVariables]
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound;
	}
}
