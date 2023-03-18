using System;
using System.Runtime.CompilerServices;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Power.Components;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.PowerSink
{
	// Token: 0x02000272 RID: 626
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PowerSinkSystem : EntitySystem
	{
		// Token: 0x06000C80 RID: 3200 RVA: 0x000414FF File Offset: 0x0003F6FF
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PowerSinkComponent, ExaminedEvent>(new ComponentEventHandler<PowerSinkComponent, ExaminedEvent>(this.OnExamine), null, null);
		}

		// Token: 0x06000C81 RID: 3201 RVA: 0x0004151C File Offset: 0x0003F71C
		private void OnExamine(EntityUid uid, PowerSinkComponent component, ExaminedEvent args)
		{
			PowerConsumerComponent consumer;
			if (!args.IsInDetailsRange || !base.TryComp<PowerConsumerComponent>(uid, ref consumer))
			{
				return;
			}
			int drainAmount = (int)consumer.NetworkLoad.ReceivingPower / 1000;
			args.PushMarkup(Loc.GetString("powersink-examine-drain-amount", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("amount", drainAmount),
				new ValueTuple<string, object>("markupDrainColor", "orange")
			}));
		}

		// Token: 0x06000C82 RID: 3202 RVA: 0x00041594 File Offset: 0x0003F794
		public override void Update(float frameTime)
		{
			RemQueue<ValueTuple<PowerSinkComponent, BatteryComponent>> toRemove = default(RemQueue<ValueTuple<PowerSinkComponent, BatteryComponent>>);
			foreach (ValueTuple<PowerSinkComponent, PowerConsumerComponent, BatteryComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<PowerSinkComponent, PowerConsumerComponent, BatteryComponent, TransformComponent>(false))
			{
				PowerSinkComponent comp = valueTuple.Item1;
				PowerConsumerComponent networkLoad = valueTuple.Item2;
				BatteryComponent battery = valueTuple.Item3;
				if (valueTuple.Item4.Anchored)
				{
					battery.CurrentCharge += networkLoad.NetworkLoad.ReceivingPower / 1000f;
					if (battery.CurrentCharge >= battery.MaxCharge)
					{
						toRemove.Add(new ValueTuple<PowerSinkComponent, BatteryComponent>(comp, battery));
					}
				}
			}
			foreach (ValueTuple<PowerSinkComponent, BatteryComponent> valueTuple2 in toRemove)
			{
				PowerSinkComponent comp2 = valueTuple2.Item1;
				BatteryComponent battery2 = valueTuple2.Item2;
				this._explosionSystem.QueueExplosion(comp2.Owner, "Default", 5f * (battery2.MaxCharge / 2500000f), 0.5f, 10f, 1f, int.MaxValue, false, false, null, true);
				this.EntityManager.RemoveComponent(comp2.Owner, comp2);
			}
		}

		// Token: 0x040007B5 RID: 1973
		[Dependency]
		private readonly ExplosionSystem _explosionSystem;
	}
}
