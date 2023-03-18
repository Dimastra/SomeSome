using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.CCVar;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Roles;
using Robust.Client;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Client.Players.PlayTimeTracking
{
	// Token: 0x020001B6 RID: 438
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PlayTimeTrackingManager
	{
		// Token: 0x06000B4D RID: 2893 RVA: 0x00041A6C File Offset: 0x0003FC6C
		public void Initialize()
		{
			this._net.RegisterNetMessage<MsgPlayTime>(new ProcessMessage<MsgPlayTime>(this.RxPlayTime), 3);
			this._client.RunLevelChanged += this.ClientOnRunLevelChanged;
		}

		// Token: 0x06000B4E RID: 2894 RVA: 0x00041A9D File Offset: 0x0003FC9D
		private void ClientOnRunLevelChanged([Nullable(2)] object sender, RunLevelChangedEventArgs e)
		{
			if (e.NewLevel == 1)
			{
				this._roles.Clear();
			}
		}

		// Token: 0x06000B4F RID: 2895 RVA: 0x00041AB4 File Offset: 0x0003FCB4
		private void RxPlayTime(MsgPlayTime message)
		{
			this._roles.Clear();
			foreach (KeyValuePair<string, TimeSpan> keyValuePair in message.Trackers)
			{
				string text;
				TimeSpan timeSpan;
				keyValuePair.Deconstruct(out text, out timeSpan);
				string key = text;
				TimeSpan value = timeSpan;
				this._roles[key] = value;
			}
		}

		// Token: 0x06000B50 RID: 2896 RVA: 0x00041B2C File Offset: 0x0003FD2C
		public bool IsAllowed(JobPrototype job, [Nullable(2)] [NotNullWhen(false)] out string reason)
		{
			reason = null;
			if (job.Requirements == null || !this._cfg.GetCVar<bool>(CCVars.GameRoleTimers))
			{
				return true;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (((localPlayer != null) ? localPlayer.Session : null) == null)
			{
				return true;
			}
			Dictionary<string, TimeSpan> roles = this._roles;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			using (HashSet<JobRequirement>.Enumerator enumerator = job.Requirements.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!JobRequirements.TryRequirementMet(enumerator.Current, roles, out reason, this._prototypes))
					{
						if (!flag)
						{
							stringBuilder.Append('\n');
						}
						flag = false;
						stringBuilder.AppendLine(reason);
					}
				}
			}
			reason = ((stringBuilder.Length == 0) ? null : stringBuilder.ToString());
			return reason == null;
		}

		// Token: 0x04000587 RID: 1415
		[Dependency]
		private readonly IBaseClient _client;

		// Token: 0x04000588 RID: 1416
		[Dependency]
		private readonly IClientNetManager _net;

		// Token: 0x04000589 RID: 1417
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x0400058A RID: 1418
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400058B RID: 1419
		[Dependency]
		private readonly IPrototypeManager _prototypes;

		// Token: 0x0400058C RID: 1420
		private readonly Dictionary<string, TimeSpan> _roles = new Dictionary<string, TimeSpan>();
	}
}
