using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Components;
using Content.Shared.Examine;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Shared.Construction
{
	// Token: 0x0200056C RID: 1388
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MachinePartSystem : EntitySystem
	{
		// Token: 0x060010F2 RID: 4338 RVA: 0x00037CB1 File Offset: 0x00035EB1
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MachineBoardComponent, ExaminedEvent>(new ComponentEventHandler<MachineBoardComponent, ExaminedEvent>(this.OnMachineBoardExamined), null, null);
			base.SubscribeLocalEvent<MachinePartComponent, ExaminedEvent>(new ComponentEventHandler<MachinePartComponent, ExaminedEvent>(this.OnMachinePartExamined), null, null);
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x00037CE4 File Offset: 0x00035EE4
		private void OnMachineBoardExamined(EntityUid uid, MachineBoardComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			args.PushMarkup(Loc.GetString("machine-board-component-on-examine-label"));
			foreach (KeyValuePair<string, int> keyValuePair in component.Requirements)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string part = text;
				int amount = num;
				args.PushMarkup(Loc.GetString("machine-board-component-required-element-entry-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("amount", amount),
					new ValueTuple<string, object>("requiredElement", Loc.GetString(part))
				}));
			}
			foreach (KeyValuePair<StackPrototype, int> keyValuePair2 in component.MaterialRequirements)
			{
				int num;
				StackPrototype stackPrototype;
				keyValuePair2.Deconstruct(out stackPrototype, out num);
				StackPrototype material = stackPrototype;
				int amount2 = num;
				args.PushMarkup(Loc.GetString("machine-board-component-required-element-entry-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("amount", amount2),
					new ValueTuple<string, object>("requiredElement", Loc.GetString(material.Name))
				}));
			}
			foreach (KeyValuePair<string, GenericPartInfo> keyValuePair3 in component.ComponentRequirements)
			{
				string text;
				GenericPartInfo genericPartInfo;
				keyValuePair3.Deconstruct(out text, out genericPartInfo);
				GenericPartInfo info = genericPartInfo;
				args.PushMarkup(Loc.GetString("machine-board-component-required-element-entry-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("amount", info.Amount),
					new ValueTuple<string, object>("requiredElement", Loc.GetString(info.ExamineName))
				}));
			}
			foreach (KeyValuePair<string, GenericPartInfo> keyValuePair3 in component.TagRequirements)
			{
				string text;
				GenericPartInfo genericPartInfo;
				keyValuePair3.Deconstruct(out text, out genericPartInfo);
				GenericPartInfo info2 = genericPartInfo;
				args.PushMarkup(Loc.GetString("machine-board-component-required-element-entry-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("amount", info2.Amount),
					new ValueTuple<string, object>("requiredElement", Loc.GetString(info2.ExamineName))
				}));
			}
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x00037F70 File Offset: 0x00036170
		private void OnMachinePartExamined(EntityUid uid, MachinePartComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			args.PushMarkup(Loc.GetString("machine-part-component-on-examine-rating-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("rating", component.Rating)
			}));
			args.PushMarkup(Loc.GetString("machine-part-component-on-examine-type-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("type", component.PartType)
			}));
		}
	}
}
