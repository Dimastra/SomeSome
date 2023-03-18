﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.Popups;
using Robust.Client.AutoGenerated;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Suspicion
{
	// Token: 0x020000FE RID: 254
	[GenerateTypedNameReferences]
	public sealed class SuspicionGui : UIWidget
	{
		// Token: 0x06000724 RID: 1828 RVA: 0x000259BC File Offset: 0x00023BBC
		public SuspicionGui()
		{
			SuspicionGui.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<SuspicionGui>(this);
			this.RoleButton.OnPressed += this.RoleButtonPressed;
			this.RoleButton.MinSize = new ValueTuple<float, float>(200f, 60f);
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x00025A14 File Offset: 0x00023C14
		[NullableContext(1)]
		private void RoleButtonPressed(BaseButton.ButtonEventArgs obj)
		{
			SuspicionRoleComponent suspicionRoleComponent;
			if (!this.TryGetComponent(out suspicionRoleComponent))
			{
				return;
			}
			if ((!suspicionRoleComponent.Antagonist).GetValueOrDefault())
			{
				return;
			}
			string item = string.Join(", ", from tuple in suspicionRoleComponent.Allies
			select tuple.Item1);
			suspicionRoleComponent.Owner.PopupMessage(Loc.GetString("suspicion-ally-count-display", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("allyCount", suspicionRoleComponent.Allies.Count),
				new ValueTuple<string, object>("allyNames", item)
			}));
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x00025AE4 File Offset: 0x00023CE4
		[NullableContext(2)]
		private bool TryGetComponent([NotNullWhen(true)] out SuspicionRoleComponent suspicion)
		{
			suspicion = null;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			return localPlayer != null && localPlayer.ControlledEntity != null && this._entManager.TryGetComponent<SuspicionRoleComponent>(this._playerManager.LocalPlayer.ControlledEntity, ref suspicion);
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x00025B38 File Offset: 0x00023D38
		public void UpdateLabel()
		{
			SuspicionRoleComponent suspicionRoleComponent;
			if (!this.TryGetComponent(out suspicionRoleComponent))
			{
				base.Visible = false;
				return;
			}
			if (suspicionRoleComponent.Role == null || suspicionRoleComponent.Antagonist == null)
			{
				base.Visible = false;
				return;
			}
			TimeSpan? endTime = this._entManager.System<SuspicionEndTimerSystem>().EndTime;
			if (endTime == null)
			{
				this.TimerLabel.Visible = false;
			}
			else
			{
				TimeSpan timeSpan = endTime.Value - this._timing.CurTime;
				if (timeSpan < TimeSpan.Zero)
				{
					timeSpan = TimeSpan.Zero;
				}
				this.TimerLabel.Visible = true;
				Label timerLabel = this.TimerLabel;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<TimeSpan>(timeSpan, "mm\\:ss");
				timerLabel.Text = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			if (this._previousRoleName == suspicionRoleComponent.Role)
			{
				bool previousAntagonist = this._previousAntagonist;
				bool? antagonist = suspicionRoleComponent.Antagonist;
				if (previousAntagonist == antagonist.GetValueOrDefault() & antagonist != null)
				{
					return;
				}
			}
			this._previousRoleName = suspicionRoleComponent.Role;
			this._previousAntagonist = suspicionRoleComponent.Antagonist.Value;
			string text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this._previousRoleName);
			text = Loc.GetString(text);
			this.RoleButton.Text = text;
			this.RoleButton.ModulateSelfOverride = new Color?(this._previousAntagonist ? Color.Red : Color.LimeGreen);
			base.Visible = true;
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x00025CAA File Offset: 0x00023EAA
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			this.UpdateLabel();
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000729 RID: 1833 RVA: 0x00025CB9 File Offset: 0x00023EB9
		private Button RoleButton
		{
			get
			{
				return base.FindControl<Button>("RoleButton");
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600072A RID: 1834 RVA: 0x00025CC6 File Offset: 0x00023EC6
		private Label TimerLabel
		{
			get
			{
				return base.FindControl<Label>("TimerLabel");
			}
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x00025CD4 File Offset: 0x00023ED4
		static void xaml(IServiceProvider A_0, SuspicionGui A_1)
		{
			XamlIlContext.Context<SuspicionGui> context = new XamlIlContext.Context<SuspicionGui>(A_0, null, "resm:Content.Client.Suspicion.SuspicionGui.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.SeparationOverride = new int?(0);
			Button button = new Button();
			button.Name = "RoleButton";
			Control control = button;
			context.RobustNameScope.Register("RoleButton", control);
			Label label = new Label();
			label.Name = "TimerLabel";
			control = label;
			context.RobustNameScope.Register("TimerLabel", control);
			label.HorizontalAlignment = 3;
			label.VerticalAlignment = 3;
			control = label;
			button.XamlChildren.Add(control);
			control = button;
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

		// Token: 0x0600072C RID: 1836 RVA: 0x00025E13 File Offset: 0x00024013
		private static void !XamlIlPopulateTrampoline(SuspicionGui A_0)
		{
			SuspicionGui.Populate:Content.Client.Suspicion.SuspicionGui.xaml(null, A_0);
		}

		// Token: 0x04000348 RID: 840
		[Nullable(1)]
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000349 RID: 841
		[Nullable(1)]
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400034A RID: 842
		[Nullable(1)]
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x0400034B RID: 843
		[Nullable(2)]
		private string _previousRoleName;

		// Token: 0x0400034C RID: 844
		private bool _previousAntagonist;
	}
}
