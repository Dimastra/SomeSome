using System;
using System.Runtime.CompilerServices;
using Content.Client.Weapons.Ranged.Systems;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Weapons.Ranged
{
	// Token: 0x0200002B RID: 43
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShowSpreadCommand : IConsoleCommand
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00006825 File Offset: 0x00004A25
		public string Command
		{
			get
			{
				return "showgunspread";
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x0000682C File Offset: 0x00004A2C
		public string Description
		{
			get
			{
				return "Shows gun spread overlay for debugging";
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00006833 File Offset: 0x00004A33
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00006844 File Offset: 0x00004A44
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			GunSystem entitySystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<GunSystem>();
			GunSystem gunSystem = entitySystem;
			gunSystem.SpreadOverlay = !gunSystem.SpreadOverlay;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Set spread overlay to ");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(entitySystem.SpreadOverlay);
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
