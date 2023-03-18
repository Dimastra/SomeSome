using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Disposal.Unit.Components;
using Content.Server.Disposal.Unit.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Disposal.Tube.Components
{
	// Token: 0x02000558 RID: 1368
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(IDisposalTubeComponent))]
	[ComponentReference(typeof(DisposalTubeComponent))]
	public sealed class DisposalEntryComponent : DisposalTubeComponent
	{
		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06001CE3 RID: 7395 RVA: 0x00099E30 File Offset: 0x00098030
		public override string ContainerId
		{
			get
			{
				return "DisposalEntry";
			}
		}

		// Token: 0x06001CE4 RID: 7396 RVA: 0x00099E38 File Offset: 0x00098038
		public bool TryInsert(DisposalUnitComponent from, [Nullable(new byte[]
		{
			2,
			1
		})] IEnumerable<string> tags = null)
		{
			EntityUid holder = this._entMan.SpawnEntity("DisposalHolder", this._entMan.GetComponent<TransformComponent>(base.Owner).MapPosition);
			DisposalHolderComponent holderComponent = this._entMan.GetComponent<DisposalHolderComponent>(holder);
			foreach (EntityUid entity in from.Container.ContainedEntities.ToArray<EntityUid>())
			{
				holderComponent.TryInsert(entity);
			}
			EntitySystem.Get<AtmosphereSystem>().Merge(holderComponent.Air, from.Air);
			from.Air.Clear();
			if (tags != null)
			{
				holderComponent.Tags.UnionWith(tags);
			}
			return EntitySystem.Get<DisposableSystem>().EnterTube(holderComponent.Owner, base.Owner, holderComponent, null, this, null);
		}

		// Token: 0x06001CE5 RID: 7397 RVA: 0x00099EF4 File Offset: 0x000980F4
		protected override Direction[] ConnectableDirections()
		{
			return new Direction[]
			{
				this._entMan.GetComponent<TransformComponent>(base.Owner).LocalRotation.GetDir()
			};
		}

		// Token: 0x06001CE6 RID: 7398 RVA: 0x00099F28 File Offset: 0x00098128
		public override Direction NextDirection(DisposalHolderComponent holder)
		{
			if (holder.PreviousDirectionFrom != -1)
			{
				return -1;
			}
			return this.ConnectableDirections()[0];
		}

		// Token: 0x04001279 RID: 4729
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x0400127A RID: 4730
		private const string HolderPrototypeId = "DisposalHolder";
	}
}
