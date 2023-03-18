using System;
using System.Runtime.CompilerServices;
using Content.Server.Disposal.Unit.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Disposal.Tube.Components
{
	// Token: 0x02000557 RID: 1367
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(IDisposalTubeComponent))]
	[ComponentReference(typeof(DisposalTubeComponent))]
	public sealed class DisposalBendComponent : DisposalTubeComponent
	{
		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06001CDF RID: 7391 RVA: 0x00099D90 File Offset: 0x00097F90
		public override string ContainerId
		{
			get
			{
				return "DisposalBend";
			}
		}

		// Token: 0x06001CE0 RID: 7392 RVA: 0x00099D98 File Offset: 0x00097F98
		protected override Direction[] ConnectableDirections()
		{
			Angle direction = IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(base.Owner).LocalRotation;
			Angle side;
			side..ctor(MathHelper.DegreesToRadians(direction.Degrees + (double)this._sideDegrees));
			return new Direction[]
			{
				direction.GetDir(),
				side.GetDir()
			};
		}

		// Token: 0x06001CE1 RID: 7393 RVA: 0x00099DF0 File Offset: 0x00097FF0
		public override Direction NextDirection(DisposalHolderComponent holder)
		{
			Direction[] directions = this.ConnectableDirections();
			Direction previousDF = holder.PreviousDirectionFrom;
			if (previousDF == -1)
			{
				return directions[0];
			}
			if (previousDF != directions[0])
			{
				return directions[0];
			}
			return directions[1];
		}

		// Token: 0x04001278 RID: 4728
		[DataField("sideDegrees", false, 1, false, false, null)]
		private int _sideDegrees = -90;
	}
}
