using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Construction.Components;
using Content.Shared.Construction;
using Content.Shared.Construction.Components;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Construction.Conditions
{
	// Token: 0x020005FF RID: 1535
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class MachineFrameComplete : IGraphCondition
	{
		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x060020F8 RID: 8440 RVA: 0x000AD239 File Offset: 0x000AB439
		[DataField("guideIconBoard", false, 1, false, false, null)]
		public SpriteSpecifier GuideIconBoard { get; }

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x060020F9 RID: 8441 RVA: 0x000AD241 File Offset: 0x000AB441
		[DataField("guideIconParts", false, 1, false, false, null)]
		public SpriteSpecifier GuideIconPart { get; }

		// Token: 0x060020FA RID: 8442 RVA: 0x000AD24C File Offset: 0x000AB44C
		[NullableContext(1)]
		public bool Condition(EntityUid uid, IEntityManager entityManager)
		{
			MachineFrameComponent machineFrame;
			return entityManager.TryGetComponent<MachineFrameComponent>(uid, ref machineFrame) && entityManager.EntitySysManager.GetEntitySystem<MachineFrameSystem>().IsComplete(machineFrame);
		}

		// Token: 0x060020FB RID: 8443 RVA: 0x000AD278 File Offset: 0x000AB478
		[NullableContext(1)]
		public bool DoExamine(ExaminedEvent args)
		{
			EntityUid entity = args.Examined;
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			MachineFrameComponent machineFrame;
			if (!entityManager.TryGetComponent<MachineFrameComponent>(entity, ref machineFrame))
			{
				return false;
			}
			if (!machineFrame.HasBoard)
			{
				args.PushMarkup(Loc.GetString("construction-condition-machine-frame-insert-circuit-board-message"));
				return true;
			}
			if (entityManager.EntitySysManager.GetEntitySystem<MachineFrameSystem>().IsComplete(machineFrame))
			{
				return false;
			}
			args.Message.AddMarkup(Loc.GetString("construction-condition-machine-frame-requirement-label") + "\n");
			foreach (KeyValuePair<string, int> keyValuePair in machineFrame.Requirements)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string part = text;
				int amount = num - machineFrame.Progress[part];
				if (amount != 0)
				{
					args.Message.AddMarkup(Loc.GetString("construction-condition-machine-frame-required-element-entry", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("amount", amount),
						new ValueTuple<string, object>("elementName", Loc.GetString(part))
					}) + "\n");
				}
			}
			foreach (KeyValuePair<string, int> keyValuePair in machineFrame.MaterialRequirements)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string material = text;
				int amount2 = num - machineFrame.MaterialProgress[material];
				if (amount2 != 0)
				{
					args.Message.AddMarkup(Loc.GetString("construction-condition-machine-frame-required-element-entry", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("amount", amount2),
						new ValueTuple<string, object>("elementName", Loc.GetString(material))
					}) + "\n");
				}
			}
			foreach (KeyValuePair<string, GenericPartInfo> keyValuePair2 in machineFrame.ComponentRequirements)
			{
				string text;
				GenericPartInfo genericPartInfo;
				keyValuePair2.Deconstruct(out text, out genericPartInfo);
				string compName = text;
				GenericPartInfo info = genericPartInfo;
				if (info.Amount - machineFrame.ComponentProgress[compName] != 0)
				{
					args.Message.AddMarkup(Loc.GetString("construction-condition-machine-frame-required-element-entry", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("amount", info.Amount),
						new ValueTuple<string, object>("elementName", Loc.GetString(info.ExamineName))
					}) + "\n");
				}
			}
			foreach (KeyValuePair<string, GenericPartInfo> keyValuePair2 in machineFrame.TagRequirements)
			{
				string text;
				GenericPartInfo genericPartInfo;
				keyValuePair2.Deconstruct(out text, out genericPartInfo);
				string tagName = text;
				GenericPartInfo info2 = genericPartInfo;
				if (info2.Amount - machineFrame.TagProgress[tagName] != 0)
				{
					args.Message.AddMarkup(Loc.GetString("construction-condition-machine-frame-required-element-entry", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("amount", info2.Amount),
						new ValueTuple<string, object>("elementName", Loc.GetString(info2.ExamineName))
					}) + "\n");
				}
			}
			return true;
		}

		// Token: 0x060020FC RID: 8444 RVA: 0x000AD604 File Offset: 0x000AB804
		[NullableContext(1)]
		public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
		{
			yield return new ConstructionGuideEntry
			{
				Localization = "construction-step-condition-machine-frame-board",
				Icon = this.GuideIconBoard,
				EntryNumber = new int?(0)
			};
			yield return new ConstructionGuideEntry
			{
				Localization = "construction-step-condition-machine-frame-parts",
				Icon = this.GuideIconPart,
				EntryNumber = new int?(0)
			};
			yield break;
		}
	}
}
