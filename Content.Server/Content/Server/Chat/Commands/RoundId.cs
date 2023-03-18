using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.Chat.Commands
{
	// Token: 0x020006D2 RID: 1746
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	internal sealed class RoundId : IConsoleCommand
	{
		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x06002472 RID: 9330 RVA: 0x000BE112 File Offset: 0x000BC312
		public string Command
		{
			get
			{
				return "roundid";
			}
		}

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06002473 RID: 9331 RVA: 0x000BE119 File Offset: 0x000BC319
		public string Description
		{
			get
			{
				return "Shows the id of the current round.";
			}
		}

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06002474 RID: 9332 RVA: 0x000BE120 File Offset: 0x000BC320
		public string Help
		{
			get
			{
				return "Write roundid, output *Current round #roundId*";
			}
		}

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06002475 RID: 9333 RVA: 0x000BE127 File Offset: 0x000BC327
		private int _roundId
		{
			get
			{
				return EntitySystem.Get<GameTicker>().RoundId;
			}
		}

		// Token: 0x06002476 RID: 9334 RVA: 0x000BE134 File Offset: 0x000BC334
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Current round #");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this._roundId);
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
