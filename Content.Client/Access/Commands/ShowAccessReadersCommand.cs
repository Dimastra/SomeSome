using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Access.Commands
{
	// Token: 0x020004FF RID: 1279
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShowAccessReadersCommand : IConsoleCommand
	{
		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x06002091 RID: 8337 RVA: 0x000BCDFD File Offset: 0x000BAFFD
		public string Command
		{
			get
			{
				return "showaccessreaders";
			}
		}

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x06002092 RID: 8338 RVA: 0x000BCE04 File Offset: 0x000BB004
		public string Description
		{
			get
			{
				return "Shows all access readers in the viewport";
			}
		}

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x06002093 RID: 8339 RVA: 0x000BCE0B File Offset: 0x000BB00B
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06002094 RID: 8340 RVA: 0x000BCE1C File Offset: 0x000BB01C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IDependencyCollection instance = IoCManager.Instance;
			if (instance == null)
			{
				return;
			}
			IOverlayManager overlayManager = instance.Resolve<IOverlayManager>();
			if (overlayManager.RemoveOverlay<AccessOverlay>())
			{
				shell.WriteLine("Set access reader debug overlay to false");
				return;
			}
			IEntityManager entityManager = instance.Resolve<IEntityManager>();
			IResourceCache cache = instance.Resolve<IResourceCache>();
			EntityLookupSystem entitySystem = entityManager.EntitySysManager.GetEntitySystem<EntityLookupSystem>();
			overlayManager.AddOverlay(new AccessOverlay(entityManager, cache, entitySystem));
			shell.WriteLine("Set access reader debug overlay to true");
		}
	}
}
