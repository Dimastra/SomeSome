using System;
using System.Runtime.CompilerServices;
using Content.Server.Access.Components;
using Content.Shared.Access.Components;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Access.Systems
{
	// Token: 0x0200087D RID: 2173
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class IdExaminableSystem : EntitySystem
	{
		// Token: 0x06002F77 RID: 12151 RVA: 0x000F5D16 File Offset: 0x000F3F16
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<IdExaminableComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<IdExaminableComponent, GetVerbsEvent<ExamineVerb>>(this.OnGetExamineVerbs), null, null);
		}

		// Token: 0x06002F78 RID: 12152 RVA: 0x000F5D34 File Offset: 0x000F3F34
		private void OnGetExamineVerbs(EntityUid uid, IdExaminableComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			bool detailsRange = this._examineSystem.IsInDetailsRange(args.User, uid);
			string info = this.GetInfo(component.Owner) ?? Loc.GetString("id-examinable-component-verb-no-id");
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate()
				{
					FormattedMessage markup = FormattedMessage.FromMarkup(info);
					this._examineSystem.SendExamineTooltip(args.User, uid, markup, false, false);
				},
				Text = Loc.GetString("id-examinable-component-verb-text"),
				Category = VerbCategory.Examine,
				Disabled = !detailsRange,
				Message = Loc.GetString("id-examinable-component-verb-disabled"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/information.svg.192dpi.png", "/"))
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06002F79 RID: 12153 RVA: 0x000F5E14 File Offset: 0x000F4014
		[NullableContext(2)]
		private string GetInfo(EntityUid uid)
		{
			EntityUid? idUid;
			if (this._inventorySystem.TryGetSlotEntity(uid, "id", out idUid, null, null))
			{
				PDAComponent pda;
				if (this.EntityManager.TryGetComponent<PDAComponent>(idUid, ref pda) && pda.ContainedID != null)
				{
					return this.GetNameAndJob(pda.ContainedID);
				}
				IdCardComponent id;
				if (this.EntityManager.TryGetComponent<IdCardComponent>(idUid, ref id))
				{
					return this.GetNameAndJob(id);
				}
			}
			return null;
		}

		// Token: 0x06002F7A RID: 12154 RVA: 0x000F5E78 File Offset: 0x000F4078
		private string GetNameAndJob(IdCardComponent id)
		{
			string jobSuffix = string.IsNullOrWhiteSpace(id.JobTitle) ? string.Empty : (" (" + id.JobTitle + ")");
			if (!string.IsNullOrWhiteSpace(id.FullName))
			{
				return Loc.GetString("access-id-card-component-owner-full-name-job-title-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("fullName", id.FullName),
					new ValueTuple<string, object>("jobSuffix", jobSuffix)
				});
			}
			return Loc.GetString("access-id-card-component-owner-name-job-title-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("jobSuffix", jobSuffix)
			});
		}

		// Token: 0x04001C8E RID: 7310
		[Dependency]
		private readonly ExamineSystemShared _examineSystem;

		// Token: 0x04001C8F RID: 7311
		[Dependency]
		private readonly InventorySystem _inventorySystem;
	}
}
