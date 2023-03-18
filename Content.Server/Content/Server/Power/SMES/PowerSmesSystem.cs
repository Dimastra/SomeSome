using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Power.SMES
{
	// Token: 0x02000276 RID: 630
	internal sealed class PowerSmesSystem : EntitySystem
	{
		// Token: 0x06000CA9 RID: 3241 RVA: 0x00042258 File Offset: 0x00040458
		public override void Update(float frameTime)
		{
			foreach (SmesComponent smesComponent in this.EntityManager.EntityQuery<SmesComponent>(true))
			{
				smesComponent.OnUpdate();
			}
		}
	}
}
