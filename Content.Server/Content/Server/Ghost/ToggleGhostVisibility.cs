using System;
using System.Runtime.CompilerServices;
using Content.Server.Ghost.Components;
using Content.Shared.Administration;
using Robust.Server.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Players;

namespace Content.Server.Ghost
{
	// Token: 0x02000492 RID: 1170
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class ToggleGhostVisibility : IConsoleCommand
	{
		// Token: 0x1700032B RID: 811
		// (get) Token: 0x0600177B RID: 6011 RVA: 0x0007B492 File Offset: 0x00079692
		public string Command
		{
			get
			{
				return "toggleghosts";
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x0600177C RID: 6012 RVA: 0x0007B499 File Offset: 0x00079699
		public string Description
		{
			get
			{
				return "Toggles ghost visibility";
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x0600177D RID: 6013 RVA: 0x0007B4A0 File Offset: 0x000796A0
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x0007B4B4 File Offset: 0x000796B4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (shell.Player == null)
			{
				shell.WriteLine("You can only toggle ghost visibility on a client.");
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			ICommonSession player = shell.Player;
			EntityUid? uid = (player != null) ? player.AttachedEntity : null;
			EyeComponent eyeComponent;
			if (uid == null || !entityManager.HasComponent<GhostComponent>(uid) || !entityManager.TryGetComponent<EyeComponent>(uid, ref eyeComponent))
			{
				return;
			}
			eyeComponent.VisibilityMask ^= 2U;
		}
	}
}
