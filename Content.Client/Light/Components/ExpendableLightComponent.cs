using System;
using System.Runtime.CompilerServices;
using Content.Shared.Light.Component;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;

namespace Content.Client.Light.Components
{
	// Token: 0x0200026D RID: 621
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ExpendableLightComponent : SharedExpendableLightComponent
	{
		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06000FC6 RID: 4038 RVA: 0x0005F065 File Offset: 0x0005D265
		// (set) Token: 0x06000FC7 RID: 4039 RVA: 0x0005F06D File Offset: 0x0005D26D
		public IPlayingAudioStream PlayingStream { get; set; }
	}
}
