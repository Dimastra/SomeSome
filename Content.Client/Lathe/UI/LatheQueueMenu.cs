﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.Research.Prototypes;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Lathe.UI
{
	// Token: 0x02000282 RID: 642
	[GenerateTypedNameReferences]
	public sealed class LatheQueueMenu : DefaultWindow
	{
		// Token: 0x0600106E RID: 4206 RVA: 0x000622FF File Offset: 0x000604FF
		public LatheQueueMenu()
		{
			LatheQueueMenu.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<LatheQueueMenu>(this);
			this._spriteSystem = this._entityManager.EntitySysManager.GetEntitySystem<SpriteSystem>();
			this.SetInfo(null);
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x00062334 File Offset: 0x00060534
		[NullableContext(2)]
		public void SetInfo(LatheRecipePrototype recipe)
		{
			if (recipe != null)
			{
				this.Icon.Texture = ((recipe.Icon == null) ? this._spriteSystem.GetPrototypeIcon(recipe.Result).Default : this._spriteSystem.Frame0(recipe.Icon));
				this.NameLabel.Text = recipe.Name;
				this.Description.Text = recipe.Description;
				return;
			}
			this.Icon.Texture = Texture.Transparent;
			this.NameLabel.Text = string.Empty;
			this.Description.Text = Loc.GetString("lathe-queue-menu-not-producing-text");
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x000623D8 File Offset: 0x000605D8
		[NullableContext(1)]
		public void PopulateList(List<LatheRecipePrototype> queue)
		{
			this.QueueList.Clear();
			int num = 1;
			foreach (LatheRecipePrototype latheRecipePrototype in queue)
			{
				Texture texture = (latheRecipePrototype.Icon == null) ? this._spriteSystem.GetPrototypeIcon(latheRecipePrototype.Result).Default : this._spriteSystem.Frame0(latheRecipePrototype.Icon);
				ItemList queueList = this.QueueList;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
				defaultInterpolatedStringHandler.AppendFormatted<int>(num);
				defaultInterpolatedStringHandler.AppendLiteral(". ");
				defaultInterpolatedStringHandler.AppendFormatted(latheRecipePrototype.Name);
				queueList.AddItem(defaultInterpolatedStringHandler.ToStringAndClear(), texture, true);
				num++;
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06001071 RID: 4209 RVA: 0x00023682 File Offset: 0x00021882
		private TextureRect Icon
		{
			get
			{
				return base.FindControl<TextureRect>("Icon");
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06001072 RID: 4210 RVA: 0x0001A0AC File Offset: 0x000182AC
		private Label NameLabel
		{
			get
			{
				return base.FindControl<Label>("NameLabel");
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06001073 RID: 4211 RVA: 0x0000F67B File Offset: 0x0000D87B
		private Label Description
		{
			get
			{
				return base.FindControl<Label>("Description");
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06001074 RID: 4212 RVA: 0x000624A4 File Offset: 0x000606A4
		private ItemList QueueList
		{
			get
			{
				return base.FindControl<ItemList>("QueueList");
			}
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x000624B4 File Offset: 0x000606B4
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Lathe.UI.LatheQueueMenu.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("lathe-queue-menu-title").ProvideValue();
			A_1.MinSize = new Vector2(300f, 450f);
			A_1.SetSize = new Vector2(300f, 450f);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			boxContainer2.HorizontalExpand = true;
			boxContainer2.SizeFlagsStretchRatio = 2f;
			TextureRect textureRect = new TextureRect();
			textureRect.Name = "Icon";
			Control control = textureRect;
			context.RobustNameScope.Register("Icon", control);
			textureRect.HorizontalExpand = true;
			textureRect.SizeFlagsStretchRatio = 2f;
			control = textureRect;
			boxContainer2.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 1;
			boxContainer3.VerticalExpand = true;
			boxContainer3.SizeFlagsStretchRatio = 3f;
			Label label = new Label();
			label.Name = "NameLabel";
			control = label;
			context.RobustNameScope.Register("NameLabel", control);
			label.RectClipContent = true;
			control = label;
			boxContainer3.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "Description";
			control = label2;
			context.RobustNameScope.Register("Description", control);
			label2.RectClipContent = true;
			label2.VerticalAlignment = 0;
			label2.VerticalExpand = true;
			control = label2;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			ItemList itemList = new ItemList();
			itemList.Name = "QueueList";
			control = itemList;
			context.RobustNameScope.Register("QueueList", control);
			itemList.VerticalExpand = true;
			itemList.SizeFlagsStretchRatio = 3f;
			itemList.SelectMode = 0;
			control = itemList;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x00062766 File Offset: 0x00060966
		private static void !XamlIlPopulateTrampoline(LatheQueueMenu A_0)
		{
			LatheQueueMenu.Populate:Content.Client.Lathe.UI.LatheQueueMenu.xaml(null, A_0);
		}

		// Token: 0x04000817 RID: 2071
		[Nullable(1)]
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000818 RID: 2072
		[Nullable(1)]
		private readonly SpriteSystem _spriteSystem;
	}
}