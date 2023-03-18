using System;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.Administration;
using Content.Server.Mind.Components;
using Content.Server.Players;
using Content.Server.Roles;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Mind.Commands
{
	// Token: 0x020003AB RID: 939
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class MindInfoCommand : IConsoleCommand
	{
		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06001338 RID: 4920 RVA: 0x0006304C File Offset: 0x0006124C
		public string Command
		{
			get
			{
				return "mindinfo";
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06001339 RID: 4921 RVA: 0x00063053 File Offset: 0x00061253
		public string Description
		{
			get
			{
				return "Lists info for the mind of a specific player.";
			}
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x0600133A RID: 4922 RVA: 0x0006305A File Offset: 0x0006125A
		public string Help
		{
			get
			{
				return "mindinfo <session ID>";
			}
		}

		// Token: 0x0600133B RID: 4923 RVA: 0x00063064 File Offset: 0x00061264
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine("Expected exactly 1 argument.");
				return;
			}
			IPlayerSession data;
			if (!IoCManager.Resolve<IPlayerManager>().TryGetSessionByUsername(args[0], ref data))
			{
				shell.WriteLine("Can't find that mind");
				return;
			}
			PlayerData playerData = data.ContentData();
			Mind mind = (playerData != null) ? playerData.Mind : null;
			if (mind == null)
			{
				shell.WriteLine("Can't find that mind");
				return;
			}
			StringBuilder builder = new StringBuilder();
			StringBuilder stringBuilder = builder;
			string format = "player: {0}, mob: {1}\nroles: ";
			object arg = mind.UserId;
			MindComponent ownedComponent = mind.OwnedComponent;
			stringBuilder.AppendFormat(format, arg, (ownedComponent != null) ? new EntityUid?(ownedComponent.Owner) : null);
			foreach (Role role in mind.AllRoles)
			{
				builder.AppendFormat("{0} ", role.Name);
			}
			shell.WriteLine(builder.ToString());
		}
	}
}
