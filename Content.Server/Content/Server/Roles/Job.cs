using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Server.Mind;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Server.Roles
{
	// Token: 0x02000229 RID: 553
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class Job : Role, IRoleTimer
	{
		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000B0A RID: 2826 RVA: 0x0003A057 File Offset: 0x00038257
		[ViewVariables]
		public string Timer
		{
			get
			{
				return this.Prototype.PlayTimeTracker;
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000B0B RID: 2827 RVA: 0x0003A064 File Offset: 0x00038264
		[ViewVariables]
		public JobPrototype Prototype { get; }

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000B0C RID: 2828 RVA: 0x0003A06C File Offset: 0x0003826C
		public override string Name { get; }

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000B0D RID: 2829 RVA: 0x0003A074 File Offset: 0x00038274
		public override bool Antagonist
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000B0E RID: 2830 RVA: 0x0003A077 File Offset: 0x00038277
		[Nullable(2)]
		[ViewVariables]
		public string StartingGear
		{
			[NullableContext(2)]
			get
			{
				return this.Prototype.StartingGear;
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000B0F RID: 2831 RVA: 0x0003A084 File Offset: 0x00038284
		[Nullable(2)]
		[ViewVariables]
		public string JobEntity
		{
			[NullableContext(2)]
			get
			{
				return this.Prototype.JobEntity;
			}
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x0003A091 File Offset: 0x00038291
		public Job(Mind mind, JobPrototype jobPrototype) : base(mind)
		{
			this.Prototype = jobPrototype;
			this.Name = jobPrototype.LocalizedName;
			this.CanBeAntag = jobPrototype.CanBeAntag;
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x0003A0BC File Offset: 0x000382BC
		public override void Greet()
		{
			base.Greet();
			IPlayerSession session;
			if (base.Mind.TryGetSession(out session))
			{
				IChatManager chatMgr = IoCManager.Resolve<IChatManager>();
				chatMgr.DispatchServerMessage(session, Loc.GetString("job-greet-introduce-job-name", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("jobName", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.Name))
				}), false);
				if (this.Prototype.RequireAdminNotify)
				{
					chatMgr.DispatchServerMessage(session, Loc.GetString("job-greet-important-disconnect-admin-notify"), false);
				}
				chatMgr.DispatchServerMessage(session, Loc.GetString("job-greet-supervisors-warning", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("jobName", this.Name),
					new ValueTuple<string, object>("supervisors", Loc.GetString(this.Prototype.Supervisors))
				}), false);
			}
		}

		// Token: 0x040006CA RID: 1738
		[ViewVariables]
		public bool CanBeAntag;
	}
}
