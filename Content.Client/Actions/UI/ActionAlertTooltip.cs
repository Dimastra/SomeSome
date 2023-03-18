using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Actions.UI
{
	// Token: 0x020004F4 RID: 1268
	public sealed class ActionAlertTooltip : PanelContainer
	{
		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x06002048 RID: 8264 RVA: 0x000BB34F File Offset: 0x000B954F
		// (set) Token: 0x06002049 RID: 8265 RVA: 0x000BB357 File Offset: 0x000B9557
		[TupleElementNames(new string[]
		{
			"Start",
			"End"
		})]
		public ValueTuple<TimeSpan, TimeSpan>? Cooldown { [return: TupleElementNames(new string[]
		{
			"Start",
			"End"
		})] get; [param: TupleElementNames(new string[]
		{
			"Start",
			"End"
		})] set; }

		// Token: 0x0600204A RID: 8266 RVA: 0x000BB360 File Offset: 0x000B9560
		[NullableContext(2)]
		public ActionAlertTooltip([Nullable(1)] FormattedMessage name, FormattedMessage desc, string requires = null)
		{
			this._gameTiming = IoCManager.Resolve<IGameTiming>();
			base.SetOnlyStyleClass("tooltipBox");
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.RectClipContent = true;
			BoxContainer boxContainer2 = boxContainer;
			base.AddChild(boxContainer);
			RichTextLabel richTextLabel = new RichTextLabel
			{
				MaxWidth = 350f,
				StyleClasses = 
				{
					"tooltipActionTitle"
				}
			};
			richTextLabel.SetMessage(name);
			boxContainer2.AddChild(richTextLabel);
			if (desc != null && !string.IsNullOrWhiteSpace(desc.ToString()))
			{
				RichTextLabel richTextLabel2 = new RichTextLabel
				{
					MaxWidth = 350f,
					StyleClasses = 
					{
						"tooltipActionDesc"
					}
				};
				richTextLabel2.SetMessage(desc);
				boxContainer2.AddChild(richTextLabel2);
			}
			Control control = boxContainer2;
			RichTextLabel richTextLabel3 = new RichTextLabel();
			richTextLabel3.MaxWidth = 350f;
			richTextLabel3.StyleClasses.Add("tooltipActionCooldown");
			richTextLabel3.Visible = false;
			RichTextLabel richTextLabel4 = richTextLabel3;
			this._cooldownLabel = richTextLabel3;
			control.AddChild(richTextLabel4);
			if (!string.IsNullOrWhiteSpace(requires))
			{
				RichTextLabel richTextLabel5 = new RichTextLabel
				{
					MaxWidth = 350f,
					StyleClasses = 
					{
						"tooltipActionCooldown"
					}
				};
				richTextLabel5.SetMessage(FormattedMessage.FromMarkup("[color=#635c5c]" + requires + "[/color]"));
				boxContainer2.AddChild(richTextLabel5);
			}
		}

		// Token: 0x0600204B RID: 8267 RVA: 0x000BB498 File Offset: 0x000B9698
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (this.Cooldown == null)
			{
				this._cooldownLabel.Visible = false;
				return;
			}
			TimeSpan t = this.Cooldown.Value.Item2 - this._gameTiming.CurTime;
			if (t > TimeSpan.Zero)
			{
				TimeSpan timeSpan = this.Cooldown.Value.Item2 - this.Cooldown.Value.Item1;
				RichTextLabel cooldownLabel = this._cooldownLabel;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[color=#a10505]");
				defaultInterpolatedStringHandler.AppendFormatted<int>((int)timeSpan.TotalSeconds);
				defaultInterpolatedStringHandler.AppendLiteral(" sec cooldown (");
				defaultInterpolatedStringHandler.AppendFormatted<int>((int)t.TotalSeconds + 1);
				defaultInterpolatedStringHandler.AppendLiteral(" sec remaining)[/color]");
				cooldownLabel.SetMessage(FormattedMessage.FromMarkup(defaultInterpolatedStringHandler.ToStringAndClear()));
				this._cooldownLabel.Visible = true;
				return;
			}
			this._cooldownLabel.Visible = false;
		}

		// Token: 0x04000F66 RID: 3942
		private const float TooltipTextMaxWidth = 350f;

		// Token: 0x04000F67 RID: 3943
		[Nullable(1)]
		private readonly RichTextLabel _cooldownLabel;

		// Token: 0x04000F68 RID: 3944
		[Nullable(1)]
		private readonly IGameTiming _gameTiming;
	}
}
