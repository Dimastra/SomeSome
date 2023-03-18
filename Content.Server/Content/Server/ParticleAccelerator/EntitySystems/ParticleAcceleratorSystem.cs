using System;
using System.Runtime.CompilerServices;
using Content.Server.ParticleAccelerator.Components;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Events;

namespace Content.Server.ParticleAccelerator.EntitySystems
{
	// Token: 0x020002E3 RID: 739
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ParticleAcceleratorSystem : EntitySystem
	{
		// Token: 0x06000F1F RID: 3871 RVA: 0x0004D971 File Offset: 0x0004BB71
		private void InitializeControlBoxSystem()
		{
			ComponentEventRefHandler<ParticleAcceleratorControlBoxComponent, PowerChangedEvent> componentEventRefHandler;
			if ((componentEventRefHandler = ParticleAcceleratorSystem.<>O.<0>__OnControlBoxPowerChange) == null)
			{
				componentEventRefHandler = (ParticleAcceleratorSystem.<>O.<0>__OnControlBoxPowerChange = new ComponentEventRefHandler<ParticleAcceleratorControlBoxComponent, PowerChangedEvent>(ParticleAcceleratorSystem.OnControlBoxPowerChange));
			}
			base.SubscribeLocalEvent<ParticleAcceleratorControlBoxComponent, PowerChangedEvent>(componentEventRefHandler, null, null);
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x0004D996 File Offset: 0x0004BB96
		private static void OnControlBoxPowerChange(EntityUid uid, ParticleAcceleratorControlBoxComponent component, ref PowerChangedEvent args)
		{
			component.OnPowerStateChanged(args);
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x0004D9A4 File Offset: 0x0004BBA4
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeControlBoxSystem();
			this.InitializePartSystem();
			this.InitializePowerBoxSystem();
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x0004D9C0 File Offset: 0x0004BBC0
		private void InitializePartSystem()
		{
			ComponentEventRefHandler<ParticleAcceleratorPartComponent, MoveEvent> componentEventRefHandler;
			if ((componentEventRefHandler = ParticleAcceleratorSystem.<>O.<1>__OnMoveEvent) == null)
			{
				componentEventRefHandler = (ParticleAcceleratorSystem.<>O.<1>__OnMoveEvent = new ComponentEventRefHandler<ParticleAcceleratorPartComponent, MoveEvent>(ParticleAcceleratorSystem.OnMoveEvent));
			}
			base.SubscribeLocalEvent<ParticleAcceleratorPartComponent, MoveEvent>(componentEventRefHandler, null, null);
			ComponentEventRefHandler<ParticleAcceleratorPartComponent, PhysicsBodyTypeChangedEvent> componentEventRefHandler2;
			if ((componentEventRefHandler2 = ParticleAcceleratorSystem.<>O.<2>__BodyTypeChanged) == null)
			{
				componentEventRefHandler2 = (ParticleAcceleratorSystem.<>O.<2>__BodyTypeChanged = new ComponentEventRefHandler<ParticleAcceleratorPartComponent, PhysicsBodyTypeChangedEvent>(ParticleAcceleratorSystem.BodyTypeChanged));
			}
			base.SubscribeLocalEvent<ParticleAcceleratorPartComponent, PhysicsBodyTypeChangedEvent>(componentEventRefHandler2, null, null);
		}

		// Token: 0x06000F23 RID: 3875 RVA: 0x0004DA13 File Offset: 0x0004BC13
		private static void BodyTypeChanged(EntityUid uid, ParticleAcceleratorPartComponent component, ref PhysicsBodyTypeChangedEvent args)
		{
			component.OnAnchorChanged();
		}

		// Token: 0x06000F24 RID: 3876 RVA: 0x0004DA1B File Offset: 0x0004BC1B
		private static void OnMoveEvent(EntityUid uid, ParticleAcceleratorPartComponent component, ref MoveEvent args)
		{
			component.Moved();
		}

		// Token: 0x06000F25 RID: 3877 RVA: 0x0004DA23 File Offset: 0x0004BC23
		private void InitializePowerBoxSystem()
		{
			ComponentEventRefHandler<ParticleAcceleratorPowerBoxComponent, PowerConsumerReceivedChanged> componentEventRefHandler;
			if ((componentEventRefHandler = ParticleAcceleratorSystem.<>O.<3>__PowerBoxReceivedChanged) == null)
			{
				componentEventRefHandler = (ParticleAcceleratorSystem.<>O.<3>__PowerBoxReceivedChanged = new ComponentEventRefHandler<ParticleAcceleratorPowerBoxComponent, PowerConsumerReceivedChanged>(ParticleAcceleratorSystem.PowerBoxReceivedChanged));
			}
			base.SubscribeLocalEvent<ParticleAcceleratorPowerBoxComponent, PowerConsumerReceivedChanged>(componentEventRefHandler, null, null);
		}

		// Token: 0x06000F26 RID: 3878 RVA: 0x0004DA48 File Offset: 0x0004BC48
		private static void PowerBoxReceivedChanged(EntityUid uid, ParticleAcceleratorPowerBoxComponent component, ref PowerConsumerReceivedChanged args)
		{
			ParticleAcceleratorControlBoxComponent master = component.Master;
			if (master == null)
			{
				return;
			}
			master.PowerBoxReceivedChanged(args);
		}

		// Token: 0x02000955 RID: 2389
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04001FE5 RID: 8165
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<ParticleAcceleratorControlBoxComponent, PowerChangedEvent> <0>__OnControlBoxPowerChange;

			// Token: 0x04001FE6 RID: 8166
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<ParticleAcceleratorPartComponent, MoveEvent> <1>__OnMoveEvent;

			// Token: 0x04001FE7 RID: 8167
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<ParticleAcceleratorPartComponent, PhysicsBodyTypeChangedEvent> <2>__BodyTypeChanged;

			// Token: 0x04001FE8 RID: 8168
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<ParticleAcceleratorPowerBoxComponent, PowerConsumerReceivedChanged> <3>__PowerBoxReceivedChanged;
		}
	}
}
