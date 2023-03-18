using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Audio
{
	// Token: 0x0200067F RID: 1663
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedAmbientSoundSystem)
	})]
	public sealed class AmbientSoundComponent : Component, IComponentTreeEntry<AmbientSoundComponent>
	{
		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x0600145D RID: 5213 RVA: 0x000440EC File Offset: 0x000422EC
		// (set) Token: 0x0600145E RID: 5214 RVA: 0x000440F4 File Offset: 0x000422F4
		[DataField("enabled", false, 1, false, false, null)]
		[ViewVariables]
		public bool Enabled { get; set; } = true;

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x0600145F RID: 5215 RVA: 0x000440FD File Offset: 0x000422FD
		// (set) Token: 0x06001460 RID: 5216 RVA: 0x00044105 File Offset: 0x00042305
		public EntityUid? TreeUid { get; set; }

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06001461 RID: 5217 RVA: 0x0004410E File Offset: 0x0004230E
		// (set) Token: 0x06001462 RID: 5218 RVA: 0x00044116 File Offset: 0x00042316
		[Nullable(new byte[]
		{
			2,
			0,
			1
		})]
		public DynamicTree<ComponentTreeEntry<AmbientSoundComponent>> Tree { [return: Nullable(new byte[]
		{
			2,
			0,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			0,
			1
		})] set; }

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06001463 RID: 5219 RVA: 0x0004411F File Offset: 0x0004231F
		public bool AddToTree
		{
			get
			{
				return this.Enabled;
			}
		}

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06001464 RID: 5220 RVA: 0x00044127 File Offset: 0x00042327
		// (set) Token: 0x06001465 RID: 5221 RVA: 0x0004412F File Offset: 0x0004232F
		public bool TreeUpdateQueued { get; set; }

		// Token: 0x04001401 RID: 5121
		[Nullable(1)]
		[DataField("sound", false, 1, true, false, null)]
		[ViewVariables]
		public SoundSpecifier Sound;

		// Token: 0x04001402 RID: 5122
		[ViewVariables]
		[DataField("range", false, 1, false, false, null)]
		public float Range = 2f;

		// Token: 0x04001403 RID: 5123
		[ViewVariables]
		[DataField("volume", false, 1, false, false, null)]
		public float Volume = -10f;
	}
}
