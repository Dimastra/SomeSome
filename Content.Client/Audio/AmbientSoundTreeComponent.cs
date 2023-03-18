using System;
using System.Runtime.CompilerServices;
using Content.Shared.Audio;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics;

namespace Content.Client.Audio
{
	// Token: 0x0200042C RID: 1068
	[RegisterComponent]
	public sealed class AmbientSoundTreeComponent : Component, IComponentTreeComponent<AmbientSoundComponent>
	{
		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06001A08 RID: 6664 RVA: 0x000951CA File Offset: 0x000933CA
		// (set) Token: 0x06001A09 RID: 6665 RVA: 0x000951D2 File Offset: 0x000933D2
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public DynamicTree<ComponentTreeEntry<AmbientSoundComponent>> Tree { [return: Nullable(new byte[]
		{
			1,
			0,
			1
		})] get; [param: Nullable(new byte[]
		{
			1,
			0,
			1
		})] set; }
	}
}
