﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.ContextMenu.UI;
using Content.Client.Examine;
using Content.Client.Guidebook.Richtext;
using Content.Client.Verbs.UI;
using Content.Shared.Input;
using Content.Shared.Tag;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Guidebook.Controls
{
	// Token: 0x020002F5 RID: 757
	[NullableContext(1)]
	[Nullable(0)]
	[GenerateTypedNameReferences]
	public sealed class GuideEntityEmbed : BoxContainer, IDocumentTag
	{
		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06001303 RID: 4867 RVA: 0x0007124F File Offset: 0x0006F44F
		// (set) Token: 0x06001304 RID: 4868 RVA: 0x0007125C File Offset: 0x0006F45C
		[Nullable(2)]
		public SpriteComponent Sprite
		{
			[NullableContext(2)]
			get
			{
				return this.View.Sprite;
			}
			[NullableContext(2)]
			set
			{
				this.View.Sprite = value;
			}
		}

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06001305 RID: 4869 RVA: 0x0007126A File Offset: 0x0006F46A
		// (set) Token: 0x06001306 RID: 4870 RVA: 0x00071277 File Offset: 0x0006F477
		public Vector2 Scale
		{
			get
			{
				return this.View.Scale;
			}
			set
			{
				this.View.Scale = value;
			}
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x00071288 File Offset: 0x0006F488
		public GuideEntityEmbed()
		{
			GuideEntityEmbed.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<GuideEntityEmbed>(this);
			this._tagSystem = this._systemManager.GetEntitySystem<TagSystem>();
			this._examineSystem = this._systemManager.GetEntitySystem<ExamineSystem>();
			this._guidebookSystem = this._systemManager.GetEntitySystem<GuidebookSystem>();
			base.MouseFilter = 0;
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x000712E4 File Offset: 0x0006F4E4
		public GuideEntityEmbed(string proto, bool caption, bool interactive) : this()
		{
			this.Interactive = interactive;
			EntityUid entityUid = this._entityManager.SpawnEntity(proto, MapCoordinates.Nullspace);
			this.Sprite = this._entityManager.GetComponent<SpriteComponent>(entityUid);
			if (caption)
			{
				this.Caption.Text = this._entityManager.GetComponent<MetaDataComponent>(entityUid).EntityName;
			}
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x00071344 File Offset: 0x0006F544
		protected override void KeyBindDown(GUIBoundKeyEventArgs args)
		{
			base.KeyBindDown(args);
			SpriteComponent sprite = this.Sprite;
			EntityUid? entityUid = (sprite != null) ? new EntityUid?(sprite.Owner) : null;
			if (this._entityManager.Deleted(entityUid))
			{
				return;
			}
			if (args.Function == ContentKeyFunctions.ExamineEntity)
			{
				this._examineSystem.DoExamine(entityUid.Value, true);
				args.Handle();
				return;
			}
			if (!this.Interactive)
			{
				return;
			}
			if (args.Function == EngineKeyFunctions.UseSecondary)
			{
				this._ui.GetUIController<VerbMenuUIController>().OpenVerbMenu(entityUid.Value, false, null);
				args.Handle();
				return;
			}
			if (args.Function == ContentKeyFunctions.ActivateItemInWorld)
			{
				this._guidebookSystem.FakeClientActivateInWorld(entityUid.Value);
				this._ui.GetUIController<ContextMenuUIController>().Close();
				args.Handle();
				return;
			}
			if (args.Function == ContentKeyFunctions.AltActivateItemInWorld)
			{
				this._guidebookSystem.FakeClientAltActivateInWorld(entityUid.Value);
				this._ui.GetUIController<ContextMenuUIController>().Close();
				args.Handle();
				return;
			}
			if (args.Function == ContentKeyFunctions.AltActivateItemInWorld)
			{
				this._guidebookSystem.FakeClientUse(entityUid.Value);
				this._ui.GetUIController<ContextMenuUIController>().Close();
				args.Handle();
			}
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x0007149E File Offset: 0x0006F69E
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (this.Sprite != null)
			{
				this._entityManager.DeleteEntity(this.Sprite.Owner);
			}
		}

		// Token: 0x0600130B RID: 4875 RVA: 0x000714C8 File Offset: 0x0006F6C8
		public bool TryParseTag(Dictionary<string, string> args, [Nullable(2)] [NotNullWhen(true)] out Control control)
		{
			string text;
			if (!args.TryGetValue("Entity", out text))
			{
				Logger.Error("Entity embed tag is missing entity prototype argument");
				control = null;
				return false;
			}
			EntityUid entityUid = this._entityManager.SpawnEntity(text, MapCoordinates.Nullspace);
			this._tagSystem.AddTag(entityUid, "GuideEmbeded");
			this.Sprite = this._entityManager.GetComponent<SpriteComponent>(entityUid);
			string entityName;
			if (!args.TryGetValue("Caption", out entityName))
			{
				entityName = this._entityManager.GetComponent<MetaDataComponent>(entityUid).EntityName;
			}
			if (!string.IsNullOrEmpty(entityName))
			{
				this.Caption.Text = entityName;
			}
			string s;
			if (args.TryGetValue("Scale", out s))
			{
				float num = float.Parse(s);
				this.Scale = new Vector2(num, num);
			}
			else
			{
				this.Scale = new ValueTuple<float, float>(2f, 2f);
			}
			string value;
			if (args.TryGetValue("Interactive", out value))
			{
				this.Interactive = bool.Parse(value);
			}
			base.Margin = new Thickness(4f, 8f);
			control = this;
			return true;
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x0600130C RID: 4876 RVA: 0x000715D5 File Offset: 0x0006F7D5
		[Nullable(0)]
		private SpriteView View
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<SpriteView>("View");
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x0600130D RID: 4877 RVA: 0x000715E2 File Offset: 0x0006F7E2
		[Nullable(0)]
		private Label Caption
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<Label>("Caption");
			}
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x000715F0 File Offset: 0x0006F7F0
		static void xaml(IServiceProvider A_0, BoxContainer A_1)
		{
			XamlIlContext.Context<BoxContainer> context = new XamlIlContext.Context<BoxContainer>(A_0, null, "resm:Content.Client.Guidebook.Controls.GuideEntityEmbed.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Orientation = 1;
			A_1.Margin = new Thickness(5f, 5f, 5f, 5f);
			SpriteView spriteView = new SpriteView();
			spriteView.Name = "View";
			Control control = spriteView;
			context.RobustNameScope.Register("View", control);
			control = spriteView;
			A_1.XamlChildren.Add(control);
			Label label = new Label();
			label.Name = "Caption";
			control = label;
			context.RobustNameScope.Register("Caption", control);
			label.HorizontalAlignment = 2;
			control = label;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x0007171B File Offset: 0x0006F91B
		private static void !XamlIlPopulateTrampoline(GuideEntityEmbed A_0)
		{
			GuideEntityEmbed.Populate:Content.Client.Guidebook.Controls.GuideEntityEmbed.xaml(null, A_0);
		}

		// Token: 0x04000984 RID: 2436
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000985 RID: 2437
		[Dependency]
		private readonly IEntitySystemManager _systemManager;

		// Token: 0x04000986 RID: 2438
		[Dependency]
		private readonly IUserInterfaceManager _ui;

		// Token: 0x04000987 RID: 2439
		private readonly TagSystem _tagSystem;

		// Token: 0x04000988 RID: 2440
		private readonly ExamineSystem _examineSystem;

		// Token: 0x04000989 RID: 2441
		private readonly GuidebookSystem _guidebookSystem;

		// Token: 0x0400098A RID: 2442
		public bool Interactive;
	}
}
