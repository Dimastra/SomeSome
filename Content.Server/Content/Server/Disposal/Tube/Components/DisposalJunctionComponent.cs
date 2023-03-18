using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Disposal.Unit.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Disposal.Tube.Components
{
	// Token: 0x02000559 RID: 1369
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	[RegisterComponent]
	[ComponentReference(typeof(IDisposalTubeComponent))]
	[ComponentReference(typeof(DisposalTubeComponent))]
	public class DisposalJunctionComponent : DisposalTubeComponent
	{
		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06001CE8 RID: 7400 RVA: 0x00099F45 File Offset: 0x00098145
		public override string ContainerId
		{
			get
			{
				return "DisposalJunction";
			}
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x00099F4C File Offset: 0x0009814C
		protected override Direction[] ConnectableDirections()
		{
			Angle direction = this._entMan.GetComponent<TransformComponent>(base.Owner).LocalRotation;
			return (from degree in this._degrees
			select new Angle(degree.Theta + direction.Theta).GetDir()).ToArray<Direction>();
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x00099F98 File Offset: 0x00098198
		public override Direction NextDirection(DisposalHolderComponent holder)
		{
			Direction next = this._entMan.GetComponent<TransformComponent>(base.Owner).LocalRotation.GetDir();
			Direction[] directions = this.ConnectableDirections().Skip(1).ToArray<Direction>();
			if (holder.PreviousDirectionFrom == -1 || holder.PreviousDirectionFrom == next)
			{
				return RandomExtensions.Pick<Direction>(this._random, directions);
			}
			return next;
		}

		// Token: 0x0400127B RID: 4731
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x0400127C RID: 4732
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400127D RID: 4733
		[DataField("degrees", false, 1, false, false, null)]
		private List<Angle> _degrees = new List<Angle>();
	}
}
