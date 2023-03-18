using System;
using System.Runtime.CompilerServices;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Administration
{
	// Token: 0x02000746 RID: 1862
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedNetworkResourceManager : IDisposable
	{
		// Token: 0x06001699 RID: 5785 RVA: 0x00049B32 File Offset: 0x00047D32
		public virtual void Initialize()
		{
			this._netManager.RegisterNetMessage<NetworkResourceUploadMessage>(new ProcessMessage<NetworkResourceUploadMessage>(this.ResourceUploadMsg), 3);
			this.ResourceManager.AddRoot(SharedNetworkResourceManager.Prefix, this.ContentRoot);
		}

		// Token: 0x0600169A RID: 5786
		protected abstract void ResourceUploadMsg(NetworkResourceUploadMessage msg);

		// Token: 0x0600169B RID: 5787 RVA: 0x00049B63 File Offset: 0x00047D63
		public void Dispose()
		{
			this.ContentRoot.Dispose();
		}

		// Token: 0x040016D3 RID: 5843
		[Dependency]
		private readonly INetManager _netManager;

		// Token: 0x040016D4 RID: 5844
		[Dependency]
		protected readonly IResourceManager ResourceManager;

		// Token: 0x040016D5 RID: 5845
		public const double BytesToMegabytes = 1E-06;

		// Token: 0x040016D6 RID: 5846
		private static readonly ResourcePath Prefix = ResourcePath.Root / "Uploaded";

		// Token: 0x040016D7 RID: 5847
		protected readonly MemoryContentRoot ContentRoot = new MemoryContentRoot();

		// Token: 0x0200089B RID: 2203
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class ReplayResourceUploadMsg
		{
			// Token: 0x04001A8B RID: 6795
			public byte[] Data;

			// Token: 0x04001A8C RID: 6796
			public ResourcePath RelativePath;
		}
	}
}
