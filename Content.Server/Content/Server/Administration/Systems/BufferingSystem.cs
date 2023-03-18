using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Components;
using Content.Shared.Administration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Administration.Systems
{
	// Token: 0x0200080E RID: 2062
	public sealed class BufferingSystem : EntitySystem
	{
		// Token: 0x06002CD1 RID: 11473 RVA: 0x000ECB40 File Offset: 0x000EAD40
		public override void Update(float frameTime)
		{
			foreach (BufferingComponent buffering in base.EntityQuery<BufferingComponent>(false))
			{
				EntityUid? bufferingIcon = buffering.BufferingIcon;
				if (bufferingIcon != null)
				{
					buffering.BufferingTimer -= frameTime;
					if (buffering.BufferingTimer <= 0f)
					{
						base.Del(buffering.BufferingIcon.Value);
						base.RemComp<AdminFrozenComponent>(buffering.Owner);
						buffering.TimeTilNextBuffer = this._random.NextFloat(buffering.MinimumTimeTilNextBuffer, buffering.MaximumTimeTilNextBuffer);
						buffering.BufferingIcon = null;
					}
				}
				else
				{
					buffering.TimeTilNextBuffer -= frameTime;
					if (buffering.TimeTilNextBuffer <= 0f)
					{
						buffering.BufferingTimer = this._random.NextFloat(buffering.MinimumBufferTime, buffering.MaximumBufferTime);
						buffering.BufferingIcon = new EntityUid?(base.Spawn("BufferingIcon", new EntityCoordinates(buffering.Owner, Vector2.Zero)));
						base.EnsureComp<AdminFrozenComponent>(buffering.Owner);
					}
				}
			}
		}

		// Token: 0x04001BCA RID: 7114
		[Nullable(1)]
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
