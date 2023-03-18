﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Humanoid
{
	// Token: 0x020002D1 RID: 721
	[GenerateTypedNameReferences]
	public sealed class HumanoidMarkingModifierWindow : DefaultWindow
	{
		// Token: 0x0600120B RID: 4619 RVA: 0x0006B650 File Offset: 0x00069850
		public HumanoidMarkingModifierWindow()
		{
			HumanoidMarkingModifierWindow.!XamlIlPopulateTrampoline(this);
			this._protoMan = IoCManager.Resolve<IPrototypeManager>();
			HumanoidVisualLayers[] values = Enum.GetValues<HumanoidVisualLayers>();
			for (int i = 0; i < values.Length; i++)
			{
				HumanoidVisualLayers layer = values[i];
				HumanoidMarkingModifierWindow.HumanoidBaseLayerModifier modifier = new HumanoidMarkingModifierWindow.HumanoidBaseLayerModifier(layer);
				this.BaseLayersContainer.AddChild(modifier);
				this._modifiers.Add(layer, modifier);
				HumanoidMarkingModifierWindow.HumanoidBaseLayerModifier modifier2 = modifier;
				modifier2.OnStateChanged = (Action)Delegate.Combine(modifier2.OnStateChanged, new Action(delegate()
				{
					this.OnStateChanged(layer, modifier);
				}));
			}
			MarkingPicker markingPickerWidget = this.MarkingPickerWidget;
			markingPickerWidget.OnMarkingAdded = (Action<MarkingSet>)Delegate.Combine(markingPickerWidget.OnMarkingAdded, new Action<MarkingSet>(delegate(MarkingSet set)
			{
				this.OnMarkingAdded(set);
			}));
			MarkingPicker markingPickerWidget2 = this.MarkingPickerWidget;
			markingPickerWidget2.OnMarkingRemoved = (Action<MarkingSet>)Delegate.Combine(markingPickerWidget2.OnMarkingRemoved, new Action<MarkingSet>(delegate(MarkingSet set)
			{
				this.OnMarkingRemoved(set);
			}));
			MarkingPicker markingPickerWidget3 = this.MarkingPickerWidget;
			markingPickerWidget3.OnMarkingColorChange = (Action<MarkingSet>)Delegate.Combine(markingPickerWidget3.OnMarkingColorChange, new Action<MarkingSet>(delegate(MarkingSet set)
			{
				this.OnMarkingColorChange(set);
			}));
			MarkingPicker markingPickerWidget4 = this.MarkingPickerWidget;
			markingPickerWidget4.OnMarkingRankChange = (Action<MarkingSet>)Delegate.Combine(markingPickerWidget4.OnMarkingRankChange, new Action<MarkingSet>(delegate(MarkingSet set)
			{
				this.OnMarkingRankChange(set);
			}));
			this.MarkingForced.OnToggled += delegate(BaseButton.ButtonToggledEventArgs args)
			{
				this.MarkingPickerWidget.Forced = args.Pressed;
			};
			this.MarkingIgnoreSpecies.OnToggled += delegate(BaseButton.ButtonToggledEventArgs args)
			{
				this.MarkingPickerWidget.Forced = args.Pressed;
			};
			this.MarkingPickerWidget.Forced = this.MarkingForced.Pressed;
			this.MarkingPickerWidget.IgnoreSpecies = this.MarkingForced.Pressed;
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x0006B800 File Offset: 0x00069A00
		[NullableContext(1)]
		private void OnStateChanged(HumanoidVisualLayers layer, HumanoidMarkingModifierWindow.HumanoidBaseLayerModifier modifier)
		{
			if (!modifier.Enabled)
			{
				Action<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo?> onLayerInfoModified = this.OnLayerInfoModified;
				if (onLayerInfoModified == null)
				{
					return;
				}
				onLayerInfoModified(layer, null);
				return;
			}
			else
			{
				string id = this._protoMan.HasIndex<HumanoidSpeciesSpriteLayer>(modifier.Text) ? modifier.Text : null;
				Action<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo?> onLayerInfoModified2 = this.OnLayerInfoModified;
				if (onLayerInfoModified2 == null)
				{
					return;
				}
				onLayerInfoModified2(layer, new HumanoidAppearanceState.CustomBaseLayerInfo?(new HumanoidAppearanceState.CustomBaseLayerInfo(id, new Color?(modifier.Color))));
				return;
			}
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x0006B874 File Offset: 0x00069A74
		[NullableContext(1)]
		public void SetState(MarkingSet markings, string species, Color skinColor, Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> info)
		{
			this.MarkingPickerWidget.SetData(markings, species, skinColor);
			foreach (KeyValuePair<HumanoidVisualLayers, HumanoidMarkingModifierWindow.HumanoidBaseLayerModifier> keyValuePair in this._modifiers)
			{
				HumanoidVisualLayers humanoidVisualLayers;
				HumanoidMarkingModifierWindow.HumanoidBaseLayerModifier humanoidBaseLayerModifier;
				keyValuePair.Deconstruct(out humanoidVisualLayers, out humanoidBaseLayerModifier);
				HumanoidVisualLayers key = humanoidVisualLayers;
				HumanoidMarkingModifierWindow.HumanoidBaseLayerModifier humanoidBaseLayerModifier2 = humanoidBaseLayerModifier;
				HumanoidAppearanceState.CustomBaseLayerInfo customBaseLayerInfo;
				if (!info.TryGetValue(key, out customBaseLayerInfo))
				{
					humanoidBaseLayerModifier2.SetState(false, string.Empty, Color.White);
				}
				else
				{
					humanoidBaseLayerModifier2.SetState(true, customBaseLayerInfo.ID ?? string.Empty, customBaseLayerInfo.Color ?? Color.White);
				}
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x0600120E RID: 4622 RVA: 0x0006B938 File Offset: 0x00069B38
		private MarkingPicker MarkingPickerWidget
		{
			get
			{
				return base.FindControl<MarkingPicker>("MarkingPickerWidget");
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x0600120F RID: 4623 RVA: 0x0006B945 File Offset: 0x00069B45
		private CheckBox MarkingForced
		{
			get
			{
				return base.FindControl<CheckBox>("MarkingForced");
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06001210 RID: 4624 RVA: 0x0006B952 File Offset: 0x00069B52
		private CheckBox MarkingIgnoreSpecies
		{
			get
			{
				return base.FindControl<CheckBox>("MarkingIgnoreSpecies");
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06001211 RID: 4625 RVA: 0x0006B95F File Offset: 0x00069B5F
		private BoxContainer BaseLayersContainer
		{
			get
			{
				return base.FindControl<BoxContainer>("BaseLayersContainer");
			}
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x0006B9B8 File Offset: 0x00069BB8
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Humanoid.HumanoidMarkingModifierWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			ScrollContainer scrollContainer = new ScrollContainer();
			scrollContainer.MinHeight = 500f;
			scrollContainer.MinWidth = 700f;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.HorizontalExpand = true;
			MarkingPicker markingPicker = new MarkingPicker();
			markingPicker.Name = "MarkingPickerWidget";
			Control control = markingPicker;
			context.RobustNameScope.Register("MarkingPickerWidget", control);
			control = markingPicker;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer2 = new BoxContainer();
			CheckBox checkBox = new CheckBox();
			checkBox.Name = "MarkingForced";
			control = checkBox;
			context.RobustNameScope.Register("MarkingForced", control);
			checkBox.Text = "Force";
			checkBox.Pressed = true;
			control = checkBox;
			boxContainer2.XamlChildren.Add(control);
			CheckBox checkBox2 = new CheckBox();
			checkBox2.Name = "MarkingIgnoreSpecies";
			control = checkBox2;
			context.RobustNameScope.Register("MarkingIgnoreSpecies", control);
			checkBox2.Text = "Ignore Species";
			checkBox2.Pressed = true;
			control = checkBox2;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			Collapsible collapsible = new Collapsible();
			collapsible.Orientation = 1;
			collapsible.HorizontalExpand = true;
			control = new CollapsibleHeading
			{
				Title = "Base layers"
			};
			collapsible.XamlChildren.Add(control);
			CollapsibleBody collapsibleBody = new CollapsibleBody();
			collapsibleBody.HorizontalExpand = true;
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Name = "BaseLayersContainer";
			control = boxContainer3;
			context.RobustNameScope.Register("BaseLayersContainer", control);
			boxContainer3.Orientation = 1;
			boxContainer3.HorizontalExpand = true;
			control = boxContainer3;
			collapsibleBody.XamlChildren.Add(control);
			control = collapsibleBody;
			collapsible.XamlChildren.Add(control);
			control = collapsible;
			boxContainer.XamlChildren.Add(control);
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

		// Token: 0x06001219 RID: 4633 RVA: 0x0006BC56 File Offset: 0x00069E56
		private static void !XamlIlPopulateTrampoline(HumanoidMarkingModifierWindow A_0)
		{
			HumanoidMarkingModifierWindow.Populate:Content.Client.Humanoid.HumanoidMarkingModifierWindow.xaml(null, A_0);
		}

		// Token: 0x040008EB RID: 2283
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<MarkingSet> OnMarkingAdded;

		// Token: 0x040008EC RID: 2284
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<MarkingSet> OnMarkingRemoved;

		// Token: 0x040008ED RID: 2285
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<MarkingSet> OnMarkingColorChange;

		// Token: 0x040008EE RID: 2286
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<MarkingSet> OnMarkingRankChange;

		// Token: 0x040008EF RID: 2287
		[Nullable(2)]
		public Action<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo?> OnLayerInfoModified;

		// Token: 0x040008F0 RID: 2288
		[Nullable(1)]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x040008F1 RID: 2289
		[Nullable(1)]
		private readonly Dictionary<HumanoidVisualLayers, HumanoidMarkingModifierWindow.HumanoidBaseLayerModifier> _modifiers = new Dictionary<HumanoidVisualLayers, HumanoidMarkingModifierWindow.HumanoidBaseLayerModifier>();

		// Token: 0x020002D2 RID: 722
		[NullableContext(1)]
		[Nullable(0)]
		private sealed class HumanoidBaseLayerModifier : BoxContainer
		{
			// Token: 0x170003D4 RID: 980
			// (get) Token: 0x0600121A RID: 4634 RVA: 0x0006BC5F File Offset: 0x00069E5F
			public bool Enabled
			{
				get
				{
					return this._enable.Pressed;
				}
			}

			// Token: 0x170003D5 RID: 981
			// (get) Token: 0x0600121B RID: 4635 RVA: 0x0006BC6C File Offset: 0x00069E6C
			public string Text
			{
				get
				{
					return this._lineEdit.Text;
				}
			}

			// Token: 0x170003D6 RID: 982
			// (get) Token: 0x0600121C RID: 4636 RVA: 0x0006BC79 File Offset: 0x00069E79
			public Color Color
			{
				get
				{
					return this._colorSliders.Color;
				}
			}

			// Token: 0x0600121D RID: 4637 RVA: 0x0006BC88 File Offset: 0x00069E88
			public HumanoidBaseLayerModifier(HumanoidVisualLayers layer)
			{
				base.HorizontalExpand = true;
				base.Orientation = 1;
				BoxContainer boxContainer = new BoxContainer
				{
					MinWidth = 250f,
					HorizontalExpand = true
				};
				base.AddChild(boxContainer);
				boxContainer.AddChild(new Label
				{
					HorizontalExpand = true,
					Text = layer.ToString()
				});
				this._enable = new CheckBox
				{
					Text = "Enable",
					HorizontalAlignment = 3
				};
				boxContainer.AddChild(this._enable);
				this._infoBox = new BoxContainer
				{
					Orientation = 1,
					Visible = false
				};
				this._enable.OnToggled += delegate(BaseButton.ButtonToggledEventArgs args)
				{
					this._infoBox.Visible = args.Pressed;
					this.OnStateChanged();
				};
				BoxContainer boxContainer2 = new BoxContainer();
				boxContainer2.AddChild(new Label
				{
					Text = "Prototype id: "
				});
				this._lineEdit = new LineEdit
				{
					MinWidth = 200f
				};
				this._lineEdit.OnTextEntered += delegate(LineEdit.LineEditEventArgs args)
				{
					this.OnStateChanged();
				};
				boxContainer2.AddChild(this._lineEdit);
				this._infoBox.AddChild(boxContainer2);
				this._colorSliders = new ColorSelectorSliders();
				ColorSelectorSliders colorSliders = this._colorSliders;
				colorSliders.OnColorChanged = (Action<Color>)Delegate.Combine(colorSliders.OnColorChanged, new Action<Color>(delegate(Color color)
				{
					this.OnStateChanged();
				}));
				this._infoBox.AddChild(this._colorSliders);
				base.AddChild(this._infoBox);
			}

			// Token: 0x0600121E RID: 4638 RVA: 0x0006BDF6 File Offset: 0x00069FF6
			public void SetState(bool enabled, string state, Color color)
			{
				this._enable.Pressed = enabled;
				this._infoBox.Visible = enabled;
				this._lineEdit.Text = state;
				this._colorSliders.Color = color;
			}

			// Token: 0x040008F2 RID: 2290
			private CheckBox _enable;

			// Token: 0x040008F3 RID: 2291
			private LineEdit _lineEdit;

			// Token: 0x040008F4 RID: 2292
			private ColorSelectorSliders _colorSliders;

			// Token: 0x040008F5 RID: 2293
			private BoxContainer _infoBox;

			// Token: 0x040008F6 RID: 2294
			[Nullable(2)]
			public Action OnStateChanged;
		}
	}
}
