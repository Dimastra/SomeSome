﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Computer;
using Content.Client.UserInterface.Controls;
using Content.Shared.Shuttles.BUIStates;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Shuttles.UI
{
	// Token: 0x0200014D RID: 333
	[GenerateTypedNameReferences]
	public sealed class RadarConsoleWindow : FancyWindow, IComputerWindow<RadarConsoleBoundInterfaceState>
	{
		// Token: 0x060008B2 RID: 2226 RVA: 0x00032E42 File Offset: 0x00031042
		public RadarConsoleWindow()
		{
			RadarConsoleWindow.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x00032E50 File Offset: 0x00031050
		[NullableContext(1)]
		public void UpdateState(RadarConsoleBoundInterfaceState scc)
		{
			this.RadarScreen.UpdateState(scc);
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x00032E5E File Offset: 0x0003105E
		public void SetMatrix(EntityCoordinates? coordinates, Angle? angle)
		{
			this.RadarScreen.SetMatrix(coordinates, angle);
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060008B5 RID: 2229 RVA: 0x00032E6D File Offset: 0x0003106D
		private RadarControl RadarScreen
		{
			get
			{
				return base.FindControl<RadarControl>("RadarScreen");
			}
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x00032E7C File Offset: 0x0003107C
		static void xaml(IServiceProvider A_0, FancyWindow A_1)
		{
			XamlIlContext.Context<FancyWindow> context = new XamlIlContext.Context<FancyWindow>(A_0, null, "resm:Content.Client.Shuttles.UI.RadarConsoleWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("radar-console-window-title").ProvideValue();
			A_1.SetSize = new Vector2(648f, 648f);
			A_1.MinSize = new Vector2(256f, 256f);
			RadarControl radarControl = new RadarControl();
			radarControl.Name = "RadarScreen";
			Control control = radarControl;
			context.RobustNameScope.Register("RadarScreen", control);
			radarControl.Margin = new Thickness(4f, 4f, 4f, 4f);
			radarControl.HorizontalExpand = true;
			radarControl.VerticalExpand = true;
			control = radarControl;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x060008B7 RID: 2231 RVA: 0x00032FAE File Offset: 0x000311AE
		private static void !XamlIlPopulateTrampoline(RadarConsoleWindow A_0)
		{
			RadarConsoleWindow.Populate:Content.Client.Shuttles.UI.RadarConsoleWindow.xaml(null, A_0);
		}
	}
}
