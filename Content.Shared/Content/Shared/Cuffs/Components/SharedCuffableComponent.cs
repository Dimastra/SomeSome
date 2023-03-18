using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cuffs.Components
{
	// Token: 0x0200054A RID: 1354
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	public abstract class SharedCuffableComponent : Component
	{
		// Token: 0x1700033D RID: 829
		// (get) Token: 0x0600107C RID: 4220 RVA: 0x000360D8 File Offset: 0x000342D8
		[ViewVariables]
		public int CuffedHandCount
		{
			get
			{
				return this.Container.ContainedEntities.Count * 2;
			}
		}

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x0600107D RID: 4221 RVA: 0x000360EC File Offset: 0x000342EC
		public EntityUid LastAddedCuffs
		{
			get
			{
				IReadOnlyList<EntityUid> containedEntities = this.Container.ContainedEntities;
				return containedEntities[containedEntities.Count - 1];
			}
		}

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x0600107E RID: 4222 RVA: 0x00036106 File Offset: 0x00034306
		public IReadOnlyList<EntityUid> StoredEntities
		{
			get
			{
				return this.Container.ContainedEntities;
			}
		}

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x0600107F RID: 4223 RVA: 0x00036113 File Offset: 0x00034313
		// (set) Token: 0x06001080 RID: 4224 RVA: 0x0003611B File Offset: 0x0003431B
		[ViewVariables]
		public Container Container { get; set; }

		// Token: 0x06001081 RID: 4225 RVA: 0x00036124 File Offset: 0x00034324
		protected override void Initialize()
		{
			base.Initialize();
			this.Container = this._sysMan.GetEntitySystem<SharedContainerSystem>().EnsureContainer<Container>(base.Owner, this._componentFactory.GetComponentName(base.GetType()), null);
		}

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06001082 RID: 4226 RVA: 0x0003615A File Offset: 0x0003435A
		// (set) Token: 0x06001083 RID: 4227 RVA: 0x00036162 File Offset: 0x00034362
		[ViewVariables]
		public bool CanStillInteract { get; set; } = true;

		// Token: 0x04000F7C RID: 3964
		[Dependency]
		private readonly IEntitySystemManager _sysMan;

		// Token: 0x04000F7D RID: 3965
		[Dependency]
		private readonly IComponentFactory _componentFactory;

		// Token: 0x02000838 RID: 2104
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class CuffableComponentState : ComponentState
		{
			// Token: 0x17000518 RID: 1304
			// (get) Token: 0x0600191D RID: 6429 RVA: 0x0004F90B File Offset: 0x0004DB0B
			public bool CanStillInteract { get; }

			// Token: 0x17000519 RID: 1305
			// (get) Token: 0x0600191E RID: 6430 RVA: 0x0004F913 File Offset: 0x0004DB13
			public int NumHandsCuffed { get; }

			// Token: 0x1700051A RID: 1306
			// (get) Token: 0x0600191F RID: 6431 RVA: 0x0004F91B File Offset: 0x0004DB1B
			[Nullable(2)]
			public string RSI { [NullableContext(2)] get; }

			// Token: 0x1700051B RID: 1307
			// (get) Token: 0x06001920 RID: 6432 RVA: 0x0004F923 File Offset: 0x0004DB23
			public string IconState { get; }

			// Token: 0x1700051C RID: 1308
			// (get) Token: 0x06001921 RID: 6433 RVA: 0x0004F92B File Offset: 0x0004DB2B
			public Color Color { get; }

			// Token: 0x06001922 RID: 6434 RVA: 0x0004F933 File Offset: 0x0004DB33
			public CuffableComponentState(int numHandsCuffed, bool canStillInteract, [Nullable(2)] string rsiPath, string iconState, Color color)
			{
				this.NumHandsCuffed = numHandsCuffed;
				this.CanStillInteract = canStillInteract;
				this.RSI = rsiPath;
				this.IconState = iconState;
				this.Color = color;
			}
		}
	}
}
