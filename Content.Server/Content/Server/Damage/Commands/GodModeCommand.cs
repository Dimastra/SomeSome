using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Damage.Systems;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Damage.Commands
{
	// Token: 0x020005D1 RID: 1489
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class GodModeCommand : IConsoleCommand
	{
		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06001FAF RID: 8111 RVA: 0x000A5C4C File Offset: 0x000A3E4C
		public string Command
		{
			get
			{
				return "godmode";
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06001FB0 RID: 8112 RVA: 0x000A5C53 File Offset: 0x000A3E53
		public string Description
		{
			get
			{
				return "Makes your entity or another invulnerable to almost anything. May have irreversible changes.";
			}
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06001FB1 RID: 8113 RVA: 0x000A5C5C File Offset: 0x000A3E5C
		public string Help
		{
			get
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Usage: ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				defaultInterpolatedStringHandler.AppendLiteral(" / ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				defaultInterpolatedStringHandler.AppendLiteral(" <entityUid>");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
		}

		// Token: 0x06001FB2 RID: 8114 RVA: 0x000A5CB8 File Offset: 0x000A3EB8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			int num = args.Length;
			EntityUid entity;
			if (num != 0)
			{
				if (num != 1)
				{
					shell.WriteLine(this.Help);
					return;
				}
				EntityUid id;
				if (!EntityUid.TryParse(args[0], ref id))
				{
					shell.WriteLine(args[0] + " isn't a valid entity id.");
					return;
				}
				if (!entityManager.EntityExists(id))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
					defaultInterpolatedStringHandler.AppendLiteral("No entity found with id ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(id);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
					return;
				}
				entity = id;
			}
			else
			{
				if (player == null)
				{
					shell.WriteLine("An entity needs to be specified when the command isn't used by a player.");
					return;
				}
				if (player.AttachedEntity == null)
				{
					shell.WriteLine("An entity needs to be specified when you aren't attached to an entity.");
					return;
				}
				entity = player.AttachedEntity.Value;
			}
			bool enabled = EntitySystem.Get<GodmodeSystem>().ToggleGodmode(entity);
			string name = entityManager.GetComponent<MetaDataComponent>(entity).EntityName;
			string text;
			if (!enabled)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Disabled godmode for entity ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				defaultInterpolatedStringHandler.AppendLiteral(" with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
				text = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Enabled godmode for entity ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				defaultInterpolatedStringHandler.AppendLiteral(" with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
				text = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			shell.WriteLine(text);
		}
	}
}
