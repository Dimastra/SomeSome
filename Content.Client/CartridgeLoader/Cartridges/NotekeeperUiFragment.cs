﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.CartridgeLoader.Cartridges
{
	// Token: 0x02000400 RID: 1024
	[GenerateTypedNameReferences]
	public sealed class NotekeeperUiFragment : BoxContainer
	{
		// Token: 0x14000094 RID: 148
		// (add) Token: 0x06001920 RID: 6432 RVA: 0x0009015C File Offset: 0x0008E35C
		// (remove) Token: 0x06001921 RID: 6433 RVA: 0x00090194 File Offset: 0x0008E394
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<string> OnNoteAdded;

		// Token: 0x14000095 RID: 149
		// (add) Token: 0x06001922 RID: 6434 RVA: 0x000901CC File Offset: 0x0008E3CC
		// (remove) Token: 0x06001923 RID: 6435 RVA: 0x00090204 File Offset: 0x0008E404
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<string> OnNoteRemoved;

		// Token: 0x06001924 RID: 6436 RVA: 0x0009023C File Offset: 0x0008E43C
		public NotekeeperUiFragment()
		{
			NotekeeperUiFragment.!XamlIlPopulateTrampoline(this);
			base.Orientation = 1;
			base.HorizontalExpand = true;
			base.VerticalExpand = true;
			this.Input.OnTextEntered += delegate(LineEdit.LineEditEventArgs _)
			{
				this.AddNote(this.Input.Text);
				Action<string> onNoteAdded = this.OnNoteAdded;
				if (onNoteAdded != null)
				{
					onNoteAdded(this.Input.Text);
				}
				this.Input.Clear();
			};
			this.UpdateState(new List<string>());
		}

		// Token: 0x06001925 RID: 6437 RVA: 0x0009028C File Offset: 0x0008E48C
		[NullableContext(1)]
		public void UpdateState(List<string> notes)
		{
			this.MessageContainer.RemoveAllChildren();
			foreach (string note in notes)
			{
				this.AddNote(note);
			}
		}

		// Token: 0x06001926 RID: 6438 RVA: 0x000902E8 File Offset: 0x0008E4E8
		[NullableContext(1)]
		private void AddNote(string note)
		{
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.HorizontalExpand = true;
			boxContainer.Orientation = 0;
			boxContainer.Margin = new Thickness(4f);
			Label label = new Label();
			label.Text = note;
			label.HorizontalExpand = true;
			label.ClipText = true;
			TextureButton textureButton = new TextureButton();
			textureButton.AddStyleClass("windowCloseButton");
			textureButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				Action<string> onNoteRemoved = this.OnNoteRemoved;
				if (onNoteRemoved == null)
				{
					return;
				}
				onNoteRemoved(label.Text);
			};
			boxContainer.AddChild(label);
			boxContainer.AddChild(textureButton);
			this.MessageContainer.AddChild(boxContainer);
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06001927 RID: 6439 RVA: 0x00090397 File Offset: 0x0008E597
		private BoxContainer MessageContainer
		{
			get
			{
				return base.FindControl<BoxContainer>("MessageContainer");
			}
		}

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06001928 RID: 6440 RVA: 0x00049E45 File Offset: 0x00048045
		private LineEdit Input
		{
			get
			{
				return base.FindControl<LineEdit>("Input");
			}
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x000903E0 File Offset: 0x0008E5E0
		static void xaml(IServiceProvider A_0, NotekeeperUiFragment A_1)
		{
			XamlIlContext.Context<NotekeeperUiFragment> context = new XamlIlContext.Context<NotekeeperUiFragment>(A_0, null, "resm:Content.Client.CartridgeLoader.Cartridges.NotekeeperUiFragment.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Margin = new Thickness(1f, 0f, 2f, 0f);
			PanelContainer panelContainer = new PanelContainer();
			string item = "BackgroundDark";
			panelContainer.StyleClasses.Add(item);
			Control control = panelContainer;
			A_1.XamlChildren.Add(control);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.HorizontalExpand = true;
			boxContainer.VerticalExpand = true;
			ScrollContainer scrollContainer = new ScrollContainer();
			scrollContainer.HorizontalExpand = true;
			scrollContainer.VerticalExpand = true;
			scrollContainer.HScrollEnabled = true;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 1;
			boxContainer2.Name = "MessageContainer";
			control = boxContainer2;
			context.RobustNameScope.Register("MessageContainer", control);
			boxContainer2.HorizontalExpand = true;
			boxContainer2.VerticalExpand = true;
			control = boxContainer2;
			scrollContainer.XamlChildren.Add(control);
			control = scrollContainer;
			boxContainer.XamlChildren.Add(control);
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "Input";
			control = lineEdit;
			context.RobustNameScope.Register("Input", control);
			lineEdit.HorizontalExpand = true;
			lineEdit.SetHeight = 32f;
			control = lineEdit;
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

		// Token: 0x0600192B RID: 6443 RVA: 0x000905CE File Offset: 0x0008E7CE
		private static void !XamlIlPopulateTrampoline(NotekeeperUiFragment A_0)
		{
			NotekeeperUiFragment.Populate:Content.Client.CartridgeLoader.Cartridges.NotekeeperUiFragment.xaml(null, A_0);
		}
	}
}
