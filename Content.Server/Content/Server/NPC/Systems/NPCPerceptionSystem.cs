using System;
using Content.Server.NPC.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.NPC.Systems
{
	// Token: 0x02000334 RID: 820
	public sealed class NPCPerceptionSystem : EntitySystem
	{
		// Token: 0x06001100 RID: 4352 RVA: 0x00057B4B File Offset: 0x00055D4B
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateRecentlyInjected(frameTime);
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x00057B5C File Offset: 0x00055D5C
		private void UpdateRecentlyInjected(float frameTime)
		{
			foreach (NPCRecentlyInjectedComponent entity in base.EntityQuery<NPCRecentlyInjectedComponent>(false))
			{
				entity.Accumulator += frameTime;
				if ((double)entity.Accumulator >= entity.RemoveTime.TotalSeconds)
				{
					entity.Accumulator = 0f;
					base.RemComp<NPCRecentlyInjectedComponent>(entity.Owner);
				}
			}
		}
	}
}
