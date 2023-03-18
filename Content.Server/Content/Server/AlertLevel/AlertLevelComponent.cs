using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.AlertLevel
{
	// Token: 0x020007DA RID: 2010
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AlertLevelComponent : Component
	{
		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x06002BB3 RID: 11187 RVA: 0x000E582C File Offset: 0x000E3A2C
		[ViewVariables]
		public bool IsSelectable
		{
			get
			{
				AlertLevelDetail level;
				return this.AlertLevels != null && this.AlertLevels.Levels.TryGetValue(this.CurrentLevel, out level) && (level.Selectable && !level.DisableSelection) && !this.IsLevelLocked;
			}
		}

		// Token: 0x04001B1E RID: 6942
		[Nullable(2)]
		[ViewVariables]
		public AlertLevelPrototype AlertLevels;

		// Token: 0x04001B1F RID: 6943
		[DataField("alertLevelPrototype", false, 1, false, false, null)]
		public string AlertLevelPrototype;

		// Token: 0x04001B20 RID: 6944
		[ViewVariables]
		public string CurrentLevel = string.Empty;

		// Token: 0x04001B21 RID: 6945
		[ViewVariables]
		public bool IsLevelLocked;

		// Token: 0x04001B22 RID: 6946
		[ViewVariables]
		public const float Delay = 30f;

		// Token: 0x04001B23 RID: 6947
		[ViewVariables]
		public float CurrentDelay;

		// Token: 0x04001B24 RID: 6948
		[ViewVariables]
		public bool ActiveDelay;
	}
}
