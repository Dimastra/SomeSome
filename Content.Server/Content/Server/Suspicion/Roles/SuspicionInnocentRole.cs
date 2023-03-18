using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Server.Mind;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Suspicion.Roles
{
	// Token: 0x02000138 RID: 312
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SuspicionInnocentRole : SuspicionRole
	{
		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060005B6 RID: 1462 RVA: 0x0001C0D7 File Offset: 0x0001A2D7
		public AntagPrototype Prototype { get; }

		// Token: 0x060005B7 RID: 1463 RVA: 0x0001C0DF File Offset: 0x0001A2DF
		public SuspicionInnocentRole(Mind mind, AntagPrototype antagPrototype) : base(mind)
		{
			this.Prototype = antagPrototype;
			this.Name = Loc.GetString(antagPrototype.Name);
			this.Antagonist = antagPrototype.Antagonist;
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060005B8 RID: 1464 RVA: 0x0001C10C File Offset: 0x0001A30C
		public override string Name { get; }

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060005B9 RID: 1465 RVA: 0x0001C114 File Offset: 0x0001A314
		public string Objective
		{
			get
			{
				return Loc.GetString(this.Prototype.Objective);
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x0001C126 File Offset: 0x0001A326
		public override bool Antagonist { get; }

		// Token: 0x060005BB RID: 1467 RVA: 0x0001C130 File Offset: 0x0001A330
		public override void Greet()
		{
			base.Greet();
			IChatManager chat = IoCManager.Resolve<IChatManager>();
			IPlayerSession session;
			if (base.Mind.TryGetSession(out session))
			{
				chat.DispatchServerMessage(session, "You're an " + this.Name + "!", false);
				chat.DispatchServerMessage(session, "Objective: " + this.Objective, false);
			}
		}
	}
}
