using System;
using System.Runtime.CompilerServices;
using Content.Shared.Audio;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Physics;

namespace Content.Client.Audio
{
	// Token: 0x0200042D RID: 1069
	[Nullable(new byte[]
	{
		0,
		1,
		1
	})]
	public sealed class AmbientSoundTreeSystem : ComponentTreeSystem<AmbientSoundTreeComponent, AmbientSoundComponent>
	{
		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06001A0B RID: 6667 RVA: 0x00003C59 File Offset: 0x00001E59
		protected override bool DoFrameUpdate
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06001A0C RID: 6668 RVA: 0x00003C56 File Offset: 0x00001E56
		protected override bool DoTickUpdate
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06001A0D RID: 6669 RVA: 0x000951DB File Offset: 0x000933DB
		protected override int InitialCapacity
		{
			get
			{
				return 256;
			}
		}

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06001A0E RID: 6670 RVA: 0x00003C56 File Offset: 0x00001E56
		protected override bool Recursive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001A0F RID: 6671 RVA: 0x000951E2 File Offset: 0x000933E2
		protected override Box2 ExtractAabb([Nullable(new byte[]
		{
			0,
			1
		})] in ComponentTreeEntry<AmbientSoundComponent> entry, Vector2 pos, Angle rot)
		{
			return new Box2(pos - entry.Component.Range, pos + entry.Component.Range);
		}

		// Token: 0x06001A10 RID: 6672 RVA: 0x0009520C File Offset: 0x0009340C
		protected override Box2 ExtractAabb([Nullable(new byte[]
		{
			0,
			1
		})] in ComponentTreeEntry<AmbientSoundComponent> entry)
		{
			if (entry.Component.TreeUid == null)
			{
				return default(Box2);
			}
			Vector2 relativePosition = this.XformSystem.GetRelativePosition(entry.Transform, entry.Component.TreeUid.Value, base.GetEntityQuery<TransformComponent>());
			return this.ExtractAabb(ref entry, relativePosition, default(Angle));
		}
	}
}
