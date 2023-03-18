using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Physics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.Eye
{
	// Token: 0x0200031A RID: 794
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EyeLerpingSystem : EntitySystem
	{
		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x0600140C RID: 5132 RVA: 0x00075DDB File Offset: 0x00073FDB
		[ViewVariables]
		private IEnumerable<LerpingEyeComponent> ActiveEyes
		{
			get
			{
				return base.EntityQuery<LerpingEyeComponent>(false);
			}
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x00075DE4 File Offset: 0x00073FE4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EyeComponent, ComponentStartup>(new ComponentEventHandler<EyeComponent, ComponentStartup>(this.OnEyeStartup), null, null);
			base.SubscribeLocalEvent<EyeComponent, ComponentShutdown>(new ComponentEventHandler<EyeComponent, ComponentShutdown>(this.OnEyeShutdown), null, null);
			base.SubscribeLocalEvent<LerpingEyeComponent, EntParentChangedMessage>(new ComponentEventRefHandler<LerpingEyeComponent, EntParentChangedMessage>(this.HandleMapChange), null, null);
			base.SubscribeLocalEvent<EyeComponent, PlayerAttachedEvent>(new ComponentEventHandler<EyeComponent, PlayerAttachedEvent>(this.OnAttached), null, null);
			base.SubscribeLocalEvent<LerpingEyeComponent, PlayerDetachedEvent>(new ComponentEventHandler<LerpingEyeComponent, PlayerDetachedEvent>(this.OnDetached), null, null);
			base.UpdatesAfter.Add(typeof(TransformSystem));
			base.UpdatesAfter.Add(typeof(PhysicsSystem));
			base.UpdatesBefore.Add(typeof(EyeUpdateSystem));
			base.UpdatesOutsidePrediction = true;
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x00075EA4 File Offset: 0x000740A4
		private void OnEyeStartup(EntityUid uid, EyeComponent component, ComponentStartup args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = false;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity != null && (controlledEntity == null || controlledEntity.GetValueOrDefault() == uid));
			}
			if (flag)
			{
				this.AddEye(uid, component, true);
			}
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x00075EFB File Offset: 0x000740FB
		private void OnEyeShutdown(EntityUid uid, EyeComponent component, ComponentShutdown args)
		{
			base.RemCompDeferred<LerpingEyeComponent>(uid);
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x00075F08 File Offset: 0x00074108
		[NullableContext(2)]
		public void AddEye(EntityUid uid, EyeComponent component = null, bool automatic = false)
		{
			if (!base.Resolve<EyeComponent>(uid, ref component, true))
			{
				return;
			}
			LerpingEyeComponent lerpingEyeComponent = base.EnsureComp<LerpingEyeComponent>(uid);
			lerpingEyeComponent.TargetRotation = this.GetRotation(uid, null, null);
			lerpingEyeComponent.LastRotation = lerpingEyeComponent.TargetRotation;
			lerpingEyeComponent.ManuallyAdded |= !automatic;
			if (component.Eye != null)
			{
				component.Eye.Rotation = lerpingEyeComponent.TargetRotation;
			}
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x00075F70 File Offset: 0x00074170
		public void RemoveEye(EntityUid uid)
		{
			LerpingEyeComponent lerpingEyeComponent;
			if (!base.TryComp<LerpingEyeComponent>(uid, ref lerpingEyeComponent))
			{
				return;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = false;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity != null && (controlledEntity == null || controlledEntity.GetValueOrDefault() == uid));
			}
			if (flag)
			{
				lerpingEyeComponent.ManuallyAdded = false;
				return;
			}
			base.RemComp(uid, lerpingEyeComponent);
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x00075FDA File Offset: 0x000741DA
		private void HandleMapChange(EntityUid uid, LerpingEyeComponent component, ref EntParentChangedMessage args)
		{
			if (args.OldMapId != args.Transform.MapID)
			{
				component.LastRotation = this.GetRotation(uid, args.Transform, null);
			}
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x00076008 File Offset: 0x00074208
		private void OnAttached(EntityUid uid, EyeComponent component, PlayerAttachedEvent args)
		{
			this.AddEye(uid, component, true);
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x00076013 File Offset: 0x00074213
		private void OnDetached(EntityUid uid, LerpingEyeComponent component, PlayerDetachedEvent args)
		{
			if (!component.ManuallyAdded)
			{
				base.RemCompDeferred(uid, component);
			}
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x00076028 File Offset: 0x00074228
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this._gameTiming.IsFirstTimePredicted)
			{
				return;
			}
			foreach (ValueTuple<LerpingEyeComponent, TransformComponent> valueTuple in base.EntityQuery<LerpingEyeComponent, TransformComponent>(false))
			{
				LerpingEyeComponent item = valueTuple.Item1;
				TransformComponent item2 = valueTuple.Item2;
				item.LastRotation = item.TargetRotation;
				item.TargetRotation = this.GetRotation(item.Owner, item2, null);
			}
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x000760B0 File Offset: 0x000742B0
		[NullableContext(2)]
		private bool NeedsLerp(InputMoverComponent mover)
		{
			return mover != null && !mover.RelativeRotation.Equals(mover.TargetRelativeRotation);
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x000760D0 File Offset: 0x000742D0
		[NullableContext(2)]
		private Angle GetRotation(EntityUid uid, TransformComponent xform = null, InputMoverComponent mover = null)
		{
			if (!base.Resolve<TransformComponent>(uid, ref xform, true))
			{
				return Angle.Zero;
			}
			if (base.Resolve<InputMoverComponent>(uid, ref mover, false))
			{
				return -this._mover.GetParentGridAngle(mover);
			}
			EntityUid? gridUid = xform.GridUid;
			EntityUid? entityUid = (gridUid != null) ? gridUid : xform.MapUid;
			if (entityUid != null)
			{
				return -base.Transform(entityUid.Value).WorldRotation;
			}
			return Angle.Zero;
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x00076150 File Offset: 0x00074350
		public override void FrameUpdate(float frameTime)
		{
			float num = (float)this._gameTiming.TickFraction / 65535f;
			foreach (ValueTuple<LerpingEyeComponent, EyeComponent, TransformComponent> valueTuple in base.EntityQuery<LerpingEyeComponent, EyeComponent, TransformComponent>(false))
			{
				LerpingEyeComponent item = valueTuple.Item1;
				EyeComponent item2 = valueTuple.Item2;
				TransformComponent item3 = valueTuple.Item3;
				EntityUid owner = item2.Owner;
				InputMoverComponent mover;
				base.TryComp<InputMoverComponent>(owner, ref mover);
				item.TargetRotation = this.GetRotation(owner, item3, mover);
				if (!this.NeedsLerp(mover))
				{
					item2.Rotation = item.TargetRotation;
				}
				else
				{
					Angle angle = Angle.ShortestDistance(ref item.LastRotation, ref item.TargetRotation);
					if (Math.Abs(angle.Theta) < 1E-05)
					{
						item2.Rotation = item.TargetRotation;
					}
					else
					{
						item2.Rotation = angle * (double)num + item.LastRotation;
					}
				}
			}
		}

		// Token: 0x04000A0D RID: 2573
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000A0E RID: 2574
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000A0F RID: 2575
		[Dependency]
		private readonly SharedMoverController _mover;
	}
}
