using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind.Components;
using Content.Server.Revenant.Components;
using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Revenant.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.Revenant.EntitySystems
{
	// Token: 0x02000231 RID: 561
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EssenceSystem : EntitySystem
	{
		// Token: 0x06000B27 RID: 2855 RVA: 0x0003A3A8 File Offset: 0x000385A8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EssenceComponent, ComponentStartup>(new ComponentEventHandler<EssenceComponent, ComponentStartup>(this.OnEssenceEventReceived), null, null);
			base.SubscribeLocalEvent<EssenceComponent, MobStateChangedEvent>(new ComponentEventHandler<EssenceComponent, MobStateChangedEvent>(this.OnMobstateChanged), null, null);
			base.SubscribeLocalEvent<EssenceComponent, MindAddedMessage>(new ComponentEventHandler<EssenceComponent, MindAddedMessage>(this.OnEssenceEventReceived), null, null);
			base.SubscribeLocalEvent<EssenceComponent, MindRemovedMessage>(new ComponentEventHandler<EssenceComponent, MindRemovedMessage>(this.OnEssenceEventReceived), null, null);
			base.SubscribeLocalEvent<EssenceComponent, ExaminedEvent>(new ComponentEventHandler<EssenceComponent, ExaminedEvent>(this.OnExamine), null, null);
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x0003A41F File Offset: 0x0003861F
		private void OnMobstateChanged(EntityUid uid, EssenceComponent component, MobStateChangedEvent args)
		{
			this.UpdateEssenceAmount(uid, component);
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x0003A42C File Offset: 0x0003862C
		private void OnExamine(EntityUid uid, EssenceComponent component, ExaminedEvent args)
		{
			if (!component.SearchComplete || !base.HasComp<RevenantComponent>(args.Examiner))
			{
				return;
			}
			float essenceAmount = component.EssenceAmount;
			string message;
			if (essenceAmount > 45f)
			{
				if (essenceAmount < 90f)
				{
					message = "revenant-soul-yield-average";
				}
				else
				{
					message = "revenant-soul-yield-high";
				}
			}
			else
			{
				message = "revenant-soul-yield-low";
			}
			args.PushMarkup(Loc.GetString(message, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", uid)
			}));
		}

		// Token: 0x06000B2A RID: 2858 RVA: 0x0003A4A8 File Offset: 0x000386A8
		private void OnEssenceEventReceived(EntityUid uid, EssenceComponent component, EntityEventArgs args)
		{
			this.UpdateEssenceAmount(uid, component);
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x0003A4B4 File Offset: 0x000386B4
		private void UpdateEssenceAmount(EntityUid uid, EssenceComponent component)
		{
			MobStateComponent mob;
			if (!base.TryComp<MobStateComponent>(uid, ref mob))
			{
				return;
			}
			switch (mob.CurrentState)
			{
			case MobState.Alive:
			{
				MindComponent mind;
				if (base.TryComp<MindComponent>(uid, ref mind) && mind.Mind != null)
				{
					component.EssenceAmount = this._random.NextFloat(75f, 100f);
					return;
				}
				component.EssenceAmount = this._random.NextFloat(45f, 70f);
				return;
			}
			case MobState.Critical:
				component.EssenceAmount = this._random.NextFloat(35f, 50f);
				return;
			case MobState.Dead:
				component.EssenceAmount = this._random.NextFloat(15f, 20f);
				return;
			default:
				return;
			}
		}

		// Token: 0x040006D1 RID: 1745
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
