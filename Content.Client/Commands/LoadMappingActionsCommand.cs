using System;
using System.Runtime.CompilerServices;
using Content.Client.Mapping;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Client.Commands
{
	// Token: 0x0200039F RID: 927
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class LoadMappingActionsCommand : IConsoleCommand
	{
		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x0600171D RID: 5917 RVA: 0x0008638C File Offset: 0x0008458C
		public string Command
		{
			get
			{
				return "loadmapacts";
			}
		}

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x0600171E RID: 5918 RVA: 0x00086393 File Offset: 0x00084593
		public string Description
		{
			get
			{
				return "Loads the mapping preset action toolbar assignments.";
			}
		}

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x0600171F RID: 5919 RVA: 0x0008639A File Offset: 0x0008459A
		public string Help
		{
			get
			{
				return "Usage: " + this.Command;
			}
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x000863AC File Offset: 0x000845AC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			try
			{
				EntitySystem.Get<MappingSystem>().LoadMappingActions();
			}
			catch
			{
				shell.WriteLine("Failed to load action assignments");
			}
		}
	}
}
