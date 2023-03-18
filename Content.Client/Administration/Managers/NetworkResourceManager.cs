using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;

namespace Content.Client.Administration.Managers
{
	// Token: 0x020004E7 RID: 1255
	public sealed class NetworkResourceManager : SharedNetworkResourceManager
	{
		// Token: 0x06001FF8 RID: 8184 RVA: 0x000B9F2E File Offset: 0x000B812E
		[NullableContext(1)]
		protected override void ResourceUploadMsg(NetworkResourceUploadMessage msg)
		{
			this.ContentRoot.AddOrUpdateFile(msg.RelativePath, msg.Data);
		}
	}
}
