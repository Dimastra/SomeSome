using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.AlertLevel
{
	// Token: 0x020007DD RID: 2013
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class AlertLevelDetail
	{
		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x06002BBC RID: 11196 RVA: 0x000E59A6 File Offset: 0x000E3BA6
		[DataField("announcement", false, 1, false, false, null)]
		public string Announcement { get; } = string.Empty;

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x06002BBD RID: 11197 RVA: 0x000E59AE File Offset: 0x000E3BAE
		[DataField("selectable", false, 1, false, false, null)]
		public bool Selectable { get; } = 1;

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x06002BBE RID: 11198 RVA: 0x000E59B6 File Offset: 0x000E3BB6
		[DataField("disableSelection", false, 1, false, false, null)]
		public bool DisableSelection { get; }

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x06002BBF RID: 11199 RVA: 0x000E59BE File Offset: 0x000E3BBE
		[Nullable(2)]
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound { [NullableContext(2)] get; }

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x06002BC0 RID: 11200 RVA: 0x000E59C6 File Offset: 0x000E3BC6
		[DataField("color", false, 1, false, false, null)]
		public Color Color { get; } = Color.White;

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x06002BC1 RID: 11201 RVA: 0x000E59CE File Offset: 0x000E3BCE
		[DataField("emergencyLightColor", false, 1, false, false, null)]
		public Color EmergencyLightColor { get; } = Color.FromHex("#FF4020", null);

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x06002BC2 RID: 11202 RVA: 0x000E59D6 File Offset: 0x000E3BD6
		[DataField("forceEnableEmergencyLights", false, 1, false, false, null)]
		public bool ForceEnableEmergencyLights { get; }

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x06002BC3 RID: 11203 RVA: 0x000E59DE File Offset: 0x000E3BDE
		[DataField("shuttleTime", false, 1, false, false, null)]
		public TimeSpan ShuttleTime { get; } = TimeSpan.FromMinutes(5.0);
	}
}
