using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Configurable;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Configurable.UI
{
	// Token: 0x02000398 RID: 920
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ConfigurationMenu : DefaultWindow
	{
		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x060016E9 RID: 5865 RVA: 0x000856BC File Offset: 0x000838BC
		public ConfigurationBoundUserInterface Owner { get; }

		// Token: 0x060016EA RID: 5866 RVA: 0x000856C4 File Offset: 0x000838C4
		public ConfigurationMenu(ConfigurationBoundUserInterface owner)
		{
			base.MinSize = (base.SetSize = new ValueTuple<float, float>(300f, 250f));
			this.Owner = owner;
			this._inputs = new List<ValueTuple<string, LineEdit>>();
			base.Title = Loc.GetString("configuration-menu-device-title");
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 1,
				VerticalExpand = true,
				HorizontalExpand = true
			};
			this._column = new BoxContainer
			{
				Orientation = 1,
				Margin = new Thickness(8f),
				SeparationOverride = new int?(16)
			};
			this._row = new BoxContainer
			{
				Orientation = 0,
				SeparationOverride = new int?(16),
				HorizontalExpand = true
			};
			Button button = new Button
			{
				Text = Loc.GetString("configuration-menu-confirm"),
				HorizontalAlignment = 2,
				VerticalAlignment = 2
			};
			button.OnButtonUp += this.OnConfirm;
			ScrollContainer scrollContainer = new ScrollContainer
			{
				VerticalExpand = true,
				HorizontalExpand = true,
				ModulateSelfOverride = new Color?(Color.FromHex("#202020", null))
			};
			scrollContainer.AddChild(this._column);
			boxContainer.AddChild(scrollContainer);
			boxContainer.AddChild(button);
			base.Contents.AddChild(boxContainer);
		}

		// Token: 0x060016EB RID: 5867 RVA: 0x00085820 File Offset: 0x00083A20
		public void Populate(ConfigurationComponent.ConfigurationBoundUserInterfaceState state)
		{
			this._column.Children.Clear();
			this._inputs.Clear();
			foreach (KeyValuePair<string, string> keyValuePair in state.Config)
			{
				Label label = new Label
				{
					Margin = new Thickness(0f, 0f, 8f, 0f),
					Name = keyValuePair.Key,
					Text = keyValuePair.Key + ":",
					VerticalAlignment = 2,
					HorizontalExpand = true,
					SizeFlagsStretchRatio = 0.2f,
					MinSize = new Vector2(60f, 0f)
				};
				LineEdit lineEdit = new LineEdit
				{
					Name = keyValuePair.Key + "-input",
					Text = keyValuePair.Value,
					IsValid = new Func<string, bool>(this.Validate),
					HorizontalExpand = true,
					SizeFlagsStretchRatio = 0.8f
				};
				this._inputs.Add(new ValueTuple<string, LineEdit>(keyValuePair.Key, lineEdit));
				BoxContainer boxContainer = new BoxContainer
				{
					Orientation = 0
				};
				ConfigurationMenu.CopyProperties<BoxContainer>(this._row, boxContainer);
				boxContainer.AddChild(label);
				boxContainer.AddChild(lineEdit);
				this._column.AddChild(boxContainer);
			}
		}

		// Token: 0x060016EC RID: 5868 RVA: 0x000859B0 File Offset: 0x00083BB0
		private void OnConfirm(BaseButton.ButtonEventArgs args)
		{
			Dictionary<string, string> config = this.GenerateDictionary(this._inputs, "Text");
			this.Owner.SendConfiguration(config);
			this.Close();
		}

		// Token: 0x060016ED RID: 5869 RVA: 0x000859E1 File Offset: 0x00083BE1
		private bool Validate(string value)
		{
			return this.Owner.Validation == null || this.Owner.Validation.IsMatch(value);
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x00085A04 File Offset: 0x00083C04
		private Dictionary<string, string> GenerateDictionary([TupleElementNames(new string[]
		{
			"name",
			"input"
		})] [Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})] IEnumerable<ValueTuple<string, LineEdit>> inputs, string propertyName)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (ValueTuple<string, LineEdit> valueTuple in inputs)
			{
				dictionary.Add(valueTuple.Item1, valueTuple.Item2.Text);
			}
			return dictionary;
		}

		// Token: 0x060016EF RID: 5871 RVA: 0x00085A64 File Offset: 0x00083C64
		private static void CopyProperties<[Nullable(0)] T>(T from, T to) where T : Control
		{
			foreach (KeyValuePair<AttachedProperty, object> keyValuePair in from.AllAttachedProperties)
			{
				to.SetValue(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x04000BE9 RID: 3049
		private readonly BoxContainer _column;

		// Token: 0x04000BEA RID: 3050
		private readonly BoxContainer _row;

		// Token: 0x04000BEB RID: 3051
		[TupleElementNames(new string[]
		{
			"name",
			"input"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		private readonly List<ValueTuple<string, LineEdit>> _inputs;
	}
}
