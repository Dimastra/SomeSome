using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Explosion.Components
{
	// Token: 0x02000523 RID: 1315
	[RegisterComponent]
	public sealed class TriggerOnVoiceComponent : Component
	{
		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06001B51 RID: 6993 RVA: 0x0009271E File Offset: 0x0009091E
		public bool IsListening
		{
			get
			{
				return this.IsRecording || !string.IsNullOrWhiteSpace(this.KeyPhrase);
			}
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06001B52 RID: 6994 RVA: 0x00092738 File Offset: 0x00090938
		// (set) Token: 0x06001B53 RID: 6995 RVA: 0x00092740 File Offset: 0x00090940
		[ViewVariables]
		[DataField("listenRange", false, 1, false, false, null)]
		public int ListenRange { get; private set; } = 4;

		// Token: 0x0400118F RID: 4495
		[Nullable(2)]
		[ViewVariables]
		[DataField("keyPhrase", false, 1, false, false, null)]
		public string KeyPhrase;

		// Token: 0x04001191 RID: 4497
		[DataField("isRecording", false, 1, false, false, null)]
		public bool IsRecording;

		// Token: 0x04001192 RID: 4498
		[DataField("minLength", false, 1, false, false, null)]
		public int MinLength = 3;

		// Token: 0x04001193 RID: 4499
		[DataField("maxLength", false, 1, false, false, null)]
		public int MaxLength = 50;
	}
}
