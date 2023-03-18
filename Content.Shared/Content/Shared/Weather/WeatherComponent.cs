using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weather
{
	// Token: 0x0200003F RID: 63
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class WeatherComponent : Component
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00003156 File Offset: 0x00001356
		[ViewVariables]
		public TimeSpan Duration
		{
			get
			{
				return this.EndTime - this.StartTime;
			}
		}

		// Token: 0x040000A7 RID: 167
		[ViewVariables]
		[DataField("weather", false, 1, false, false, null)]
		public string Weather;

		// Token: 0x040000A8 RID: 168
		public IPlayingAudioStream Stream;

		// Token: 0x040000A9 RID: 169
		[ViewVariables]
		[DataField("startTime", false, 1, false, false, null)]
		public TimeSpan StartTime = TimeSpan.Zero;

		// Token: 0x040000AA RID: 170
		[ViewVariables]
		[DataField("endTime", false, 1, false, false, null)]
		public TimeSpan EndTime = TimeSpan.Zero;

		// Token: 0x040000AB RID: 171
		[ViewVariables]
		public WeatherState State;
	}
}
