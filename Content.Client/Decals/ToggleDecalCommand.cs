using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Client.Decals
{
	// Token: 0x0200035C RID: 860
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ToggleDecalCommand : IConsoleCommand
	{
		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06001542 RID: 5442 RVA: 0x0007D1FC File Offset: 0x0007B3FC
		public string Command
		{
			get
			{
				return "toggledecals";
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06001543 RID: 5443 RVA: 0x0007D203 File Offset: 0x0007B403
		public string Description
		{
			get
			{
				return "Toggles decaloverlay";
			}
		}

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06001544 RID: 5444 RVA: 0x0007D20A File Offset: 0x0007B40A
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06001545 RID: 5445 RVA: 0x0007D21B File Offset: 0x0007B41B
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			EntitySystem.Get<DecalSystem>().ToggleOverlay();
		}
	}
}
