﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.Ghost;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Systems.Ghost.Controls
{
	// Token: 0x02000087 RID: 135
	[GenerateTypedNameReferences]
	public sealed class GhostTargetWindow : DefaultWindow
	{
		// Token: 0x14000012 RID: 18
		// (add) Token: 0x06000320 RID: 800 RVA: 0x0001374C File Offset: 0x0001194C
		// (remove) Token: 0x06000321 RID: 801 RVA: 0x00013784 File Offset: 0x00011984
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<EntityUid> WarpClicked;

		// Token: 0x06000322 RID: 802 RVA: 0x000137B9 File Offset: 0x000119B9
		public GhostTargetWindow()
		{
			GhostTargetWindow.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x06000323 RID: 803 RVA: 0x000137D4 File Offset: 0x000119D4
		[NullableContext(1)]
		public void UpdateWarps(IEnumerable<GhostWarp> warps)
		{
			this._warps = (from w in (from w in warps
			orderby w.IsWarpPoint
			select w).ThenBy((GhostWarp w) => w.DisplayName, Comparer<string>.Create((string x, string y) => string.Compare(x, y, StringComparison.Ordinal)))
			select new ValueTuple<string, EntityUid>(w.IsWarpPoint ? Loc.GetString("ghost-target-window-current-button", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("name", w.DisplayName)
			}) : w.DisplayName, w.Entity)).ToList<ValueTuple<string, EntityUid>>();
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0001387D File Offset: 0x00011A7D
		public void Populate()
		{
			this.ButtonContainer.DisposeAllChildren();
			this.AddButtons();
		}

		// Token: 0x06000325 RID: 805 RVA: 0x00013890 File Offset: 0x00011A90
		private void AddButtons()
		{
			foreach (ValueTuple<string, EntityUid> valueTuple in this._warps)
			{
				string item = valueTuple.Item1;
				EntityUid warpTarget = valueTuple.Item2;
				Button button = new Button
				{
					Text = item,
					TextAlign = 2,
					HorizontalAlignment = 2,
					VerticalAlignment = 2,
					SizeFlagsStretchRatio = 1f,
					MinSize = new ValueTuple<float, float>(340f, 20f),
					ClipText = true
				};
				button.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					Action<EntityUid> warpClicked = this.WarpClicked;
					if (warpClicked == null)
					{
						return;
					}
					warpClicked(warpTarget);
				};
				this.ButtonContainer.AddChild(button);
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000326 RID: 806 RVA: 0x00013974 File Offset: 0x00011B74
		private BoxContainer ButtonContainer
		{
			get
			{
				return base.FindControl<BoxContainer>("ButtonContainer");
			}
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00013984 File Offset: 0x00011B84
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.UserInterface.Systems.Ghost.Controls.GhostTargetWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("ghost-target-window-title").ProvideValue();
			A_1.MinSize = new Vector2(450f, 450f);
			A_1.SetSize = new Vector2(450f, 450f);
			ScrollContainer scrollContainer = new ScrollContainer();
			scrollContainer.VerticalExpand = true;
			scrollContainer.HorizontalExpand = true;
			scrollContainer.HScrollEnabled = false;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Name = "ButtonContainer";
			Control control = boxContainer;
			context.RobustNameScope.Register("ButtonContainer", control);
			boxContainer.Orientation = 1;
			boxContainer.VerticalExpand = true;
			boxContainer.SeparationOverride = new int?(5);
			control = boxContainer;
			scrollContainer.XamlChildren.Add(control);
			control = scrollContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06000328 RID: 808 RVA: 0x00013AE0 File Offset: 0x00011CE0
		private static void !XamlIlPopulateTrampoline(GhostTargetWindow A_0)
		{
			GhostTargetWindow.Populate:Content.Client.UserInterface.Systems.Ghost.Controls.GhostTargetWindow.xaml(null, A_0);
		}

		// Token: 0x0400018D RID: 397
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		private List<ValueTuple<string, EntityUid>> _warps = new List<ValueTuple<string, EntityUid>>();
	}
}
