using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Server.ParticleAccelerator.Components;
using Content.Server.Singularity.Components;
using Content.Shared.Coordinates;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Physics.Events;
using Robust.Shared.ViewVariables;

namespace Content.Server.Singularity.EntitySystems
{
	// Token: 0x020001ED RID: 493
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SingularityGeneratorSystem : EntitySystem
	{
		// Token: 0x06000989 RID: 2441 RVA: 0x00030630 File Offset: 0x0002E830
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ParticleProjectileComponent, StartCollideEvent>(new ComponentEventRefHandler<ParticleProjectileComponent, StartCollideEvent>(this.HandleParticleCollide), null, null);
			ViewVariablesTypeHandler<SingularityGeneratorComponent> typeHandler = this._vvm.GetTypeHandler<SingularityGeneratorComponent>();
			typeHandler.AddPath<float>("Power", (EntityUid _, SingularityGeneratorComponent comp) => comp.Power, new ComponentPropertySetter<SingularityGeneratorComponent, float>(this.SetPower));
			typeHandler.AddPath<float>("Threshold", (EntityUid _, SingularityGeneratorComponent comp) => comp.Threshold, new ComponentPropertySetter<SingularityGeneratorComponent, float>(this.SetThreshold));
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x000306CF File Offset: 0x0002E8CF
		public override void Shutdown()
		{
			ViewVariablesTypeHandler<SingularityGeneratorComponent> typeHandler = this._vvm.GetTypeHandler<SingularityGeneratorComponent>();
			typeHandler.RemovePath("Power");
			typeHandler.RemovePath("Threshold");
			base.Shutdown();
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x000306FC File Offset: 0x0002E8FC
		[NullableContext(2)]
		private void OnPassThreshold(EntityUid uid, SingularityGeneratorComponent comp)
		{
			if (!base.Resolve<SingularityGeneratorComponent>(uid, ref comp, true))
			{
				return;
			}
			this.SetPower(comp, 0f);
			IEnumerable<ContainmentFieldComponent> fieldComp = this._entityManager.EntityQuery<ContainmentFieldComponent>(false);
			if (!fieldComp.Any<ContainmentFieldComponent>())
			{
				this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-singularity-no-fields", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("singularity", base.ToPrettyString(uid))
				}));
			}
			using (IEnumerator<ContainmentFieldComponent> enumerator = fieldComp.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Owner.ToCoordinates().InRange(this._entityManager, uid.ToCoordinates(), (float)this._checkFieldRange))
					{
						this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-singularity-no-fields", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("singularity", base.ToPrettyString(uid))
						}));
						break;
					}
				}
			}
			this.EntityManager.SpawnEntity(comp.SpawnPrototype, base.Transform(comp.Owner).Coordinates);
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x00030828 File Offset: 0x0002EA28
		public void SetPower(SingularityGeneratorComponent comp, float value)
		{
			float oldValue = comp.Power;
			if (value == oldValue)
			{
				return;
			}
			comp.Power = value;
			if (comp.Power >= comp.Threshold && oldValue < comp.Threshold)
			{
				this.OnPassThreshold(comp.Owner, comp);
			}
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x0003086C File Offset: 0x0002EA6C
		public void SetThreshold(SingularityGeneratorComponent comp, float value)
		{
			float oldValue = comp.Threshold;
			if (value == comp.Threshold)
			{
				return;
			}
			comp.Power = value;
			if (comp.Power >= comp.Threshold && comp.Power < oldValue)
			{
				this.OnPassThreshold(comp.Owner, comp);
			}
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x000308B5 File Offset: 0x0002EAB5
		[NullableContext(2)]
		public void SetPower(EntityUid uid, float value, SingularityGeneratorComponent comp)
		{
			if (!base.Resolve<SingularityGeneratorComponent>(uid, ref comp, true))
			{
				return;
			}
			this.SetPower(comp, value);
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x000308CC File Offset: 0x0002EACC
		[NullableContext(2)]
		public void SetThreshold(EntityUid uid, float value, SingularityGeneratorComponent comp)
		{
			if (!base.Resolve<SingularityGeneratorComponent>(uid, ref comp, true))
			{
				return;
			}
			this.SetThreshold(comp, value);
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x000308E4 File Offset: 0x0002EAE4
		private void HandleParticleCollide(EntityUid uid, ParticleProjectileComponent component, ref StartCollideEvent args)
		{
			SingularityGeneratorComponent singularityGeneratorComponent;
			if (this.EntityManager.TryGetComponent<SingularityGeneratorComponent>(args.OtherFixture.Body.Owner, ref singularityGeneratorComponent))
			{
				SingularityGeneratorComponent comp = singularityGeneratorComponent;
				float power = singularityGeneratorComponent.Power;
				int num;
				switch (component.State)
				{
				case ParticleAcceleratorPowerState.Standby:
					num = 0;
					break;
				case ParticleAcceleratorPowerState.Level0:
					num = 1;
					break;
				case ParticleAcceleratorPowerState.Level1:
					num = 2;
					break;
				case ParticleAcceleratorPowerState.Level2:
					num = 4;
					break;
				case ParticleAcceleratorPowerState.Level3:
					num = 8;
					break;
				default:
					num = 0;
					break;
				}
				this.SetPower(comp, power + (float)num);
				this.EntityManager.QueueDeleteEntity(uid);
			}
		}

		// Token: 0x040005B8 RID: 1464
		[Dependency]
		private readonly IViewVariablesManager _vvm;

		// Token: 0x040005B9 RID: 1465
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x040005BA RID: 1466
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x040005BB RID: 1467
		private readonly int _checkFieldRange = 12;
	}
}
