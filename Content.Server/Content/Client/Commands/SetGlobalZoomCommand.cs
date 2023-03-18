using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Commands
{
	// Token: 0x02000011 RID: 17
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class SetGlobalZoomCommand : IConsoleCommand
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002B0E File Offset: 0x00000D0E
		public string Command
		{
			get
			{
				return "setglobalzoom";
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002B15 File Offset: 0x00000D15
		public string Description
		{
			get
			{
				return "Sets the global zoom for all characters.";
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002B1C File Offset: 0x00000D1C
		public string Help
		{
			get
			{
				return "setglobalzoom <float>";
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002B24 File Offset: 0x00000D24
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine("Invalid number of arguments.");
				return;
			}
			float zoom;
			if (!float.TryParse(args[0], out zoom))
			{
				shell.WriteLine("Invalid zoom value.");
				return;
			}
			EntityManager entityManager = IoCManager.Resolve<EntityManager>();
			foreach (SharedEyeComponent eye in entityManager.GetAllComponents(typeof(SharedEyeComponent), true).Cast<SharedEyeComponent>().ToList<SharedEyeComponent>())
			{
				eye.Zoom = new Vector2(zoom, zoom);
				entityManager.Dirty(eye, null);
			}
		}
	}
}
