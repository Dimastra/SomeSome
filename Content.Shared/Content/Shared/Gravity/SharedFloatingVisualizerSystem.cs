using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared.Gravity
{
	// Token: 0x02000448 RID: 1096
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedFloatingVisualizerSystem : EntitySystem
	{
		// Token: 0x06000D4E RID: 3406 RVA: 0x0002BF44 File Offset: 0x0002A144
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FloatingVisualsComponent, ComponentStartup>(new ComponentEventHandler<FloatingVisualsComponent, ComponentStartup>(this.OnComponentStartup), null, null);
			base.SubscribeLocalEvent<GravityChangedEvent>(new EntityEventRefHandler<GravityChangedEvent>(this.OnGravityChanged), null, null);
			base.SubscribeLocalEvent<FloatingVisualsComponent, EntParentChangedMessage>(new ComponentEventRefHandler<FloatingVisualsComponent, EntParentChangedMessage>(this.OnEntParentChanged), null, null);
			base.SubscribeLocalEvent<FloatingVisualsComponent, ComponentGetState>(new ComponentEventRefHandler<FloatingVisualsComponent, ComponentGetState>(this.OnComponentGetState), null, null);
			base.SubscribeLocalEvent<FloatingVisualsComponent, ComponentHandleState>(new ComponentEventRefHandler<FloatingVisualsComponent, ComponentHandleState>(this.OnComponentHandleState), null, null);
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x0002BFBB File Offset: 0x0002A1BB
		public virtual void FloatAnimation(EntityUid uid, Vector2 offset, string animationKey, float animationTime, bool stop = false)
		{
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x0002BFC0 File Offset: 0x0002A1C0
		protected bool CanFloat(EntityUid uid, FloatingVisualsComponent component, [Nullable(2)] TransformComponent transform = null)
		{
			if (!base.Resolve<TransformComponent>(uid, ref transform, true))
			{
				return false;
			}
			if (transform.MapID == MapId.Nullspace)
			{
				return false;
			}
			component.CanFloat = this.GravitySystem.IsWeightless(uid, null, transform);
			base.Dirty(component, null);
			return component.CanFloat;
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x0002C011 File Offset: 0x0002A211
		private void OnComponentStartup(EntityUid uid, FloatingVisualsComponent component, ComponentStartup args)
		{
			if (this.CanFloat(uid, component, null))
			{
				this.FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime, false);
			}
		}

		// Token: 0x06000D52 RID: 3410 RVA: 0x0002C038 File Offset: 0x0002A238
		private void OnGravityChanged(ref GravityChangedEvent args)
		{
			foreach (ValueTuple<FloatingVisualsComponent, TransformComponent> valueTuple in base.EntityQuery<FloatingVisualsComponent, TransformComponent>(true))
			{
				FloatingVisualsComponent floating = valueTuple.Item1;
				TransformComponent transform = valueTuple.Item2;
				if (!(transform.MapID == MapId.Nullspace))
				{
					EntityUid? gridUid = transform.GridUid;
					EntityUid changedGridIndex = args.ChangedGridIndex;
					if (gridUid != null && (gridUid == null || !(gridUid.GetValueOrDefault() != changedGridIndex)))
					{
						floating.CanFloat = !args.HasGravity;
						base.Dirty(floating, null);
						EntityUid uid = floating.Owner;
						if (!args.HasGravity)
						{
							this.FloatAnimation(uid, floating.Offset, floating.AnimationKey, floating.AnimationTime, false);
						}
					}
				}
			}
		}

		// Token: 0x06000D53 RID: 3411 RVA: 0x0002C11C File Offset: 0x0002A31C
		private void OnEntParentChanged(EntityUid uid, FloatingVisualsComponent component, ref EntParentChangedMessage args)
		{
			TransformComponent transform = args.Transform;
			if (this.CanFloat(uid, component, transform))
			{
				this.FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime, false);
			}
		}

		// Token: 0x06000D54 RID: 3412 RVA: 0x0002C155 File Offset: 0x0002A355
		private void OnComponentGetState(EntityUid uid, FloatingVisualsComponent component, ref ComponentGetState args)
		{
			args.State = new SharedFloatingVisualsComponentState(component.AnimationTime, component.Offset, component.CanFloat);
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x0002C174 File Offset: 0x0002A374
		private void OnComponentHandleState(EntityUid uid, FloatingVisualsComponent component, ref ComponentHandleState args)
		{
			SharedFloatingVisualsComponentState state = args.Current as SharedFloatingVisualsComponentState;
			if (state == null)
			{
				return;
			}
			component.AnimationTime = state.AnimationTime;
			component.Offset = state.Offset;
			component.CanFloat = state.HasGravity;
		}

		// Token: 0x04000CD2 RID: 3282
		[Dependency]
		private readonly SharedGravitySystem GravitySystem;
	}
}
