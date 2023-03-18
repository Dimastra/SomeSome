using System;
using System.Runtime.CompilerServices;
using Content.Shared.Light;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Light.Components
{
	// Token: 0x0200026E RID: 622
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HandheldLightStatus : Control
	{
		// Token: 0x06000FC9 RID: 4041 RVA: 0x0005F080 File Offset: 0x0005D280
		public HandheldLightStatus(HandheldLightComponent parent)
		{
			this._parent = parent;
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 0,
				SeparationOverride = new int?(4),
				HorizontalAlignment = 2
			};
			base.AddChild(boxContainer);
			for (int i = 0; i < this._sections.Length; i++)
			{
				PanelContainer panelContainer = new PanelContainer
				{
					MinSize = new ValueTuple<float, float>(20f, 20f)
				};
				boxContainer.AddChild(panelContainer);
				this._sections[i] = panelContainer;
			}
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x0005F110 File Offset: 0x0005D310
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			this._timer += args.DeltaSeconds;
			this._timer %= 1f;
			byte? level = this._parent.Level;
			for (int i = 0; i < this._sections.Length; i++)
			{
				if (i == 0)
				{
					byte? b = level;
					int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
					int num2 = 0;
					if ((num.GetValueOrDefault() == num2 & num != null) || level == null)
					{
						this._sections[0].PanelOverride = HandheldLightStatus.StyleBoxUnlit;
					}
					else
					{
						b = level;
						num = ((b != null) ? new int?((int)b.GetValueOrDefault()) : null);
						num2 = 1;
						if (num.GetValueOrDefault() == num2 & num != null)
						{
							this._sections[0].PanelOverride = ((this._timer > 0.5f) ? HandheldLightStatus.StyleBoxLit : HandheldLightStatus.StyleBoxUnlit);
						}
						else
						{
							this._sections[0].PanelOverride = HandheldLightStatus.StyleBoxLit;
						}
					}
				}
				else
				{
					PanelContainer panelContainer = this._sections[i];
					byte? b = level;
					int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
					int num2 = i + 2;
					panelContainer.PanelOverride = ((num.GetValueOrDefault() >= num2 & num != null) ? HandheldLightStatus.StyleBoxLit : HandheldLightStatus.StyleBoxUnlit);
				}
			}
		}

		// Token: 0x040007CA RID: 1994
		private const float TimerCycle = 1f;

		// Token: 0x040007CB RID: 1995
		private readonly HandheldLightComponent _parent;

		// Token: 0x040007CC RID: 1996
		private readonly PanelContainer[] _sections = new PanelContainer[5];

		// Token: 0x040007CD RID: 1997
		private float _timer;

		// Token: 0x040007CE RID: 1998
		private static readonly StyleBoxFlat StyleBoxLit = new StyleBoxFlat
		{
			BackgroundColor = Color.LimeGreen
		};

		// Token: 0x040007CF RID: 1999
		private static readonly StyleBoxFlat StyleBoxUnlit = new StyleBoxFlat
		{
			BackgroundColor = Color.Black
		};
	}
}
