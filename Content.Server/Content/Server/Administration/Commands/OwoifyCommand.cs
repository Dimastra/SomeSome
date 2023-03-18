using System;
using System.Runtime.CompilerServices;
using Content.Server.Speech.EntitySystems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000849 RID: 2121
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class OwoifyCommand : IConsoleCommand
	{
		// Token: 0x17000763 RID: 1891
		// (get) Token: 0x06002E65 RID: 11877 RVA: 0x000F1EF9 File Offset: 0x000F00F9
		public string Command
		{
			get
			{
				return "owoify";
			}
		}

		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x06002E66 RID: 11878 RVA: 0x000F1F00 File Offset: 0x000F0100
		public string Description
		{
			get
			{
				return "For when you need everything to be cat. Uses OwOAccent's formatting on the name and description of an entity.";
			}
		}

		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x06002E67 RID: 11879 RVA: 0x000F1F07 File Offset: 0x000F0107
		public string Help
		{
			get
			{
				return "owoify <id>";
			}
		}

		// Token: 0x06002E68 RID: 11880 RVA: 0x000F1F10 File Offset: 0x000F0110
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			int targetId;
			if (!int.TryParse(args[0], out targetId))
			{
				shell.WriteLine(Loc.GetString("shell-argument-must-be-number"));
				return;
			}
			EntityUid eUid;
			eUid..ctor(targetId);
			MetaDataComponent meta = entityManager.GetComponent<MetaDataComponent>(eUid);
			IoCManager.Resolve<IRobustRandom>();
			OwOAccentSystem owoSys = EntitySystem.Get<OwOAccentSystem>();
			meta.EntityName = owoSys.Accentuate(meta.EntityName);
			meta.EntityDescription = owoSys.Accentuate(meta.EntityDescription);
		}
	}
}
