using System;
using System.Runtime.CompilerServices;
using Content.Client.ContextMenu.UI;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Verbs.UI
{
	// Token: 0x02000062 RID: 98
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VerbMenuElement : ContextMenuElement
	{
		// Token: 0x17000042 RID: 66
		// (set) Token: 0x060001C6 RID: 454 RVA: 0x0000CAD5 File Offset: 0x0000ACD5
		public bool IconVisible
		{
			set
			{
				base.Icon.Visible = value;
			}
		}

		// Token: 0x17000043 RID: 67
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x0000CAE3 File Offset: 0x0000ACE3
		public bool TextVisible
		{
			set
			{
				base.Label.Visible = value;
			}
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000CAF4 File Offset: 0x0000ACF4
		public VerbMenuElement(Verb verb) : base(verb.Text)
		{
			base.ToolTip = verb.Message;
			base.TooltipDelay = new float?(0.5f);
			base.Disabled = verb.Disabled;
			this.Verb = verb;
			base.Label.SetOnlyStyleClass(verb.TextStyleClass);
			if (verb.ConfirmationPopup)
			{
				base.ExpansionIndicator.SetOnlyStyleClass("verbMenuConfirmationTexture");
				base.ExpansionIndicator.Visible = true;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			if (verb.Icon == null && verb.IconEntity != null)
			{
				SpriteView spriteView = new SpriteView
				{
					OverrideDirection = new Direction?(0),
					Sprite = EntityManagerExt.GetComponentOrNull<SpriteComponent>(entityManager, verb.IconEntity.Value)
				};
				base.Icon.AddChild(spriteView);
				return;
			}
			base.Icon.AddChild(new TextureRect
			{
				Texture = ((verb.Icon != null) ? entityManager.System<SpriteSystem>().Frame0(verb.Icon) : null),
				Stretch = 7
			});
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000CBFC File Offset: 0x0000ADFC
		public VerbMenuElement(VerbCategory category, string styleClass) : base(category.Text)
		{
			base.Label.SetOnlyStyleClass(styleClass);
			base.Icon.AddChild(new TextureRect
			{
				Texture = ((category.Icon != null) ? IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SpriteSystem>().Frame0(category.Icon) : null),
				Stretch = 7
			});
		}

		// Token: 0x04000130 RID: 304
		public const string StyleClassVerbMenuConfirmationTexture = "verbMenuConfirmationTexture";

		// Token: 0x04000131 RID: 305
		public const float VerbTooltipDelay = 0.5f;

		// Token: 0x04000132 RID: 306
		[Nullable(2)]
		public readonly Verb Verb;
	}
}
