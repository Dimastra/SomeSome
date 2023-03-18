using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.StepTrigger.Components;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.StepTrigger.Systems
{
	// Token: 0x02000149 RID: 329
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShoesRequiredStepTriggerSystem : EntitySystem
	{
		// Token: 0x060003ED RID: 1005 RVA: 0x0000FBCC File Offset: 0x0000DDCC
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ShoesRequiredStepTriggerComponent, StepTriggerAttemptEvent>(new ComponentEventRefHandler<ShoesRequiredStepTriggerComponent, StepTriggerAttemptEvent>(this.OnStepTriggerAttempt), null, null);
			base.SubscribeLocalEvent<ShoesRequiredStepTriggerComponent, ExaminedEvent>(new ComponentEventHandler<ShoesRequiredStepTriggerComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x0000FBF8 File Offset: 0x0000DDF8
		private void OnStepTriggerAttempt(EntityUid uid, ShoesRequiredStepTriggerComponent component, ref StepTriggerAttemptEvent args)
		{
			if (this._tagSystem.HasTag(args.Tripper, "ShoesRequiredStepTriggerImmune"))
			{
				args.Cancelled = true;
				return;
			}
			InventoryComponent inventory;
			if (!base.TryComp<InventoryComponent>(args.Tripper, ref inventory))
			{
				return;
			}
			EntityUid? entityUid;
			if (this._inventory.TryGetSlotEntity(args.Tripper, "shoes", out entityUid, inventory, null))
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x0000FC59 File Offset: 0x0000DE59
		private void OnExamined(EntityUid uid, ShoesRequiredStepTriggerComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("shoes-required-step-trigger-examine"));
		}

		// Token: 0x040003D0 RID: 976
		[Dependency]
		private readonly InventorySystem _inventory;

		// Token: 0x040003D1 RID: 977
		[Dependency]
		private readonly TagSystem _tagSystem;
	}
}
