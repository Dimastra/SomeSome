using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Disease.Effects
{
	// Token: 0x0200056C RID: 1388
	public sealed class DiseasePopUp : DiseaseEffect
	{
		// Token: 0x06001D55 RID: 7509 RVA: 0x0009C6B0 File Offset: 0x0009A8B0
		public override void Effect(DiseaseEffectArgs args)
		{
			SharedPopupSystem popupSys = args.EntityManager.EntitySysManager.GetEntitySystem<SharedPopupSystem>();
			if (this.Type == PopupRecipients.Local)
			{
				popupSys.PopupEntity(Loc.GetString(this.Message), args.DiseasedEntity, args.DiseasedEntity, this.VisualType);
				return;
			}
			if (this.Type == PopupRecipients.Pvs)
			{
				popupSys.PopupEntity(Loc.GetString(this.Message, new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("person", Identity.Entity(args.DiseasedEntity, args.EntityManager))
				}), args.DiseasedEntity, this.VisualType);
			}
		}

		// Token: 0x040012C5 RID: 4805
		[Nullable(1)]
		[DataField("message", false, 1, false, false, null)]
		public string Message = "disease-sick-generic";

		// Token: 0x040012C6 RID: 4806
		[DataField("type", false, 1, false, false, null)]
		public PopupRecipients Type = PopupRecipients.Local;

		// Token: 0x040012C7 RID: 4807
		[DataField("visualType", false, 1, false, false, null)]
		public PopupType VisualType;
	}
}
