using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.StatusEffect;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Electrocution
{
	// Token: 0x02000533 RID: 1331
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class ElectrocuteCommand : IConsoleCommand
	{
		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06001BC9 RID: 7113 RVA: 0x00093DEA File Offset: 0x00091FEA
		public string Command
		{
			get
			{
				return "electrocute";
			}
		}

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06001BCA RID: 7114 RVA: 0x00093DF1 File Offset: 0x00091FF1
		public string Description
		{
			get
			{
				return Loc.GetString("electrocute-command-description");
			}
		}

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06001BCB RID: 7115 RVA: 0x00093DFD File Offset: 0x00091FFD
		public string Help
		{
			get
			{
				return this.Command + " <uid> <seconds> <damage>";
			}
		}

		// Token: 0x06001BCC RID: 7116 RVA: 0x00093E10 File Offset: 0x00092010
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 1)
			{
				shell.WriteError("Not enough arguments!");
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			EntityUid uid;
			if (!EntityUid.TryParse(args[0], ref uid) || !entityManager.EntityExists(uid))
			{
				shell.WriteError("Invalid entity specified!");
				return;
			}
			if (!entityManager.EntitySysManager.GetEntitySystem<StatusEffectsSystem>().CanApplyEffect(uid, "Electrocution", null))
			{
				shell.WriteError(Loc.GetString("electrocute-command-entity-cannot-be-electrocuted"));
				return;
			}
			int seconds;
			if (args.Length < 2 || !int.TryParse(args[1], out seconds))
			{
				seconds = 10;
			}
			int damage;
			if (args.Length < 3 || !int.TryParse(args[2], out damage))
			{
				damage = 10;
			}
			entityManager.EntitySysManager.GetEntitySystem<ElectrocutionSystem>().TryDoElectrocution(uid, null, damage, TimeSpan.FromSeconds((double)seconds), true, 1f, null, true);
		}

		// Token: 0x040011D3 RID: 4563
		public const string ElectrocutionStatusEffect = "Electrocution";
	}
}
