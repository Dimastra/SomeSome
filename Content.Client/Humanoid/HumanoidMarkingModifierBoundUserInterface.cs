using System;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Humanoid
{
	// Token: 0x020002D0 RID: 720
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HumanoidMarkingModifierBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001205 RID: 4613 RVA: 0x000021BC File Offset: 0x000003BC
		public HumanoidMarkingModifierBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x0006B4D4 File Offset: 0x000696D4
		protected override void Open()
		{
			base.Open();
			this._window = new HumanoidMarkingModifierWindow();
			this._window.OnClose += base.Close;
			HumanoidMarkingModifierWindow window = this._window;
			window.OnMarkingAdded = (Action<MarkingSet>)Delegate.Combine(window.OnMarkingAdded, new Action<MarkingSet>(this.SendMarkingSet));
			HumanoidMarkingModifierWindow window2 = this._window;
			window2.OnMarkingRemoved = (Action<MarkingSet>)Delegate.Combine(window2.OnMarkingRemoved, new Action<MarkingSet>(this.SendMarkingSet));
			HumanoidMarkingModifierWindow window3 = this._window;
			window3.OnMarkingColorChange = (Action<MarkingSet>)Delegate.Combine(window3.OnMarkingColorChange, new Action<MarkingSet>(this.SendMarkingSetNoResend));
			HumanoidMarkingModifierWindow window4 = this._window;
			window4.OnMarkingRankChange = (Action<MarkingSet>)Delegate.Combine(window4.OnMarkingRankChange, new Action<MarkingSet>(this.SendMarkingSet));
			HumanoidMarkingModifierWindow window5 = this._window;
			window5.OnLayerInfoModified = (Action<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo?>)Delegate.Combine(window5.OnLayerInfoModified, new Action<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo?>(this.SendBaseLayer));
			this._window.OpenCenteredLeft();
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x0006B5D8 File Offset: 0x000697D8
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				HumanoidMarkingModifierState humanoidMarkingModifierState = state as HumanoidMarkingModifierState;
				if (humanoidMarkingModifierState != null)
				{
					this._window.SetState(humanoidMarkingModifierState.MarkingSet, humanoidMarkingModifierState.Species, humanoidMarkingModifierState.SkinColor, humanoidMarkingModifierState.CustomBaseLayers);
					return;
				}
			}
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x0006B622 File Offset: 0x00069822
		private void SendMarkingSet(MarkingSet set)
		{
			base.SendMessage(new HumanoidMarkingModifierMarkingSetMessage(set, true));
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x0006B631 File Offset: 0x00069831
		private void SendMarkingSetNoResend(MarkingSet set)
		{
			base.SendMessage(new HumanoidMarkingModifierMarkingSetMessage(set, false));
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x0006B640 File Offset: 0x00069840
		private void SendBaseLayer(HumanoidVisualLayers layer, HumanoidAppearanceState.CustomBaseLayerInfo? info)
		{
			base.SendMessage(new HumanoidMarkingModifierBaseLayersSetMessage(layer, info, true));
		}

		// Token: 0x040008EA RID: 2282
		[Nullable(2)]
		private HumanoidMarkingModifierWindow _window;
	}
}
