using System;
using System.Runtime.CompilerServices;
using Content.Client.Markers;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands
{
	// Token: 0x020003A4 RID: 932
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ShowMarkersCommand : IConsoleCommand
	{
		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06001736 RID: 5942 RVA: 0x000865A5 File Offset: 0x000847A5
		public string Command
		{
			get
			{
				return "showmarkers";
			}
		}

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06001737 RID: 5943 RVA: 0x000865AC File Offset: 0x000847AC
		public string Description
		{
			get
			{
				return "Toggles visibility of markers such as spawn points.";
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06001738 RID: 5944 RVA: 0x00054CC5 File Offset: 0x00052EC5
		public string Help
		{
			get
			{
				return "";
			}
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x000865B3 File Offset: 0x000847B3
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			MarkerSystem entitySystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<MarkerSystem>();
			entitySystem.MarkersVisible = !entitySystem.MarkersVisible;
		}
	}
}
