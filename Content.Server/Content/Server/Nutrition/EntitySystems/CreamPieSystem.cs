using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Explosion.Components;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Fluids.EntitySystems;
using Content.Server.Nutrition.Components;
using Content.Server.Popups;
using Content.Shared.Chemistry.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Content.Shared.Throwing;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x0200030B RID: 779
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CreamPieSystem : SharedCreamPieSystem
	{
		// Token: 0x06001001 RID: 4097 RVA: 0x000514AA File Offset: 0x0004F6AA
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CreamPieComponent, InteractUsingEvent>(new ComponentEventHandler<CreamPieComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<CreamPiedComponent, RejuvenateEvent>(new ComponentEventHandler<CreamPiedComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
		}

		// Token: 0x06001002 RID: 4098 RVA: 0x000514DC File Offset: 0x0004F6DC
		protected override void SplattedCreamPie(EntityUid uid, CreamPieComponent creamPie)
		{
			this._audio.Play(this._audio.GetSound(creamPie.Sound), Filter.Pvs(uid, 2f, null, null, null), uid, false, new AudioParams?(new AudioParams().WithVariation(new float?(0.125f))));
			FoodComponent foodComp;
			if (this.EntityManager.TryGetComponent<FoodComponent>(uid, ref foodComp))
			{
				Solution solution;
				if (this._solutions.TryGetSolution(uid, foodComp.SolutionName, out solution, null))
				{
					this._spillable.SpillAt(uid, solution, "PuddleSmear", false, true, null);
				}
				if (!string.IsNullOrEmpty(foodComp.TrashPrototype))
				{
					this.EntityManager.SpawnEntity(foodComp.TrashPrototype, base.Transform(uid).Coordinates);
				}
			}
			this.ActivatePayload(uid);
			this.EntityManager.QueueDeleteEntity(uid);
		}

		// Token: 0x06001003 RID: 4099 RVA: 0x000515AD File Offset: 0x0004F7AD
		private void OnInteractUsing(EntityUid uid, CreamPieComponent component, InteractUsingEvent args)
		{
			this.ActivatePayload(uid);
		}

		// Token: 0x06001004 RID: 4100 RVA: 0x000515B8 File Offset: 0x0004F7B8
		private void ActivatePayload(EntityUid uid)
		{
			ItemSlot itemSlot;
			EntityUid? item;
			OnUseTimerTriggerComponent timerTrigger;
			if (this._itemSlots.TryGetSlot(uid, "payloadSlot", out itemSlot, null) && this._itemSlots.TryEject(uid, itemSlot, null, out item, false) && base.TryComp<OnUseTimerTriggerComponent>(item.Value, ref timerTrigger))
			{
				this._trigger.HandleTimerTrigger(item.Value, null, timerTrigger.Delay, timerTrigger.BeepInterval, timerTrigger.InitialBeepDelay, timerTrigger.BeepSound, timerTrigger.BeepParams);
			}
		}

		// Token: 0x06001005 RID: 4101 RVA: 0x00051640 File Offset: 0x0004F840
		protected override void CreamedEntity(EntityUid uid, CreamPiedComponent creamPied, ThrowHitByEvent args)
		{
			this._popup.PopupEntity(Loc.GetString("cream-pied-component-on-hit-by-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("thrower", args.Thrown)
			}), uid, args.Target, PopupType.Small);
			Filter otherPlayers = Filter.Empty().AddPlayersByPvs(uid, 2f, null, null, null);
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(args.Target, ref actor))
			{
				otherPlayers.RemovePlayer(actor.PlayerSession);
			}
			this._popup.PopupEntity(Loc.GetString("cream-pied-component-on-hit-by-message-others", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("owner", uid),
				new ValueTuple<string, object>("thrower", args.Thrown)
			}), uid, otherPlayers, false, PopupType.Small);
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x0005170F File Offset: 0x0004F90F
		private void OnRejuvenate(EntityUid uid, CreamPiedComponent component, RejuvenateEvent args)
		{
			base.SetCreamPied(uid, component, false);
		}

		// Token: 0x04000937 RID: 2359
		[Dependency]
		private readonly SolutionContainerSystem _solutions;

		// Token: 0x04000938 RID: 2360
		[Dependency]
		private readonly SpillableSystem _spillable;

		// Token: 0x04000939 RID: 2361
		[Dependency]
		private readonly ItemSlotsSystem _itemSlots;

		// Token: 0x0400093A RID: 2362
		[Dependency]
		private readonly TriggerSystem _trigger;

		// Token: 0x0400093B RID: 2363
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x0400093C RID: 2364
		[Dependency]
		private readonly PopupSystem _popup;
	}
}
