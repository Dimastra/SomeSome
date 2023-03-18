using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Message;
using Content.Shared.GameTicking;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.RoundEnd
{
	// Token: 0x02000163 RID: 355
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RoundEndSummaryWindow : DefaultWindow
	{
		// Token: 0x0600094C RID: 2380 RVA: 0x00036570 File Offset: 0x00034770
		public RoundEndSummaryWindow(string gm, string roundEnd, TimeSpan roundTimeSpan, int roundId, RoundEndMessageEvent.RoundEndPlayerInfo[] info, IEntityManager entityManager)
		{
			this._entityManager = entityManager;
			base.MinSize = (base.SetSize = new ValueTuple<float, float>(520f, 580f));
			base.Title = Loc.GetString("round-end-summary-window-title");
			TabContainer tabContainer = new TabContainer();
			tabContainer.AddChild(this.MakeRoundEndSummaryTab(gm, roundEnd, roundTimeSpan, roundId));
			tabContainer.AddChild(this.MakePlayerManifestoTab(info));
			base.Contents.AddChild(tabContainer);
			base.OpenCentered();
			base.MoveToFront();
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x000365FC File Offset: 0x000347FC
		private BoxContainer MakeRoundEndSummaryTab(string gamemode, string roundEnd, TimeSpan roundDuration, int roundId)
		{
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 1,
				Name = Loc.GetString("round-end-summary-window-round-end-summary-tab-title")
			};
			ScrollContainer scrollContainer = new ScrollContainer
			{
				VerticalExpand = true
			};
			BoxContainer boxContainer2 = new BoxContainer
			{
				Orientation = 1
			};
			RichTextLabel richTextLabel = new RichTextLabel();
			FormattedMessage formattedMessage = new FormattedMessage();
			formattedMessage.AddMarkup(Loc.GetString("round-end-summary-window-round-id-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("roundId", roundId)
			}));
			formattedMessage.AddText(" ");
			formattedMessage.AddMarkup(Loc.GetString("round-end-summary-window-gamemode-name-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("gamemode", gamemode)
			}));
			richTextLabel.SetMessage(formattedMessage);
			boxContainer2.AddChild(richTextLabel);
			RichTextLabel richTextLabel2 = new RichTextLabel();
			richTextLabel2.SetMarkup(Loc.GetString("round-end-summary-window-duration-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("hours", roundDuration.Hours),
				new ValueTuple<string, object>("minutes", roundDuration.Minutes),
				new ValueTuple<string, object>("seconds", roundDuration.Seconds)
			}));
			boxContainer2.AddChild(richTextLabel2);
			if (!string.IsNullOrEmpty(roundEnd))
			{
				RichTextLabel richTextLabel3 = new RichTextLabel();
				richTextLabel3.SetMarkup(roundEnd);
				boxContainer2.AddChild(richTextLabel3);
			}
			scrollContainer.AddChild(boxContainer2);
			boxContainer.AddChild(scrollContainer);
			return boxContainer;
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0003676C File Offset: 0x0003496C
		private BoxContainer MakePlayerManifestoTab(RoundEndMessageEvent.RoundEndPlayerInfo[] playersInfo)
		{
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 1,
				Name = Loc.GetString("round-end-summary-window-player-manifesto-tab-title")
			};
			ScrollContainer scrollContainer = new ScrollContainer
			{
				VerticalExpand = true
			};
			BoxContainer boxContainer2 = new BoxContainer
			{
				Orientation = 1
			};
			foreach (RoundEndMessageEvent.RoundEndPlayerInfo roundEndPlayerInfo in from p in playersInfo
			orderby p.Observer, !p.Antag
			select p)
			{
				BoxContainer boxContainer3 = new BoxContainer
				{
					Orientation = 0
				};
				RichTextLabel richTextLabel = new RichTextLabel
				{
					VerticalAlignment = 2,
					VerticalExpand = true
				};
				SpriteComponent sprite;
				if (this._entityManager.TryGetComponent<SpriteComponent>(roundEndPlayerInfo.PlayerEntityUid, ref sprite))
				{
					boxContainer3.AddChild(new SpriteView
					{
						Sprite = sprite,
						OverrideDirection = new Direction?(0),
						VerticalAlignment = 2,
						VerticalExpand = true
					});
				}
				if (roundEndPlayerInfo.PlayerICName != null)
				{
					if (roundEndPlayerInfo.Observer)
					{
						richTextLabel.SetMarkup(Loc.GetString("round-end-summary-window-player-info-if-observer-text", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("playerOOCName", roundEndPlayerInfo.PlayerOOCName),
							new ValueTuple<string, object>("playerICName", roundEndPlayerInfo.PlayerICName)
						}));
					}
					else
					{
						string item = roundEndPlayerInfo.Antag ? "red" : "white";
						richTextLabel.SetMarkup(Loc.GetString("round-end-summary-window-player-info-if-not-observer-text", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("playerOOCName", roundEndPlayerInfo.PlayerOOCName),
							new ValueTuple<string, object>("icNameColor", item),
							new ValueTuple<string, object>("playerICName", roundEndPlayerInfo.PlayerICName),
							new ValueTuple<string, object>("playerRole", Loc.GetString(roundEndPlayerInfo.Role))
						}));
					}
				}
				boxContainer3.AddChild(richTextLabel);
				boxContainer2.AddChild(boxContainer3);
			}
			scrollContainer.AddChild(boxContainer2);
			boxContainer.AddChild(scrollContainer);
			return boxContainer;
		}

		// Token: 0x040004B0 RID: 1200
		private readonly IEntityManager _entityManager;
	}
}
