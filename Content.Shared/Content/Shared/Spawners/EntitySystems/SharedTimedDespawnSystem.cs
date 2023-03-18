using System;
using System.Runtime.CompilerServices;
using Content.Shared.Spawners.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Spawners.EntitySystems
{
	// Token: 0x02000183 RID: 387
	public abstract class SharedTimedDespawnSystem : EntitySystem
	{
		// Token: 0x060004AA RID: 1194 RVA: 0x0001222E File Offset: 0x0001042E
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesOutsidePrediction = true;
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00012240 File Offset: 0x00010440
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this._timing.IsFirstTimePredicted)
			{
				return;
			}
			foreach (TimedDespawnComponent comp in base.EntityQuery<TimedDespawnComponent>(false))
			{
				if (this.CanDelete(comp.Owner))
				{
					comp.Lifetime -= frameTime;
					if (comp.Lifetime <= 0f)
					{
						this.EntityManager.QueueDeleteEntity(comp.Owner);
					}
				}
			}
		}

		// Token: 0x060004AC RID: 1196
		protected abstract bool CanDelete(EntityUid uid);

		// Token: 0x04000450 RID: 1104
		[Nullable(1)]
		[Dependency]
		private readonly IGameTiming _timing;
	}
}
