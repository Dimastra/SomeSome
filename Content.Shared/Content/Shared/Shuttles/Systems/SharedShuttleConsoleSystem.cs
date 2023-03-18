using System;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Movement.Events;
using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Systems
{
	// Token: 0x020001BA RID: 442
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedShuttleConsoleSystem : EntitySystem
	{
		// Token: 0x0600051E RID: 1310 RVA: 0x0001379C File Offset: 0x0001199C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PilotComponent, UpdateCanMoveEvent>(new ComponentEventHandler<PilotComponent, UpdateCanMoveEvent>(this.HandleMovementBlock), null, null);
			base.SubscribeLocalEvent<PilotComponent, ComponentStartup>(new ComponentEventHandler<PilotComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<PilotComponent, ComponentShutdown>(new ComponentEventHandler<PilotComponent, ComponentShutdown>(this.HandlePilotShutdown), null, null);
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x000137EC File Offset: 0x000119EC
		protected virtual void HandlePilotShutdown(EntityUid uid, PilotComponent component, ComponentShutdown args)
		{
			this.ActionBlockerSystem.UpdateCanMove(uid, null);
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x000137FC File Offset: 0x000119FC
		private void OnStartup(EntityUid uid, PilotComponent component, ComponentStartup args)
		{
			this.ActionBlockerSystem.UpdateCanMove(uid, null);
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0001380C File Offset: 0x00011A0C
		private void HandleMovementBlock(EntityUid uid, PilotComponent component, UpdateCanMoveEvent args)
		{
			if (component.LifeStage > 6)
			{
				return;
			}
			if (component.Console == null)
			{
				return;
			}
			args.Cancel();
		}

		// Token: 0x04000512 RID: 1298
		[Dependency]
		protected readonly ActionBlockerSystem ActionBlockerSystem;

		// Token: 0x020007AC RID: 1964
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class PilotComponentState : ComponentState
		{
			// Token: 0x170004F3 RID: 1267
			// (get) Token: 0x060017FA RID: 6138 RVA: 0x0004D3AA File Offset: 0x0004B5AA
			public EntityUid? Console { get; }

			// Token: 0x060017FB RID: 6139 RVA: 0x0004D3B2 File Offset: 0x0004B5B2
			public PilotComponentState(EntityUid? uid)
			{
				this.Console = uid;
			}
		}
	}
}
