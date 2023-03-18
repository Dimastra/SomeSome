using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Popups;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x0200066E RID: 1646
	public sealed class PopupMessage : ReagentEffect
	{
		// Token: 0x06002286 RID: 8838 RVA: 0x000B4350 File Offset: 0x000B2550
		public override void Effect(ReagentEffectArgs args)
		{
			SharedPopupSystem popupSys = args.EntityManager.EntitySysManager.GetEntitySystem<SharedPopupSystem>();
			string msg = RandomExtensions.Pick<string>(IoCManager.Resolve<IRobustRandom>(), this.Messages);
			ValueTuple<string, object>[] msgArgs = new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("entity", args.SolutionEntity),
				new ValueTuple<string, object>("organ", args.OrganEntity.GetValueOrDefault())
			};
			if (this.Type == PopupRecipients.Local)
			{
				popupSys.PopupEntity(Loc.GetString(msg, msgArgs), args.SolutionEntity, args.SolutionEntity, this.VisualType);
				return;
			}
			if (this.Type == PopupRecipients.Pvs)
			{
				popupSys.PopupEntity(Loc.GetString(msg, msgArgs), args.SolutionEntity, this.VisualType);
			}
		}

		// Token: 0x04001558 RID: 5464
		[Nullable(1)]
		[DataField("messages", false, 1, true, false, null)]
		public string[] Messages;

		// Token: 0x04001559 RID: 5465
		[DataField("type", false, 1, false, false, null)]
		public PopupRecipients Type = PopupRecipients.Local;

		// Token: 0x0400155A RID: 5466
		[DataField("visualType", false, 1, false, false, null)]
		public PopupType VisualType;
	}
}
