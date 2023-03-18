using System;
using System.Runtime.CompilerServices;
using Content.Server.Engineering.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Engineering.EntitySystems
{
	// Token: 0x0200052B RID: 1323
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DisassembleOnAltVerbSystem : EntitySystem
	{
		// Token: 0x06001B91 RID: 7057 RVA: 0x0009396C File Offset: 0x00091B6C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DisassembleOnAltVerbComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<DisassembleOnAltVerbComponent, GetVerbsEvent<AlternativeVerb>>(this.AddDisassembleVerb), null, null);
		}

		// Token: 0x06001B92 RID: 7058 RVA: 0x00093988 File Offset: 0x00091B88
		private void AddDisassembleVerb(EntityUid uid, DisassembleOnAltVerbComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess || args.Hands == null)
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate()
				{
					this.AttemptDisassemble(uid, args.User, args.Target, component);
				},
				Text = Loc.GetString("disassemble-system-verb-disassemble"),
				Priority = 2
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06001B93 RID: 7059 RVA: 0x00093A20 File Offset: 0x00091C20
		[NullableContext(2)]
		public void AttemptDisassemble(EntityUid uid, EntityUid user, EntityUid target, DisassembleOnAltVerbComponent component = null)
		{
			DisassembleOnAltVerbSystem.<AttemptDisassemble>d__3 <AttemptDisassemble>d__;
			<AttemptDisassemble>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AttemptDisassemble>d__.<>4__this = this;
			<AttemptDisassemble>d__.uid = uid;
			<AttemptDisassemble>d__.user = user;
			<AttemptDisassemble>d__.component = component;
			<AttemptDisassemble>d__.<>1__state = -1;
			<AttemptDisassemble>d__.<>t__builder.Start<DisassembleOnAltVerbSystem.<AttemptDisassemble>d__3>(ref <AttemptDisassemble>d__);
		}

		// Token: 0x040011AB RID: 4523
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;
	}
}
