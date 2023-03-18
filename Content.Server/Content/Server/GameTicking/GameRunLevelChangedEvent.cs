using System;

namespace Content.Server.GameTicking
{
	// Token: 0x020004AB RID: 1195
	public sealed class GameRunLevelChangedEvent
	{
		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06001864 RID: 6244 RVA: 0x0007F746 File Offset: 0x0007D946
		public GameRunLevel Old { get; }

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06001865 RID: 6245 RVA: 0x0007F74E File Offset: 0x0007D94E
		public GameRunLevel New { get; }

		// Token: 0x06001866 RID: 6246 RVA: 0x0007F756 File Offset: 0x0007D956
		public GameRunLevelChangedEvent(GameRunLevel old, GameRunLevel @new)
		{
			this.Old = old;
			this.New = @new;
		}
	}
}
