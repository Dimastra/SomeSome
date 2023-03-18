using System;
using System.Runtime.CompilerServices;
using Content.Shared.Database;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Players;
using Robust.Shared.Utility;

namespace Content.Server.Administration.Notes
{
	// Token: 0x02000813 RID: 2067
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminNotesSystem : EntitySystem
	{
		// Token: 0x06002D08 RID: 11528 RVA: 0x000EDC9B File Offset: 0x000EBE9B
		public override void Initialize()
		{
			base.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.AddVerbs), null, null);
		}

		// Token: 0x06002D09 RID: 11529 RVA: 0x000EDCB4 File Offset: 0x000EBEB4
		private void AddVerbs(GetVerbsEvent<Verb> ev)
		{
			ActorComponent componentOrNull = EntityManagerExt.GetComponentOrNull<ActorComponent>(this.EntityManager, ev.User);
			if (componentOrNull != null)
			{
				IPlayerSession user = componentOrNull.PlayerSession;
				componentOrNull = EntityManagerExt.GetComponentOrNull<ActorComponent>(this.EntityManager, ev.Target);
				if (componentOrNull != null)
				{
					IPlayerSession target = componentOrNull.PlayerSession;
					if (!this._notes.CanView(user))
					{
						return;
					}
					Verb verb = new Verb
					{
						Text = Loc.GetString("admin-notes-verb-text"),
						Category = VerbCategory.Admin,
						Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/examine.svg.192dpi.png", "/")),
						Act = delegate()
						{
							IConsoleHost console = this._console;
							ICommonSession user = user;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
							defaultInterpolatedStringHandler.AppendFormatted("adminnotes");
							defaultInterpolatedStringHandler.AppendLiteral(" \"");
							defaultInterpolatedStringHandler.AppendFormatted<NetUserId>(target.UserId);
							defaultInterpolatedStringHandler.AppendLiteral("\"");
							console.RemoteExecuteCommand(user, defaultInterpolatedStringHandler.ToStringAndClear());
						},
						Impact = LogImpact.Low
					};
					ev.Verbs.Add(verb);
					return;
				}
			}
		}

		// Token: 0x04001BEE RID: 7150
		[Dependency]
		private readonly IConsoleHost _console;

		// Token: 0x04001BEF RID: 7151
		[Dependency]
		private readonly IAdminNotesManager _notes;
	}
}
