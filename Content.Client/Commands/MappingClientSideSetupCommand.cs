using System;
using System.Runtime.CompilerServices;
using Content.Client.Markers;
using Robust.Client.Graphics;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands
{
	// Token: 0x020003AB RID: 939
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class MappingClientSideSetupCommand : IConsoleCommand
	{
		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x0600175A RID: 5978 RVA: 0x000869D0 File Offset: 0x00084BD0
		public string Command
		{
			get
			{
				return "mappingclientsidesetup";
			}
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x0600175B RID: 5979 RVA: 0x000869D7 File Offset: 0x00084BD7
		public string Description
		{
			get
			{
				return "Sets up the lighting control and such settings client-side. Sent by 'mapping' to client.";
			}
		}

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x0600175C RID: 5980 RVA: 0x00054CC5 File Offset: 0x00052EC5
		public string Help
		{
			get
			{
				return "";
			}
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x000869E0 File Offset: 0x00084BE0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			ILightManager lightManager = IoCManager.Resolve<ILightManager>();
			if (!lightManager.LockConsoleAccess)
			{
				EntitySystem.Get<MarkerSystem>().MarkersVisible = true;
				lightManager.Enabled = false;
				shell.ExecuteCommand("showsubfloorforever");
				shell.ExecuteCommand("loadmapacts");
			}
		}
	}
}
