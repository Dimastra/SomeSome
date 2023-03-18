using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003DA RID: 986
	public sealed class SuicideEvent : EntityEventArgs
	{
		// Token: 0x06000B91 RID: 2961 RVA: 0x0002635F File Offset: 0x0002455F
		public SuicideEvent(EntityUid victim)
		{
			this.Victim = victim;
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x0002636E File Offset: 0x0002456E
		public void SetHandled(SuicideKind kind)
		{
			if (this.Handled)
			{
				throw new InvalidOperationException("Suicide was already handled");
			}
			this.Kind = new SuicideKind?(kind);
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x0002638F File Offset: 0x0002458F
		public void BlockSuicideAttempt(bool suicideAttempt)
		{
			if (suicideAttempt)
			{
				this.AttemptBlocked = suicideAttempt;
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06000B94 RID: 2964 RVA: 0x0002639B File Offset: 0x0002459B
		// (set) Token: 0x06000B95 RID: 2965 RVA: 0x000263A3 File Offset: 0x000245A3
		public SuicideKind? Kind { get; private set; }

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000B96 RID: 2966 RVA: 0x000263AC File Offset: 0x000245AC
		// (set) Token: 0x06000B97 RID: 2967 RVA: 0x000263B4 File Offset: 0x000245B4
		public EntityUid Victim { get; private set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000B98 RID: 2968 RVA: 0x000263BD File Offset: 0x000245BD
		// (set) Token: 0x06000B99 RID: 2969 RVA: 0x000263C5 File Offset: 0x000245C5
		public bool AttemptBlocked { get; private set; }

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000B9A RID: 2970 RVA: 0x000263D0 File Offset: 0x000245D0
		public bool Handled
		{
			get
			{
				return this.Kind != null;
			}
		}
	}
}
