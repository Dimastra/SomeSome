using System;
using System.Runtime.CompilerServices;
using Content.Server.Nutrition.Components;
using Content.Server.Popups;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x02000316 RID: 790
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class UtensilSystem : EntitySystem
	{
		// Token: 0x0600105B RID: 4187 RVA: 0x00054A6A File Offset: 0x00052C6A
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<UtensilComponent, AfterInteractEvent>(new ComponentEventHandler<UtensilComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x00054A88 File Offset: 0x00052C88
		private void OnAfterInteract(EntityUid uid, UtensilComponent component, AfterInteractEvent ev)
		{
			if (ev.Target == null || !ev.CanReach)
			{
				return;
			}
			if (this.TryUseUtensil(ev.User, ev.Target.Value, component))
			{
				ev.Handled = true;
			}
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x00054AD4 File Offset: 0x00052CD4
		private bool TryUseUtensil(EntityUid user, EntityUid target, UtensilComponent component)
		{
			FoodComponent food;
			if (!this.EntityManager.TryGetComponent<FoodComponent>(target, ref food))
			{
				return false;
			}
			if ((food.Utensil & component.Types) == UtensilType.None)
			{
				this._popupSystem.PopupEntity(Loc.GetString("food-system-wrong-utensil", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("food", target),
					new ValueTuple<string, object>("utensil", component.Owner)
				}), user, user, PopupType.Small);
				return false;
			}
			return this._interactionSystem.InRangeUnobstructed(user, target, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, true) && this._foodSystem.TryFeed(user, user, target, food);
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x00054B84 File Offset: 0x00052D84
		[NullableContext(2)]
		public void TryBreak(EntityUid uid, EntityUid userUid, UtensilComponent component = null)
		{
			if (!base.Resolve<UtensilComponent>(uid, ref component, true))
			{
				return;
			}
			if (RandomExtensions.Prob(this._robustRandom, component.BreakChance))
			{
				SoundSystem.Play(component.BreakSound.GetSound(null, null), Filter.Pvs(userUid, 2f, null, null, null), userUid, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
				this.EntityManager.DeleteEntity(component.Owner);
			}
		}

		// Token: 0x04000978 RID: 2424
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000979 RID: 2425
		[Dependency]
		private readonly FoodSystem _foodSystem;

		// Token: 0x0400097A RID: 2426
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400097B RID: 2427
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;
	}
}
