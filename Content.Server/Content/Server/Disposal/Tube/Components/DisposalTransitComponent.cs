using System;
using System.Runtime.CompilerServices;
using Content.Server.Disposal.Unit.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Disposal.Tube.Components
{
	// Token: 0x0200055C RID: 1372
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(IDisposalTubeComponent))]
	[Virtual]
	[ComponentReference(typeof(DisposalTubeComponent))]
	public class DisposalTransitComponent : DisposalTubeComponent
	{
		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06001CFE RID: 7422 RVA: 0x0009A373 File Offset: 0x00098573
		public override string ContainerId
		{
			get
			{
				return "DisposalTransit";
			}
		}

		// Token: 0x06001CFF RID: 7423 RVA: 0x0009A37C File Offset: 0x0009857C
		protected override Direction[] ConnectableDirections()
		{
			Angle rotation = IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(base.Owner).LocalRotation;
			Angle opposite;
			opposite..ctor(rotation.Theta + 3.141592653589793);
			return new Direction[]
			{
				rotation.GetDir(),
				opposite.GetDir()
			};
		}

		// Token: 0x06001D00 RID: 7424 RVA: 0x0009A3D0 File Offset: 0x000985D0
		public override Direction NextDirection(DisposalHolderComponent holder)
		{
			Direction[] directions = this.ConnectableDirections();
			Direction previousDF = holder.PreviousDirectionFrom;
			Direction forward = directions[0];
			if (previousDF == -1)
			{
				return forward;
			}
			Direction backward = directions[1];
			if (previousDF != forward)
			{
				return forward;
			}
			return backward;
		}
	}
}
