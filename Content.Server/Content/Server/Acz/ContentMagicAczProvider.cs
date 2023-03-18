using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Robust.Packaging;
using Robust.Packaging.AssetProcessing;
using Robust.Server.ServerStatus;
using Robust.Shared.IoC;

namespace Content.Server.Acz
{
	// Token: 0x02000872 RID: 2162
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ContentMagicAczProvider : IMagicAczProvider
	{
		// Token: 0x06002F48 RID: 12104 RVA: 0x000F48DB File Offset: 0x000F2ADB
		public ContentMagicAczProvider(IDependencyCollection deps)
		{
			this._deps = deps;
		}

		// Token: 0x06002F49 RID: 12105 RVA: 0x000F48EC File Offset: 0x000F2AEC
		public Task Package(AssetPass pass, IPackageLogger logger, CancellationToken cancel)
		{
			ContentMagicAczProvider.<Package>d__2 <Package>d__;
			<Package>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Package>d__.<>4__this = this;
			<Package>d__.pass = pass;
			<Package>d__.logger = logger;
			<Package>d__.cancel = cancel;
			<Package>d__.<>1__state = -1;
			<Package>d__.<>t__builder.Start<ContentMagicAczProvider.<Package>d__2>(ref <Package>d__);
			return <Package>d__.<>t__builder.Task;
		}

		// Token: 0x04001C6D RID: 7277
		private readonly IDependencyCollection _deps;
	}
}
