using System;
using System.Runtime.CompilerServices;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Systems
{
	// Token: 0x02000047 RID: 71
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedFlyBySoundSystem : EntitySystem
	{
		// Token: 0x06000097 RID: 151 RVA: 0x000033D4 File Offset: 0x000015D4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FlyBySoundComponent, ComponentGetState>(new ComponentEventRefHandler<FlyBySoundComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<FlyBySoundComponent, ComponentHandleState>(new ComponentEventRefHandler<FlyBySoundComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<FlyBySoundComponent, ComponentStartup>(new ComponentEventHandler<FlyBySoundComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<FlyBySoundComponent, ComponentShutdown>(new ComponentEventHandler<FlyBySoundComponent, ComponentShutdown>(this.OnShutdown), null, null);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00003438 File Offset: 0x00001638
		private void OnStartup(EntityUid uid, FlyBySoundComponent component, ComponentStartup args)
		{
			PhysicsComponent body;
			if (!base.TryComp<PhysicsComponent>(uid, ref body))
			{
				return;
			}
			PhysShapeCircle shape = new PhysShapeCircle(component.Range);
			this._fixtures.TryCreateFixture(uid, shape, "fly-by", 1f, false, 30, 0, 0.4f, 0f, true, null, body, null);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00003488 File Offset: 0x00001688
		private void OnShutdown(EntityUid uid, FlyBySoundComponent component, ComponentShutdown args)
		{
			PhysicsComponent body;
			if (!base.TryComp<PhysicsComponent>(uid, ref body) || base.MetaData(uid).EntityLifeStage >= 4)
			{
				return;
			}
			this._fixtures.DestroyFixture(uid, "fly-by", true, body, null, null);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000034C8 File Offset: 0x000016C8
		private void OnHandleState(EntityUid uid, FlyBySoundComponent component, ref ComponentHandleState args)
		{
			SharedFlyBySoundSystem.FlyBySoundComponentState state = args.Current as SharedFlyBySoundSystem.FlyBySoundComponentState;
			if (state == null)
			{
				return;
			}
			component.Sound = state.Sound;
			component.Range = state.Range;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000034FD File Offset: 0x000016FD
		private void OnGetState(EntityUid uid, FlyBySoundComponent component, ref ComponentGetState args)
		{
			args.State = new SharedFlyBySoundSystem.FlyBySoundComponentState
			{
				Sound = component.Sound,
				Range = component.Range
			};
		}

		// Token: 0x040000C8 RID: 200
		[Dependency]
		private readonly FixtureSystem _fixtures;

		// Token: 0x040000C9 RID: 201
		public const string FlyByFixture = "fly-by";

		// Token: 0x02000781 RID: 1921
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class FlyBySoundComponentState : ComponentState
		{
			// Token: 0x0400176B RID: 5995
			[Nullable(1)]
			public SoundSpecifier Sound;

			// Token: 0x0400176C RID: 5996
			public float Range;
		}
	}
}
