using System;
using System.Runtime.CompilerServices;
using Content.Client.Actions.UI;
using Content.Client.Cooldown;
using Content.Shared.Alert;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Alerts.Controls
{
	// Token: 0x020000BB RID: 187
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AlertControl : BaseButton
	{
		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000511 RID: 1297 RVA: 0x0001BFFA File Offset: 0x0001A1FA
		public AlertPrototype Alert { get; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000512 RID: 1298 RVA: 0x0001C002 File Offset: 0x0001A202
		// (set) Token: 0x06000513 RID: 1299 RVA: 0x0001C00C File Offset: 0x0001A20C
		[TupleElementNames(new string[]
		{
			"Start",
			"End"
		})]
		[Nullable(0)]
		public ValueTuple<TimeSpan, TimeSpan>? Cooldown
		{
			[NullableContext(0)]
			[return: TupleElementNames(new string[]
			{
				"Start",
				"End"
			})]
			get
			{
				return this._cooldown;
			}
			[NullableContext(0)]
			[param: TupleElementNames(new string[]
			{
				"Start",
				"End"
			})]
			set
			{
				this._cooldown = value;
				ActionAlertTooltip actionAlertTooltip = base.SuppliedTooltip as ActionAlertTooltip;
				if (actionAlertTooltip != null)
				{
					actionAlertTooltip.Cooldown = value;
				}
			}
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x0001C038 File Offset: 0x0001A238
		public AlertControl(AlertPrototype alert, short? severity)
		{
			this._gameTiming = IoCManager.Resolve<IGameTiming>();
			base.TooltipDelay = new float?(0.5f);
			base.TooltipSupplier = new TooltipSupplier(this.SupplyTooltip);
			this.Alert = alert;
			this._severity = severity;
			SpriteSpecifier icon = alert.GetIcon(this._severity);
			this._icon = new AnimatedTextureRect
			{
				DisplayRect = 
				{
					TextureScale = new ValueTuple<float, float>(2f, 2f)
				}
			};
			this._icon.SetFromSpriteSpecifier(icon);
			base.Children.Add(this._icon);
			this._cooldownGraphic = new CooldownGraphic();
			base.Children.Add(this._cooldownGraphic);
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x0001C0F8 File Offset: 0x0001A2F8
		private Control SupplyTooltip([Nullable(2)] Control sender)
		{
			FormattedMessage name = FormattedMessage.FromMarkup(Loc.GetString(this.Alert.Name));
			FormattedMessage desc = FormattedMessage.FromMarkup(Loc.GetString(this.Alert.Description));
			return new ActionAlertTooltip(name, desc, null)
			{
				Cooldown = this.Cooldown
			};
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x0001C144 File Offset: 0x0001A344
		public void SetSeverity(short? severity)
		{
			short? num = this._severity;
			int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
			num = severity;
			int? num3 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
			if (!(num2.GetValueOrDefault() == num3.GetValueOrDefault() & num2 != null == (num3 != null)))
			{
				this._severity = severity;
				this._icon.SetFromSpriteSpecifier(this.Alert.GetIcon(this._severity));
			}
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x0001C1E4 File Offset: 0x0001A3E4
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (this.Cooldown == null)
			{
				this._cooldownGraphic.Visible = false;
				this._cooldownGraphic.Progress = 0f;
				return;
			}
			this._cooldownGraphic.FromTime(this.Cooldown.Value.Item1, this.Cooldown.Value.Item2);
		}

		// Token: 0x04000256 RID: 598
		private const float CustomTooltipDelay = 0.5f;

		// Token: 0x04000258 RID: 600
		[TupleElementNames(new string[]
		{
			"Start",
			"End"
		})]
		[Nullable(0)]
		private ValueTuple<TimeSpan, TimeSpan>? _cooldown;

		// Token: 0x04000259 RID: 601
		private short? _severity;

		// Token: 0x0400025A RID: 602
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400025B RID: 603
		private readonly AnimatedTextureRect _icon;

		// Token: 0x0400025C RID: 604
		private readonly CooldownGraphic _cooldownGraphic;
	}
}
