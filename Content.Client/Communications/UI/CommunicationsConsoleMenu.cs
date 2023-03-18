using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Communications.UI
{
	// Token: 0x0200039D RID: 925
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CommunicationsConsoleMenu : DefaultWindow
	{
		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x0600170C RID: 5900 RVA: 0x00085E23 File Offset: 0x00084023
		// (set) Token: 0x0600170D RID: 5901 RVA: 0x00085E2B File Offset: 0x0008402B
		private CommunicationsConsoleBoundUserInterface Owner { get; set; }

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x0600170E RID: 5902 RVA: 0x00085E34 File Offset: 0x00084034
		// (set) Token: 0x0600170F RID: 5903 RVA: 0x00085E3C File Offset: 0x0008403C
		private LineEdit _messageInput { get; set; }

		// Token: 0x06001710 RID: 5904 RVA: 0x00085E48 File Offset: 0x00084048
		public CommunicationsConsoleMenu(CommunicationsConsoleBoundUserInterface owner)
		{
			base.SetSize = (base.MinSize = new ValueTuple<float, float>(600f, 400f));
			IoCManager.InjectDependencies<CommunicationsConsoleMenu>(this);
			base.Title = Loc.GetString("comms-console-menu-title");
			this.Owner = owner;
			this._messageInput = new LineEdit
			{
				PlaceHolder = Loc.GetString("comms-console-menu-announcement-placeholder"),
				HorizontalExpand = true,
				SizeFlagsStretchRatio = 1f
			};
			this.AnnounceButton = new Button();
			this.AnnounceButton.Text = Loc.GetString("comms-console-menu-announcement-button");
			this.AnnounceButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.Owner.AnnounceButtonPressed(this._messageInput.Text.Trim());
			};
			this.AnnounceButton.Disabled = !owner.CanAnnounce;
			this.AlertLevelButton = new OptionButton();
			this.AlertLevelButton.OnItemSelected += delegate(OptionButton.ItemSelectedEventArgs args)
			{
				object itemMetadata = this.AlertLevelButton.GetItemMetadata(args.Id);
				if (itemMetadata != null)
				{
					string text = itemMetadata as string;
					if (text != null)
					{
						this.Owner.AlertLevelSelected(text);
					}
				}
			};
			this.AlertLevelButton.Disabled = !owner.AlertLevelSelectable;
			this._countdownLabel = new RichTextLabel
			{
				MinSize = new Vector2(0f, 200f)
			};
			this.EmergencyShuttleButton = new Button();
			this.EmergencyShuttleButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.Owner.EmergencyShuttleButtonPressed();
			};
			this.EmergencyShuttleButton.Disabled = !owner.CanCall;
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 1,
				HorizontalExpand = true,
				VerticalExpand = true
			};
			boxContainer.AddChild(this._messageInput);
			boxContainer.AddChild(new Control
			{
				MinSize = new Vector2(0f, 10f),
				HorizontalExpand = true
			});
			boxContainer.AddChild(this.AnnounceButton);
			boxContainer.AddChild(this.AlertLevelButton);
			boxContainer.AddChild(new Control
			{
				MinSize = new Vector2(0f, 10f),
				HorizontalExpand = true
			});
			boxContainer.AddChild(this._countdownLabel);
			boxContainer.AddChild(this.EmergencyShuttleButton);
			BoxContainer boxContainer2 = new BoxContainer
			{
				Orientation = 0,
				HorizontalExpand = true,
				VerticalExpand = true
			};
			boxContainer2.AddChild(new Control
			{
				MinSize = new Vector2(100f, 0f),
				HorizontalExpand = true
			});
			boxContainer2.AddChild(boxContainer);
			boxContainer2.AddChild(new Control
			{
				MinSize = new Vector2(100f, 0f),
				HorizontalExpand = true
			});
			base.Contents.AddChild(boxContainer2);
			this.UpdateCountdown();
			Timer.SpawnRepeating(1000, new Action(this.UpdateCountdown), this._timerCancelTokenSource.Token);
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x000860F4 File Offset: 0x000842F4
		public void UpdateAlertLevels([Nullable(new byte[]
		{
			2,
			1
		})] List<string> alerts, string currentAlert)
		{
			this.AlertLevelButton.Clear();
			if (alerts == null)
			{
				string text = currentAlert;
				string text2;
				if (Loc.TryGetString("alert-level-" + currentAlert, ref text2))
				{
					text = text2;
				}
				this.AlertLevelButton.AddItem(text, null);
				this.AlertLevelButton.SetItemMetadata(this.AlertLevelButton.ItemCount - 1, currentAlert);
				return;
			}
			foreach (string text3 in alerts)
			{
				string text4 = text3;
				string text5;
				if (Loc.TryGetString("alert-level-" + text3, ref text5))
				{
					text4 = text5;
				}
				this.AlertLevelButton.AddItem(text4, null);
				this.AlertLevelButton.SetItemMetadata(this.AlertLevelButton.ItemCount - 1, text3);
				if (text3 == currentAlert)
				{
					this.AlertLevelButton.Select(this.AlertLevelButton.ItemCount - 1);
				}
			}
		}

		// Token: 0x06001712 RID: 5906 RVA: 0x00086204 File Offset: 0x00084404
		public void UpdateCountdown()
		{
			if (!this.Owner.CountdownStarted)
			{
				this._countdownLabel.SetMessage("");
				this.EmergencyShuttleButton.Text = Loc.GetString("comms-console-menu-call-shuttle");
				return;
			}
			this.EmergencyShuttleButton.Text = Loc.GetString("comms-console-menu-recall-shuttle");
			this._countdownLabel.SetMessage("Time remaining\n" + this.Owner.Countdown.ToString() + "s");
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x00086286 File Offset: 0x00084486
		public override void Close()
		{
			base.Close();
			this._timerCancelTokenSource.Cancel();
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x00086299 File Offset: 0x00084499
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this._timerCancelTokenSource.Cancel();
			}
		}

		// Token: 0x04000BF7 RID: 3063
		private readonly CancellationTokenSource _timerCancelTokenSource = new CancellationTokenSource();

		// Token: 0x04000BF9 RID: 3065
		public readonly Button AnnounceButton;

		// Token: 0x04000BFA RID: 3066
		public readonly Button EmergencyShuttleButton;

		// Token: 0x04000BFB RID: 3067
		private readonly RichTextLabel _countdownLabel;

		// Token: 0x04000BFC RID: 3068
		public readonly OptionButton AlertLevelButton;
	}
}
