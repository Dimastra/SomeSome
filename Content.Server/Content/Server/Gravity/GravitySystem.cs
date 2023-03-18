using System;
using System.Runtime.CompilerServices;
using Content.Shared.Gravity;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;

namespace Content.Server.Gravity
{
	// Token: 0x0200048A RID: 1162
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class GravitySystem : SharedGravitySystem
	{
		// Token: 0x0600174E RID: 5966 RVA: 0x0007A5C3 File Offset: 0x000787C3
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GravityComponent, ComponentInit>(new ComponentEventHandler<GravityComponent, ComponentInit>(this.OnGravityInit), null, null);
		}

		// Token: 0x0600174F RID: 5967 RVA: 0x0007A5E0 File Offset: 0x000787E0
		public void RefreshGravity(EntityUid uid, GravityComponent gravity = null)
		{
			if (!base.Resolve<GravityComponent>(uid, ref gravity, true))
			{
				return;
			}
			bool enabled = false;
			foreach (ValueTuple<GravityGeneratorComponent, TransformComponent> valueTuple in base.EntityQuery<GravityGeneratorComponent, TransformComponent>(true))
			{
				GravityGeneratorComponent comp = valueTuple.Item1;
				TransformComponent xform = valueTuple.Item2;
				if (comp.GravityActive && !(xform.ParentUid != uid))
				{
					enabled = true;
					break;
				}
			}
			if (enabled != gravity.Enabled)
			{
				gravity.Enabled = enabled;
				GravityChangedEvent ev = new GravityChangedEvent(uid, enabled);
				base.RaiseLocalEvent<GravityChangedEvent>(uid, ref ev, true);
				base.Dirty(gravity, null);
				if (base.HasComp<MapGridComponent>(uid))
				{
					base.StartGridShake(uid, null);
				}
			}
		}

		// Token: 0x06001750 RID: 5968 RVA: 0x0007A698 File Offset: 0x00078898
		[NullableContext(1)]
		private void OnGravityInit(EntityUid uid, GravityComponent component, ComponentInit args)
		{
			this.RefreshGravity(uid, null);
		}

		// Token: 0x06001751 RID: 5969 RVA: 0x0007A6A4 File Offset: 0x000788A4
		public void EnableGravity(EntityUid uid, GravityComponent gravity = null)
		{
			if (!base.Resolve<GravityComponent>(uid, ref gravity, true))
			{
				return;
			}
			if (gravity.Enabled)
			{
				return;
			}
			gravity.Enabled = true;
			GravityChangedEvent ev = new GravityChangedEvent(uid, true);
			base.RaiseLocalEvent<GravityChangedEvent>(uid, ref ev, true);
			base.Dirty(gravity, null);
			if (base.HasComp<MapGridComponent>(uid))
			{
				base.StartGridShake(uid, null);
			}
		}
	}
}
