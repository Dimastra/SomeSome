using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Replays;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.Administration
{
	// Token: 0x02000800 RID: 2048
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NetworkResourceManager : SharedNetworkResourceManager
	{
		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x06002C43 RID: 11331 RVA: 0x000E732F File Offset: 0x000E552F
		// (set) Token: 0x06002C44 RID: 11332 RVA: 0x000E7337 File Offset: 0x000E5537
		[ViewVariables]
		public bool Enabled { get; private set; } = true;

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x06002C45 RID: 11333 RVA: 0x000E7340 File Offset: 0x000E5540
		// (set) Token: 0x06002C46 RID: 11334 RVA: 0x000E7348 File Offset: 0x000E5548
		[ViewVariables]
		public float SizeLimit { get; private set; }

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x06002C47 RID: 11335 RVA: 0x000E7351 File Offset: 0x000E5551
		// (set) Token: 0x06002C48 RID: 11336 RVA: 0x000E7359 File Offset: 0x000E5559
		[ViewVariables]
		public bool StoreUploaded { get; set; } = true;

		// Token: 0x06002C49 RID: 11337 RVA: 0x000E7364 File Offset: 0x000E5564
		public override void Initialize()
		{
			base.Initialize();
			this._serverNetManager.Connected += this.ServerNetManagerOnConnected;
			this._cfgManager.OnValueChanged<bool>(CCVars.ResourceUploadingEnabled, delegate(bool value)
			{
				this.Enabled = value;
			}, true);
			this._cfgManager.OnValueChanged<float>(CCVars.ResourceUploadingLimitMb, delegate(float value)
			{
				this.SizeLimit = value;
			}, true);
			this._cfgManager.OnValueChanged<bool>(CCVars.ResourceUploadingStoreEnabled, delegate(bool value)
			{
				this.StoreUploaded = value;
			}, true);
			this.AutoDelete(this._cfgManager.GetCVar<int>(CCVars.ResourceUploadingStoreDeletionDays));
		}

		// Token: 0x06002C4A RID: 11338 RVA: 0x000E73FC File Offset: 0x000E55FC
		private void OnStartReplayRecording([Nullable(new byte[]
		{
			0,
			1,
			1,
			1
		})] ValueTuple<MappingDataNode, List<object>> initReplayData)
		{
			foreach (ValueTuple<ResourcePath, byte[]> valueTuple in this.ContentRoot.GetAllFiles())
			{
				ResourcePath path = valueTuple.Item1;
				byte[] data = valueTuple.Item2;
				initReplayData.Item2.Add(new SharedNetworkResourceManager.ReplayResourceUploadMsg
				{
					RelativePath = path,
					Data = data
				});
			}
		}

		// Token: 0x06002C4B RID: 11339 RVA: 0x000E7474 File Offset: 0x000E5674
		protected override void ResourceUploadMsg(NetworkResourceUploadMessage msg)
		{
			NetworkResourceManager.<ResourceUploadMsg>d__20 <ResourceUploadMsg>d__;
			<ResourceUploadMsg>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ResourceUploadMsg>d__.<>4__this = this;
			<ResourceUploadMsg>d__.msg = msg;
			<ResourceUploadMsg>d__.<>1__state = -1;
			<ResourceUploadMsg>d__.<>t__builder.Start<NetworkResourceManager.<ResourceUploadMsg>d__20>(ref <ResourceUploadMsg>d__);
		}

		// Token: 0x06002C4C RID: 11340 RVA: 0x000E74B4 File Offset: 0x000E56B4
		private void ServerNetManagerOnConnected([Nullable(2)] object sender, NetChannelArgs e)
		{
			foreach (ValueTuple<ResourcePath, byte[]> valueTuple in this.ContentRoot.GetAllFiles())
			{
				ResourcePath path = valueTuple.Item1;
				byte[] data = valueTuple.Item2;
				NetworkResourceUploadMessage msg = new NetworkResourceUploadMessage();
				msg.RelativePath = path;
				msg.Data = data;
				e.Channel.SendMessage(msg);
			}
		}

		// Token: 0x06002C4D RID: 11341 RVA: 0x000E752C File Offset: 0x000E572C
		private void AutoDelete(int days)
		{
			NetworkResourceManager.<AutoDelete>d__22 <AutoDelete>d__;
			<AutoDelete>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AutoDelete>d__.<>4__this = this;
			<AutoDelete>d__.days = days;
			<AutoDelete>d__.<>1__state = -1;
			<AutoDelete>d__.<>t__builder.Start<NetworkResourceManager.<AutoDelete>d__22>(ref <AutoDelete>d__);
		}

		// Token: 0x04001B6E RID: 7022
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001B6F RID: 7023
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04001B70 RID: 7024
		[Dependency]
		private readonly IServerNetManager _serverNetManager;

		// Token: 0x04001B71 RID: 7025
		[Dependency]
		private readonly IConfigurationManager _cfgManager;

		// Token: 0x04001B72 RID: 7026
		[Dependency]
		private readonly IServerDbManager _serverDb;

		// Token: 0x04001B73 RID: 7027
		[Dependency]
		private readonly IReplayRecordingManager _replay;
	}
}
