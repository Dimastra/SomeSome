using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Server.Mind;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Localization;

namespace Content.Server.Suspicion.Roles
{
	// Token: 0x0200013A RID: 314
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SuspicionTraitorRole : SuspicionRole
	{
		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060005BD RID: 1469 RVA: 0x0001C196 File Offset: 0x0001A396
		public AntagPrototype Prototype { get; }

		// Token: 0x060005BE RID: 1470 RVA: 0x0001C19E File Offset: 0x0001A39E
		public SuspicionTraitorRole(Mind mind, AntagPrototype antagPrototype) : base(mind)
		{
			this.Prototype = antagPrototype;
			this.Name = Loc.GetString(antagPrototype.Name);
			this.Antagonist = antagPrototype.Antagonist;
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x0001C1CB File Offset: 0x0001A3CB
		public override string Name { get; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060005C0 RID: 1472 RVA: 0x0001C1D3 File Offset: 0x0001A3D3
		public string Objective
		{
			get
			{
				return Loc.GetString(this.Prototype.Objective);
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x0001C1E5 File Offset: 0x0001A3E5
		public override bool Antagonist { get; }

		// Token: 0x060005C2 RID: 1474 RVA: 0x0001C1F0 File Offset: 0x0001A3F0
		public void GreetSuspicion(List<SuspicionTraitorRole> traitors, IChatManager chatMgr)
		{
			IPlayerSession session;
			if (base.Mind.TryGetSession(out session))
			{
				chatMgr.DispatchServerMessage(session, Loc.GetString("suspicion-role-greeting", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("roleName", this.Name)
				}), false);
				chatMgr.DispatchServerMessage(session, Loc.GetString("suspicion-objective", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("objectiveText", this.Objective)
				}), false);
				string allPartners = string.Join(", ", from p in traitors
				where p != this
				select p.Mind.CharacterName);
				string partnerText = Loc.GetString("suspicion-partners-in-crime", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("partnersCount", traitors.Count - 1),
					new ValueTuple<string, object>("partnerNames", allPartners)
				});
				chatMgr.DispatchServerMessage(session, partnerText, false);
			}
		}
	}
}
