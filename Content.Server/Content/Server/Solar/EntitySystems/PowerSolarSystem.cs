using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.Solar.Components;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;

namespace Content.Server.Solar.EntitySystems
{
	// Token: 0x020001DF RID: 479
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class PowerSolarSystem : EntitySystem
	{
		// Token: 0x06000917 RID: 2327 RVA: 0x0002DCCD File Offset: 0x0002BECD
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SolarPanelComponent, MapInitEvent>(new ComponentEventHandler<SolarPanelComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			this.RandomizeSun();
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x0002DCFD File Offset: 0x0002BEFD
		public void Reset(RoundRestartCleanupEvent ev)
		{
			this.RandomizeSun();
			this.TargetPanelRotation = Angle.Zero;
			this.TargetPanelVelocity = Angle.Zero;
			this.TotalPanelPower = 0f;
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x0002DD28 File Offset: 0x0002BF28
		private void RandomizeSun()
		{
			this.TowardsSun = 6.2831854820251465 * this._robustRandom.NextDouble();
			this.SunAngularVelocity = Angle.FromDegrees(0.1 + (this._robustRandom.NextDouble() - 0.5) * 0.05);
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x0002DD89 File Offset: 0x0002BF89
		private void OnMapInit(EntityUid uid, SolarPanelComponent component, MapInitEvent args)
		{
			this.UpdateSupply(uid, component, null);
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x0002DD94 File Offset: 0x0002BF94
		public override void Update(float frameTime)
		{
			this.TowardsSun += this.SunAngularVelocity * (double)frameTime;
			this.TowardsSun = this.TowardsSun.Reduced();
			this.TargetPanelRotation += this.TargetPanelVelocity * (double)frameTime;
			this.TargetPanelRotation = this.TargetPanelRotation.Reduced();
			if (this._updateQueue.Count > 0)
			{
				SolarPanelComponent panel = this._updateQueue.Dequeue();
				if (panel.Running)
				{
					this.UpdatePanelCoverage(panel);
					return;
				}
			}
			else
			{
				this.TotalPanelPower = 0f;
				foreach (ValueTuple<SolarPanelComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<SolarPanelComponent, TransformComponent>(false))
				{
					SolarPanelComponent panel2 = valueTuple.Item1;
					TransformComponent item = valueTuple.Item2;
					this.TotalPanelPower += (float)panel2.MaxSupply * panel2.Coverage;
					item.WorldRotation = this.TargetPanelRotation;
					this._updateQueue.Enqueue(panel2);
				}
			}
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x0002DEBC File Offset: 0x0002C0BC
		private void UpdatePanelCoverage(SolarPanelComponent panel)
		{
			EntityUid entity = panel.Owner;
			TransformComponent xform = this.EntityManager.GetComponent<TransformComponent>(entity);
			Angle panelRelativeToSun = xform.WorldRotation - this.TowardsSun;
			float coverage = (float)Math.Max(0.0, Math.Cos(panelRelativeToSun));
			if (coverage > 0f)
			{
				CollisionRay ray;
				ray..ctor(xform.WorldPosition, this.TowardsSun.ToWorldVec(), 1);
				if (this._physicsSystem.IntersectRayWithPredicate(xform.MapID, ray, this.SunOcclusionCheckDistance, (EntityUid e) => !xform.Anchored || e == entity, true).Any<RayCastResults>())
				{
					coverage = 0f;
				}
			}
			panel.Coverage = coverage;
			this.UpdateSupply(panel.Owner, panel, null);
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x0002DF96 File Offset: 0x0002C196
		[NullableContext(2)]
		public void UpdateSupply(EntityUid uid, SolarPanelComponent solar = null, PowerSupplierComponent supplier = null)
		{
			if (!base.Resolve<SolarPanelComponent, PowerSupplierComponent>(uid, ref solar, ref supplier, true))
			{
				return;
			}
			supplier.MaxSupply = (float)((int)((float)solar.MaxSupply * solar.Coverage));
		}

		// Token: 0x04000581 RID: 1409
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000582 RID: 1410
		[Dependency]
		private readonly SharedPhysicsSystem _physicsSystem;

		// Token: 0x04000583 RID: 1411
		public const float MaxPanelVelocityDegrees = 1f;

		// Token: 0x04000584 RID: 1412
		public Angle TowardsSun = Angle.Zero;

		// Token: 0x04000585 RID: 1413
		public Angle SunAngularVelocity = Angle.Zero;

		// Token: 0x04000586 RID: 1414
		public float SunOcclusionCheckDistance = 20f;

		// Token: 0x04000587 RID: 1415
		public Angle TargetPanelRotation = Angle.Zero;

		// Token: 0x04000588 RID: 1416
		public Angle TargetPanelVelocity = Angle.Zero;

		// Token: 0x04000589 RID: 1417
		public float TotalPanelPower;

		// Token: 0x0400058A RID: 1418
		private readonly Queue<SolarPanelComponent> _updateQueue = new Queue<SolarPanelComponent>();
	}
}
