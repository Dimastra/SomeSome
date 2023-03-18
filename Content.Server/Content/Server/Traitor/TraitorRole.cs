using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Server.Mind;
using Content.Server.Roles;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Traitor
{
	// Token: 0x0200010C RID: 268
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TraitorRole : Role
	{
		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060004CC RID: 1228 RVA: 0x00016EF7 File Offset: 0x000150F7
		public AntagPrototype Prototype { get; }

		// Token: 0x060004CD RID: 1229 RVA: 0x00016EFF File Offset: 0x000150FF
		public TraitorRole(Mind mind, AntagPrototype antagPrototype) : base(mind)
		{
			this.Prototype = antagPrototype;
			this.Name = Loc.GetString(antagPrototype.Name);
			this.Antagonist = antagPrototype.Antagonist;
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x060004CE RID: 1230 RVA: 0x00016F2C File Offset: 0x0001512C
		public override string Name { get; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060004CF RID: 1231 RVA: 0x00016F34 File Offset: 0x00015134
		public override bool Antagonist { get; }

		// Token: 0x060004D0 RID: 1232 RVA: 0x00016F3C File Offset: 0x0001513C
		public void GreetTraitor(string[] codewords)
		{
			IPlayerSession session;
			if (base.Mind.TryGetSession(out session))
			{
				IChatManager chatManager = IoCManager.Resolve<IChatManager>();
				chatManager.DispatchServerMessage(session, Loc.GetString("traitor-role-greeting"), false);
				chatManager.DispatchServerMessage(session, Loc.GetString("traitor-role-codewords", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("codewords", string.Join(", ", codewords))
				}), false);
			}
		}
	}
}
