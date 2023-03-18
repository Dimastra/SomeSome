using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.DetailExaminable
{
	// Token: 0x02000594 RID: 1428
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DetailExaminableSystem : EntitySystem
	{
		// Token: 0x06001DD4 RID: 7636 RVA: 0x0009EA16 File Offset: 0x0009CC16
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DetailExaminableComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<DetailExaminableComponent, GetVerbsEvent<ExamineVerb>>(this.OnGetExamineVerbs), null, null);
		}

		// Token: 0x06001DD5 RID: 7637 RVA: 0x0009EA34 File Offset: 0x0009CC34
		private void OnGetExamineVerbs(EntityUid uid, DetailExaminableComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			bool detailsRange = this._examineSystem.IsInDetailsRange(args.User, uid);
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate()
				{
					FormattedMessage markup = new FormattedMessage();
					markup.AddMarkup(component.Content);
					this._examineSystem.SendExamineTooltip(args.User, uid, markup, false, false);
				},
				Text = Loc.GetString("detail-examinable-verb-text"),
				Category = VerbCategory.Examine,
				Disabled = !detailsRange,
				Message = Loc.GetString("detail-examinable-verb-disabled"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/examine.svg.192dpi.png", "/"))
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x0400131D RID: 4893
		[Dependency]
		private readonly ExamineSystemShared _examineSystem;
	}
}
