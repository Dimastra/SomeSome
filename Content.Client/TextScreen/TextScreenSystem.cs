using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.TextScreen;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.TextScreen
{
	// Token: 0x020000F7 RID: 247
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class TextScreenSystem : VisualizerSystem<TextScreenVisualsComponent>
	{
		// Token: 0x060006EB RID: 1771 RVA: 0x00024524 File Offset: 0x00022724
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TextScreenVisualsComponent, ComponentInit>(new ComponentEventHandler<TextScreenVisualsComponent, ComponentInit>(this.OnInit), null, null);
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x00024540 File Offset: 0x00022740
		private void OnInit(EntityUid uid, TextScreenVisualsComponent component, ComponentInit args)
		{
			SpriteComponent sprite;
			if (!base.TryComp<SpriteComponent>(uid, ref sprite))
			{
				return;
			}
			this.ResetTextLength(component, sprite);
			this.PrepareLayerStatesToDraw(component, sprite);
			this.UpdateLayersToDraw(component, sprite);
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x00024574 File Offset: 0x00022774
		public void ResetTextLength(TextScreenVisualsComponent component, [Nullable(2)] SpriteComponent sprite = null)
		{
			if (!base.Resolve<SpriteComponent>(component.Owner, ref sprite, true))
			{
				return;
			}
			foreach (KeyValuePair<string, string> keyValuePair in component.LayerStatesToDraw)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string text3 = text;
				sprite.RemoveLayer(text3);
			}
			component.LayerStatesToDraw.Clear();
			int textLength = component.TextLength;
			component.TextLength = 0;
			this.SetTextLength(component, textLength, sprite);
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x0002460C File Offset: 0x0002280C
		public void SetTextLength(TextScreenVisualsComponent component, int newLength, [Nullable(2)] SpriteComponent sprite = null)
		{
			if (newLength == component.TextLength)
			{
				return;
			}
			if (!base.Resolve<SpriteComponent>(component.Owner, ref sprite, true))
			{
				return;
			}
			if (newLength > component.TextLength)
			{
				for (int i = component.TextLength; i < newLength; i++)
				{
					sprite.LayerMapReserveBlank("textScreenLayerMapKey" + i.ToString());
					component.LayerStatesToDraw.Add("textScreenLayerMapKey" + i.ToString(), null);
					sprite.LayerSetRSI("textScreenLayerMapKey" + i.ToString(), new ResourcePath("Effects/text.rsi", "/"));
					sprite.LayerSetColor("textScreenLayerMapKey" + i.ToString(), component.Color);
					sprite.LayerSetState("textScreenLayerMapKey" + i.ToString(), "blank");
				}
			}
			else
			{
				for (int j = component.TextLength; j > newLength; j--)
				{
					sprite.LayerMapGet("textScreenLayerMapKey" + (j - 1).ToString());
					component.LayerStatesToDraw.Remove("textScreenLayerMapKey" + (j - 1).ToString());
					sprite.RemoveLayer("textScreenLayerMapKey" + (j - 1).ToString());
				}
			}
			this.UpdateOffsets(component, sprite);
			component.TextLength = newLength;
		}

		// Token: 0x060006EF RID: 1775 RVA: 0x0002476C File Offset: 0x0002296C
		public void UpdateOffsets(TextScreenVisualsComponent component, [Nullable(2)] SpriteComponent sprite = null)
		{
			if (!base.Resolve<SpriteComponent>(component.Owner, ref sprite, true))
			{
				return;
			}
			for (int i = 0; i < component.LayerStatesToDraw.Count; i++)
			{
				float num = (float)i - (float)(component.LayerStatesToDraw.Count - 1) / 2f;
				sprite.LayerSetOffset("textScreenLayerMapKey" + i.ToString(), new Vector2(num * 0.03125f * 4f, 0f) + component.TextOffset);
			}
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x000247F2 File Offset: 0x000229F2
		protected override void OnAppearanceChange(EntityUid uid, TextScreenVisualsComponent component, ref AppearanceChangeEvent args)
		{
			this.UpdateAppearance(component, args.Component, args.Sprite);
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x00024808 File Offset: 0x00022A08
		[NullableContext(2)]
		public void UpdateAppearance([Nullable(1)] TextScreenVisualsComponent component, AppearanceComponent appearance = null, SpriteComponent sprite = null)
		{
			if (!base.Resolve<AppearanceComponent, SpriteComponent>(component.Owner, ref appearance, ref sprite, true))
			{
				return;
			}
			bool activated;
			if (appearance.TryGetData<bool>(TextScreenVisuals.On, ref activated))
			{
				component.Activated = activated;
				this.UpdateVisibility(component, sprite);
			}
			TextScreenMode currentMode;
			if (appearance.TryGetData<TextScreenMode>(TextScreenVisuals.Mode, ref currentMode))
			{
				component.CurrentMode = currentMode;
				TextScreenSystem.UpdateText(component);
			}
			TimeSpan targetTime;
			if (appearance.TryGetData<TimeSpan>(TextScreenVisuals.TargetTime, ref targetTime))
			{
				component.TargetTime = targetTime;
			}
			string text;
			if (appearance.TryGetData<string>(TextScreenVisuals.ScreenText, ref text))
			{
				component.Text = text;
				TextScreenSystem.UpdateText(component);
			}
			this.PrepareLayerStatesToDraw(component, null);
			this.UpdateLayersToDraw(component, sprite);
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x000248A9 File Offset: 0x00022AA9
		public static void UpdateText(TextScreenVisualsComponent component)
		{
			if (component.CurrentMode == TextScreenMode.Text)
			{
				component.TextToDraw = component.Text;
			}
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x000248C0 File Offset: 0x00022AC0
		public void UpdateVisibility(TextScreenVisualsComponent component, [Nullable(2)] SpriteComponent sprite = null)
		{
			if (!base.Resolve<SpriteComponent>(component.Owner, ref sprite, true))
			{
				return;
			}
			foreach (KeyValuePair<string, string> keyValuePair in component.LayerStatesToDraw)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string text3 = text;
				sprite.LayerSetVisible(text3, component.Activated);
			}
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x00024938 File Offset: 0x00022B38
		public void PrepareLayerStatesToDraw(TextScreenVisualsComponent component, [Nullable(2)] SpriteComponent sprite = null)
		{
			if (!base.Resolve<SpriteComponent>(component.Owner, ref sprite, true))
			{
				return;
			}
			for (int i = 0; i < component.TextLength; i++)
			{
				if (i >= component.TextToDraw.Length)
				{
					component.LayerStatesToDraw["textScreenLayerMapKey" + i.ToString()] = "blank";
				}
				else
				{
					component.LayerStatesToDraw["textScreenLayerMapKey" + i.ToString()] = TextScreenSystem.GetStateFromChar(new char?(component.TextToDraw[i]));
				}
			}
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x000249CC File Offset: 0x00022BCC
		public void UpdateLayersToDraw(TextScreenVisualsComponent component, [Nullable(2)] SpriteComponent sprite = null)
		{
			if (!base.Resolve<SpriteComponent>(component.Owner, ref sprite, true))
			{
				return;
			}
			foreach (KeyValuePair<string, string> keyValuePair in component.LayerStatesToDraw)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string text3 = text;
				string text4 = text2;
				if (text4 != null)
				{
					sprite.LayerSetState(text3, text4);
				}
			}
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x00024A4C File Offset: 0x00022C4C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (TextScreenVisualsComponent textScreenVisualsComponent in base.EntityQuery<TextScreenVisualsComponent>(false))
			{
				if (textScreenVisualsComponent.CurrentMode == TextScreenMode.Timer)
				{
					TimeSpan timeSpan = (this._gameTiming.CurTime > textScreenVisualsComponent.TargetTime) ? (this._gameTiming.CurTime - textScreenVisualsComponent.TargetTime) : (textScreenVisualsComponent.TargetTime - this._gameTiming.CurTime);
					textScreenVisualsComponent.TextToDraw = TextScreenSystem.TimeToString(timeSpan, false);
					this.PrepareLayerStatesToDraw(textScreenVisualsComponent, null);
					this.UpdateLayersToDraw(textScreenVisualsComponent, null);
				}
			}
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x00024B08 File Offset: 0x00022D08
		public static string TimeToString(TimeSpan timeSpan, bool getMilliseconds = true)
		{
			string str;
			string str2;
			if (timeSpan.TotalHours >= 1.0)
			{
				str = timeSpan.Hours.ToString("D2");
				str2 = timeSpan.Minutes.ToString("D2");
			}
			else if (timeSpan.TotalMinutes >= 1.0 || !getMilliseconds)
			{
				str = timeSpan.Minutes.ToString("D2");
				str2 = (timeSpan.Seconds + ((timeSpan.Milliseconds > 500) ? 1 : 0)).ToString("D2");
			}
			else
			{
				str = timeSpan.Seconds.ToString("D2");
				str2 = (timeSpan.Milliseconds / 10).ToString("D2");
			}
			return str + ":" + str2;
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x00024BE4 File Offset: 0x00022DE4
		[NullableContext(2)]
		private static string GetStateFromChar(char? character)
		{
			if (character == null)
			{
				return null;
			}
			if (TextScreenSystem.CharStatePairs.ContainsKey(character.Value))
			{
				return TextScreenSystem.CharStatePairs[character.Value];
			}
			if (char.IsLetterOrDigit(character.Value))
			{
				return character.Value.ToString().ToLower();
			}
			return null;
		}

		// Token: 0x0400032B RID: 811
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400032C RID: 812
		private static readonly Dictionary<char, string> CharStatePairs = new Dictionary<char, string>
		{
			{
				'ё',
				"е"
			},
			{
				':',
				"colon"
			},
			{
				'!',
				"exclamation"
			},
			{
				'?',
				"question"
			},
			{
				'*',
				"star"
			},
			{
				'+',
				"plus"
			},
			{
				'-',
				"dash"
			},
			{
				' ',
				"blank"
			},
			{
				'.',
				"point"
			},
			{
				'/',
				"line"
			},
			{
				',',
				"comma"
			}
		};

		// Token: 0x0400032D RID: 813
		private const string DefaultState = "blank";

		// Token: 0x0400032E RID: 814
		private const string TextScreenLayerMapKey = "textScreenLayerMapKey";
	}
}
