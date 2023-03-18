using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Shuttles.Events;
using Content.Shared.Interaction;
using Content.Shared.Pinpointer;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Server.Pinpointer
{
	// Token: 0x020002D8 RID: 728
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ServerPinpointerSystem : SharedPinpointerSystem
	{
		// Token: 0x06000EC7 RID: 3783 RVA: 0x0004ACB8 File Offset: 0x00048EB8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PinpointerComponent, ActivateInWorldEvent>(new ComponentEventHandler<PinpointerComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<HyperspaceJumpCompletedEvent>(new EntityEventHandler<HyperspaceJumpCompletedEvent>(this.OnLocateTarget), null, null);
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x0004ACE8 File Offset: 0x00048EE8
		private void OnActivate(EntityUid uid, PinpointerComponent component, ActivateInWorldEvent args)
		{
			base.TogglePinpointer(uid, component);
			this.LocateTarget(uid, component);
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x0004ACFC File Offset: 0x00048EFC
		private void OnLocateTarget(HyperspaceJumpCompletedEvent ev)
		{
			foreach (PinpointerComponent pinpointer in base.EntityQuery<PinpointerComponent>(false))
			{
				this.LocateTarget(pinpointer.Owner, pinpointer);
			}
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x0004AD50 File Offset: 0x00048F50
		private void LocateTarget(EntityUid uid, PinpointerComponent component)
		{
			if (component.IsActive && component.Component != null)
			{
				ComponentRegistration reg;
				if (!this.EntityManager.ComponentFactory.TryGetRegistration(component.Component, ref reg, false))
				{
					Logger.Error("Unable to find component registration for " + component.Component + " for pinpointer!");
					return;
				}
				EntityUid? target = this.FindTargetFromComponent(uid, reg.Type, null);
				this.SetTarget(uid, target, component);
			}
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x0004ADBC File Offset: 0x00048FBC
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (PinpointerComponent pinpointer in base.EntityQuery<PinpointerComponent>(false))
			{
				this.UpdateDirectionToTarget(pinpointer.Owner, pinpointer);
			}
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x0004AE18 File Offset: 0x00049018
		private EntityUid? FindTargetFromComponent(EntityUid uid, Type whitelist, [Nullable(2)] TransformComponent transform = null)
		{
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			if (transform == null)
			{
				xformQuery.TryGetComponent(uid, ref transform);
			}
			if (transform == null)
			{
				return null;
			}
			MapId mapId = transform.MapID;
			SortedList<float, EntityUid> i = new SortedList<float, EntityUid>();
			Vector2 worldPos = this._transform.GetWorldPosition(transform, xformQuery);
			foreach (IComponent comp in this.EntityManager.GetAllComponents(whitelist, false))
			{
				TransformComponent compXform;
				if (xformQuery.TryGetComponent(comp.Owner, ref compXform) && !(compXform.MapID != mapId))
				{
					float dist = (this._transform.GetWorldPosition(compXform, xformQuery) - worldPos).LengthSquared;
					i.TryAdd(dist, comp.Owner);
				}
			}
			if (i.Count <= 0)
			{
				return null;
			}
			return new EntityUid?(i.First<KeyValuePair<float, EntityUid>>().Value);
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x0004AF24 File Offset: 0x00049124
		[NullableContext(2)]
		public void SetTarget(EntityUid uid, EntityUid? target, PinpointerComponent pinpointer = null)
		{
			if (!base.Resolve<PinpointerComponent>(uid, ref pinpointer, true))
			{
				return;
			}
			if (pinpointer.Target == target)
			{
				return;
			}
			pinpointer.Target = target;
			if (pinpointer.IsActive)
			{
				this.UpdateDirectionToTarget(uid, pinpointer);
			}
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x0004AF94 File Offset: 0x00049194
		[NullableContext(2)]
		private void UpdateDirectionToTarget(EntityUid uid, PinpointerComponent pinpointer = null)
		{
			if (!base.Resolve<PinpointerComponent>(uid, ref pinpointer, true))
			{
				return;
			}
			if (!pinpointer.IsActive)
			{
				return;
			}
			EntityUid? target = pinpointer.Target;
			if (target == null || !this.EntityManager.EntityExists(target.Value))
			{
				base.SetDistance(uid, Distance.Unknown, pinpointer);
				return;
			}
			Vector2? dirVec = this.CalculateDirection(uid, target.Value);
			if (dirVec != null)
			{
				Angle angle = DirectionExtensions.ToWorldAngle(dirVec.Value);
				base.TrySetArrowAngle(uid, angle, pinpointer);
				Distance dist = this.CalculateDistance(uid, dirVec.Value, pinpointer);
				base.SetDistance(uid, dist, pinpointer);
				return;
			}
			base.SetDistance(uid, Distance.Unknown, pinpointer);
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x0004B038 File Offset: 0x00049238
		private Vector2? CalculateDirection(EntityUid pinUid, EntityUid trgUid)
		{
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			TransformComponent pin;
			if (!xformQuery.TryGetComponent(pinUid, ref pin))
			{
				return null;
			}
			TransformComponent trg;
			if (!xformQuery.TryGetComponent(trgUid, ref trg))
			{
				return null;
			}
			if (pin.MapID != trg.MapID)
			{
				return null;
			}
			return new Vector2?(this._transform.GetWorldPosition(trg, xformQuery) - this._transform.GetWorldPosition(pin, xformQuery));
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x0004B0BC File Offset: 0x000492BC
		[NullableContext(2)]
		private Distance CalculateDistance(EntityUid uid, Vector2 vec, PinpointerComponent pinpointer = null)
		{
			if (!base.Resolve<PinpointerComponent>(uid, ref pinpointer, true))
			{
				return Distance.Unknown;
			}
			float dist = vec.Length;
			if (dist <= pinpointer.ReachedDistance)
			{
				return Distance.Reached;
			}
			if (dist <= pinpointer.CloseDistance)
			{
				return Distance.Close;
			}
			if (dist <= pinpointer.MediumDistance)
			{
				return Distance.Medium;
			}
			return Distance.Far;
		}

		// Token: 0x040008AE RID: 2222
		[Dependency]
		private readonly SharedTransformSystem _transform;
	}
}
