using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Disposal.Tube.Components;
using Content.Shared.Body.Components;
using Content.Shared.Item;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disposal.Unit.Components
{
	// Token: 0x02000554 RID: 1364
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DisposalHolderComponent : Component, IGasMixtureHolder
	{
		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06001CB7 RID: 7351 RVA: 0x0009979B File Offset: 0x0009799B
		// (set) Token: 0x06001CB8 RID: 7352 RVA: 0x000997A3 File Offset: 0x000979A3
		[ViewVariables]
		public float StartingTime { get; set; }

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06001CB9 RID: 7353 RVA: 0x000997AC File Offset: 0x000979AC
		// (set) Token: 0x06001CBA RID: 7354 RVA: 0x000997B4 File Offset: 0x000979B4
		[ViewVariables]
		public float TimeLeft { get; set; }

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06001CBB RID: 7355 RVA: 0x000997BD File Offset: 0x000979BD
		// (set) Token: 0x06001CBC RID: 7356 RVA: 0x000997C5 File Offset: 0x000979C5
		[Nullable(2)]
		[ViewVariables]
		public IDisposalTubeComponent PreviousTube { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06001CBD RID: 7357 RVA: 0x000997CE File Offset: 0x000979CE
		// (set) Token: 0x06001CBE RID: 7358 RVA: 0x000997D6 File Offset: 0x000979D6
		[ViewVariables]
		public Direction PreviousDirection { get; set; } = -1;

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06001CBF RID: 7359 RVA: 0x000997DF File Offset: 0x000979DF
		[ViewVariables]
		public Direction PreviousDirectionFrom
		{
			get
			{
				if (this.PreviousDirection != -1)
				{
					return DirectionExtensions.GetOpposite(this.PreviousDirection);
				}
				return -1;
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06001CC0 RID: 7360 RVA: 0x000997F7 File Offset: 0x000979F7
		// (set) Token: 0x06001CC1 RID: 7361 RVA: 0x000997FF File Offset: 0x000979FF
		[Nullable(2)]
		[ViewVariables]
		public IDisposalTubeComponent CurrentTube { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06001CC2 RID: 7362 RVA: 0x00099808 File Offset: 0x00097A08
		// (set) Token: 0x06001CC3 RID: 7363 RVA: 0x00099810 File Offset: 0x00097A10
		[ViewVariables]
		public Direction CurrentDirection { get; set; } = -1;

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06001CC4 RID: 7364 RVA: 0x00099819 File Offset: 0x00097A19
		// (set) Token: 0x06001CC5 RID: 7365 RVA: 0x00099821 File Offset: 0x00097A21
		[ViewVariables]
		public bool IsExitingDisposals { get; set; }

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06001CC6 RID: 7366 RVA: 0x0009982A File Offset: 0x00097A2A
		// (set) Token: 0x06001CC7 RID: 7367 RVA: 0x00099832 File Offset: 0x00097A32
		[ViewVariables]
		public HashSet<string> Tags { get; set; } = new HashSet<string>();

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06001CC8 RID: 7368 RVA: 0x0009983B File Offset: 0x00097A3B
		// (set) Token: 0x06001CC9 RID: 7369 RVA: 0x00099843 File Offset: 0x00097A43
		[DataField("air", false, 1, false, false, null)]
		public GasMixture Air { get; set; } = new GasMixture(70f);

		// Token: 0x06001CCA RID: 7370 RVA: 0x0009984C File Offset: 0x00097A4C
		protected override void Initialize()
		{
			base.Initialize();
			this.Container = ContainerHelpers.EnsureContainer<Container>(base.Owner, "DisposalHolderComponent", null);
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x0009986B File Offset: 0x00097A6B
		private bool CanInsert(EntityUid entity)
		{
			return this.Container.CanInsert(entity, null) && (this._entMan.HasComponent<ItemComponent>(entity) || this._entMan.HasComponent<BodyComponent>(entity));
		}

		// Token: 0x06001CCC RID: 7372 RVA: 0x0009989C File Offset: 0x00097A9C
		public bool TryInsert(EntityUid entity)
		{
			if (!this.CanInsert(entity) || !this.Container.Insert(entity, null, null, null, null, null))
			{
				return false;
			}
			PhysicsComponent physics;
			if (this._entMan.TryGetComponent<PhysicsComponent>(entity, ref physics))
			{
				this._entMan.System<SharedPhysicsSystem>().SetCanCollide(entity, false, true, false, null, physics);
			}
			return true;
		}

		// Token: 0x0400125D RID: 4701
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x0400125E RID: 4702
		public Container Container;
	}
}
