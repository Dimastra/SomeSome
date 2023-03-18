using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules.Configurations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004B9 RID: 1209
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class GameRuleSystem : EntitySystem
	{
		// Token: 0x1700037D RID: 893
		// (get) Token: 0x0600189A RID: 6298 RVA: 0x0007FCF0 File Offset: 0x0007DEF0
		// (set) Token: 0x0600189B RID: 6299 RVA: 0x0007FCF8 File Offset: 0x0007DEF8
		public bool RuleAdded { get; protected set; }

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x0600189C RID: 6300 RVA: 0x0007FD01 File Offset: 0x0007DF01
		// (set) Token: 0x0600189D RID: 6301 RVA: 0x0007FD09 File Offset: 0x0007DF09
		public bool RuleStarted { get; protected set; }

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x0600189E RID: 6302
		public abstract string Prototype { get; }

		// Token: 0x0600189F RID: 6303 RVA: 0x0007FD14 File Offset: 0x0007DF14
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GameRuleAddedEvent>(new EntityEventHandler<GameRuleAddedEvent>(this.OnGameRuleAdded), null, null);
			base.SubscribeLocalEvent<GameRuleStartedEvent>(new EntityEventHandler<GameRuleStartedEvent>(this.OnGameRuleStarted), null, null);
			base.SubscribeLocalEvent<GameRuleEndedEvent>(new EntityEventHandler<GameRuleEndedEvent>(this.OnGameRuleEnded), null, null);
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x0007FD63 File Offset: 0x0007DF63
		private void OnGameRuleAdded(GameRuleAddedEvent ev)
		{
			if (ev.Rule.Configuration.Id != this.Prototype)
			{
				return;
			}
			this.Configuration = ev.Rule.Configuration;
			this.RuleAdded = true;
			this.Added();
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x0007FDA1 File Offset: 0x0007DFA1
		private void OnGameRuleStarted(GameRuleStartedEvent ev)
		{
			if (ev.Rule.Configuration.Id != this.Prototype)
			{
				return;
			}
			this.RuleStarted = true;
			this.Started();
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x0007FDCE File Offset: 0x0007DFCE
		private void OnGameRuleEnded(GameRuleEndedEvent ev)
		{
			if (ev.Rule.Configuration.Id != this.Prototype)
			{
				return;
			}
			this.RuleAdded = false;
			this.RuleStarted = false;
			this.Ended();
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x0007FE02 File Offset: 0x0007E002
		public virtual void Added()
		{
		}

		// Token: 0x060018A4 RID: 6308
		public abstract void Started();

		// Token: 0x060018A5 RID: 6309
		public abstract void Ended();

		// Token: 0x04000F55 RID: 3925
		[Dependency]
		protected GameTicker GameTicker;

		// Token: 0x04000F58 RID: 3928
		public GameRuleConfiguration Configuration;
	}
}
