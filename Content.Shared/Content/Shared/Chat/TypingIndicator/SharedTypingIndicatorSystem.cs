using System;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chat.TypingIndicator
{
	// Token: 0x02000604 RID: 1540
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedTypingIndicatorSystem : EntitySystem
	{
		// Token: 0x060012E4 RID: 4836 RVA: 0x0003E024 File Offset: 0x0003C224
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TypingIndicatorClothingComponent, GotEquippedEvent>(new ComponentEventHandler<TypingIndicatorClothingComponent, GotEquippedEvent>(this.OnGotEquipped), null, null);
			base.SubscribeLocalEvent<TypingIndicatorClothingComponent, GotUnequippedEvent>(new ComponentEventHandler<TypingIndicatorClothingComponent, GotUnequippedEvent>(this.OnGotUnequipped), null, null);
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x0003E054 File Offset: 0x0003C254
		private void OnGotEquipped(EntityUid uid, TypingIndicatorClothingComponent component, GotEquippedEvent args)
		{
			ClothingComponent clothing;
			TypingIndicatorComponent indicator;
			if (!base.TryComp<ClothingComponent>(uid, ref clothing) || !base.TryComp<TypingIndicatorComponent>(args.Equipee, ref indicator))
			{
				return;
			}
			if (!clothing.Slots.HasFlag(args.SlotFlags))
			{
				return;
			}
			indicator.Prototype = component.Prototype;
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x0003E0A8 File Offset: 0x0003C2A8
		private void OnGotUnequipped(EntityUid uid, TypingIndicatorClothingComponent component, GotUnequippedEvent args)
		{
			TypingIndicatorComponent indicator;
			if (!base.TryComp<TypingIndicatorComponent>(args.Equipee, ref indicator))
			{
				return;
			}
			indicator.Prototype = "default";
		}

		// Token: 0x0400119C RID: 4508
		public const string InitialIndicatorId = "default";
	}
}
