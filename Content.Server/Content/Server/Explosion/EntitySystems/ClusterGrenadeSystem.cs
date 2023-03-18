using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Explosion.Components;
using Content.Server.Flash.Components;
using Content.Shared.Explosion;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Throwing;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Explosion.EntitySystems
{
	// Token: 0x02000509 RID: 1289
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClusterGrenadeSystem : EntitySystem
	{
		// Token: 0x06001A94 RID: 6804 RVA: 0x0008C148 File Offset: 0x0008A348
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ClusterGrenadeComponent, ComponentInit>(new ComponentEventHandler<ClusterGrenadeComponent, ComponentInit>(this.OnClugInit), null, null);
			base.SubscribeLocalEvent<ClusterGrenadeComponent, ComponentStartup>(new ComponentEventHandler<ClusterGrenadeComponent, ComponentStartup>(this.OnClugStartup), null, null);
			base.SubscribeLocalEvent<ClusterGrenadeComponent, InteractUsingEvent>(new ComponentEventHandler<ClusterGrenadeComponent, InteractUsingEvent>(this.OnClugUsing), null, null);
			base.SubscribeLocalEvent<ClusterGrenadeComponent, UseInHandEvent>(new ComponentEventHandler<ClusterGrenadeComponent, UseInHandEvent>(this.OnClugUse), null, null);
		}

		// Token: 0x06001A95 RID: 6805 RVA: 0x0008C1AB File Offset: 0x0008A3AB
		private void OnClugInit(EntityUid uid, ClusterGrenadeComponent component, ComponentInit args)
		{
			component.GrenadesContainer = this._container.EnsureContainer<Container>(uid, "cluster-flash", null);
		}

		// Token: 0x06001A96 RID: 6806 RVA: 0x0008C1C5 File Offset: 0x0008A3C5
		private void OnClugStartup(EntityUid uid, ClusterGrenadeComponent component, ComponentStartup args)
		{
			if (component.FillPrototype != null)
			{
				component.UnspawnedCount = Math.Max(0, component.MaxGrenades - component.GrenadesContainer.ContainedEntities.Count);
				this.UpdateAppearance(uid, component);
			}
		}

		// Token: 0x06001A97 RID: 6807 RVA: 0x0008C1FC File Offset: 0x0008A3FC
		private void OnClugUsing(EntityUid uid, ClusterGrenadeComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (component.GrenadesContainer.ContainedEntities.Count >= component.MaxGrenades || !base.HasComp<FlashOnTriggerComponent>(args.Used))
			{
				return;
			}
			component.GrenadesContainer.Insert(args.Used, null, null, null, null, null);
			this.UpdateAppearance(uid, component);
			args.Handled = true;
		}

		// Token: 0x06001A98 RID: 6808 RVA: 0x0008C260 File Offset: 0x0008A460
		private void OnClugUse(EntityUid uid, ClusterGrenadeComponent component, UseInHandEvent args)
		{
			if (component.CountDown || component.GrenadesContainer.ContainedEntities.Count + component.UnspawnedCount <= 0)
			{
				return;
			}
			TimerExtensions.SpawnTimer(uid, (int)(component.Delay * 1000f), delegate()
			{
				if (this.Deleted(component.Owner, null))
				{
					return;
				}
				component.CountDown = true;
				int delay = 20;
				int grenadesInserted = component.GrenadesContainer.ContainedEntities.Count + component.UnspawnedCount;
				int thrownCount = 0;
				int segmentAngle = 360 / grenadesInserted;
				for (;;)
				{
					EntityUid grenade;
					if (!this.TryGetGrenade(component, out grenade))
					{
						break;
					}
					int angleMin = segmentAngle * thrownCount;
					int angleMax = segmentAngle * (thrownCount + 1);
					Angle angle = Angle.FromDegrees((double)this._random.Next(angleMin, angleMax));
					delay += this._random.Next(550, 900);
					thrownCount++;
					this._throwingSystem.TryThrow(grenade, angle.ToVec().Normalized * component.ThrowDistance, 1f, null, 5f, null, null, null, null);
					TimerExtensions.SpawnTimer(grenade, delay, delegate()
					{
						if (((!this.EntityManager.EntityExists(grenade)) ? 5 : this.MetaData(grenade).EntityLifeStage) >= 5)
						{
							return;
						}
						this._trigger.Trigger(grenade, new EntityUid?(args.User));
					}, default(CancellationToken));
				}
				this.EntityManager.DeleteEntity(uid);
			}, default(CancellationToken));
			args.Handled = true;
		}

		// Token: 0x06001A99 RID: 6809 RVA: 0x0008C300 File Offset: 0x0008A500
		private bool TryGetGrenade(ClusterGrenadeComponent component, out EntityUid grenade)
		{
			grenade = default(EntityUid);
			if (component.UnspawnedCount > 0)
			{
				component.UnspawnedCount--;
				grenade = this.EntityManager.SpawnEntity(component.FillPrototype, base.Transform(component.Owner).MapPosition);
				return true;
			}
			if (component.GrenadesContainer.ContainedEntities.Count > 0)
			{
				grenade = component.GrenadesContainer.ContainedEntities[0];
				return component.GrenadesContainer.Remove(grenade, null, null, null, true, false, null, null);
			}
			return false;
		}

		// Token: 0x06001A9A RID: 6810 RVA: 0x0008C3B0 File Offset: 0x0008A5B0
		private void UpdateAppearance(EntityUid uid, ClusterGrenadeComponent component)
		{
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(component.Owner, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, ClusterGrenadeVisuals.GrenadesCounter, component.GrenadesContainer.ContainedEntities.Count + component.UnspawnedCount, appearance);
		}

		// Token: 0x040010E8 RID: 4328
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040010E9 RID: 4329
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x040010EA RID: 4330
		[Dependency]
		private readonly TriggerSystem _trigger;

		// Token: 0x040010EB RID: 4331
		[Dependency]
		private readonly ThrowingSystem _throwingSystem;

		// Token: 0x040010EC RID: 4332
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
