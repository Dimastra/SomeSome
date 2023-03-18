using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Server.ParticleAccelerator.Components
{
	// Token: 0x020002E9 RID: 745
	public abstract class ParticleAcceleratorPartComponent : Component
	{
		// Token: 0x06000F4B RID: 3915 RVA: 0x0004E7D5 File Offset: 0x0004C9D5
		protected override void Initialize()
		{
			base.Initialize();
			IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(base.Owner).Anchored = true;
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x0004E7F3 File Offset: 0x0004C9F3
		public void OnAnchorChanged()
		{
			this.RescanIfPossible();
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x0004E7FB File Offset: 0x0004C9FB
		protected override void OnRemove()
		{
			base.OnRemove();
			this.RescanIfPossible();
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x0004E809 File Offset: 0x0004CA09
		private void RescanIfPossible()
		{
			ParticleAcceleratorControlBoxComponent master = this.Master;
			if (master == null)
			{
				return;
			}
			master.RescanParts(null);
		}

		// Token: 0x06000F4F RID: 3919 RVA: 0x0004E81C File Offset: 0x0004CA1C
		public virtual void Moved()
		{
			this.RescanIfPossible();
		}

		// Token: 0x040008FC RID: 2300
		[Nullable(2)]
		[ViewVariables]
		public ParticleAcceleratorControlBoxComponent Master;
	}
}
