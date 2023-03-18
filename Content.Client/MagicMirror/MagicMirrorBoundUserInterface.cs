using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Markings;
using Content.Shared.MagicMirror;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.MagicMirror
{
	// Token: 0x02000251 RID: 593
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MagicMirrorBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000EF9 RID: 3833 RVA: 0x000021BC File Offset: 0x000003BC
		public MagicMirrorBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x0005A414 File Offset: 0x00058614
		protected override void Open()
		{
			base.Open();
			this._window = new MagicMirrorWindow();
			MagicMirrorWindow window = this._window;
			window.OnHairSelected = (Action<ValueTuple<int, string>>)Delegate.Combine(window.OnHairSelected, new Action<ValueTuple<int, string>>(delegate([TupleElementNames(new string[]
			{
				"slot",
				"id"
			})] [Nullable(new byte[]
			{
				0,
				1
			})] ValueTuple<int, string> tuple)
			{
				this.SelectHair(MagicMirrorCategory.Hair, tuple.Item2, tuple.Item1);
			}));
			MagicMirrorWindow window2 = this._window;
			window2.OnHairColorChanged = (Action<ValueTuple<int, Marking>>)Delegate.Combine(window2.OnHairColorChanged, new Action<ValueTuple<int, Marking>>(delegate([TupleElementNames(new string[]
			{
				"slot",
				"marking"
			})] [Nullable(new byte[]
			{
				0,
				1
			})] ValueTuple<int, Marking> args)
			{
				this.ChangeColor(MagicMirrorCategory.Hair, args.Item2, args.Item1);
			}));
			MagicMirrorWindow window3 = this._window;
			window3.OnHairSlotAdded = (Action)Delegate.Combine(window3.OnHairSlotAdded, new Action(delegate()
			{
				this.AddSlot(MagicMirrorCategory.Hair);
			}));
			MagicMirrorWindow window4 = this._window;
			window4.OnHairSlotRemoved = (Action<int>)Delegate.Combine(window4.OnHairSlotRemoved, new Action<int>(delegate(int args)
			{
				this.RemoveSlot(MagicMirrorCategory.Hair, args);
			}));
			MagicMirrorWindow window5 = this._window;
			window5.OnFacialHairSelected = (Action<ValueTuple<int, string>>)Delegate.Combine(window5.OnFacialHairSelected, new Action<ValueTuple<int, string>>(delegate([TupleElementNames(new string[]
			{
				"slot",
				"id"
			})] [Nullable(new byte[]
			{
				0,
				1
			})] ValueTuple<int, string> tuple)
			{
				this.SelectHair(MagicMirrorCategory.FacialHair, tuple.Item2, tuple.Item1);
			}));
			MagicMirrorWindow window6 = this._window;
			window6.OnFacialHairColorChanged = (Action<ValueTuple<int, Marking>>)Delegate.Combine(window6.OnFacialHairColorChanged, new Action<ValueTuple<int, Marking>>(delegate([TupleElementNames(new string[]
			{
				"slot",
				"marking"
			})] [Nullable(new byte[]
			{
				0,
				1
			})] ValueTuple<int, Marking> args)
			{
				this.ChangeColor(MagicMirrorCategory.FacialHair, args.Item2, args.Item1);
			}));
			MagicMirrorWindow window7 = this._window;
			window7.OnFacialHairSlotAdded = (Action)Delegate.Combine(window7.OnFacialHairSlotAdded, new Action(delegate()
			{
				this.AddSlot(MagicMirrorCategory.FacialHair);
			}));
			MagicMirrorWindow window8 = this._window;
			window8.OnFacialHairSlotRemoved = (Action<int>)Delegate.Combine(window8.OnFacialHairSlotRemoved, new Action<int>(delegate(int args)
			{
				this.RemoveSlot(MagicMirrorCategory.FacialHair, args);
			}));
			this._window.OpenCentered();
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x0005A575 File Offset: 0x00058775
		private void SelectHair(MagicMirrorCategory category, string marking, int slot)
		{
			base.SendMessage(new MagicMirrorSelectMessage(category, marking, slot));
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x0005A585 File Offset: 0x00058785
		private void ChangeColor(MagicMirrorCategory category, Marking marking, int slot)
		{
			base.SendMessage(new MagicMirrorChangeColorMessage(category, new List<Color>(marking.MarkingColors), slot));
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x0005A59F File Offset: 0x0005879F
		private void RemoveSlot(MagicMirrorCategory category, int slot)
		{
			base.SendMessage(new MagicMirrorRemoveSlotMessage(category, slot));
		}

		// Token: 0x06000EFE RID: 3838 RVA: 0x0005A5AE File Offset: 0x000587AE
		private void AddSlot(MagicMirrorCategory category)
		{
			base.SendMessage(new MagicMirrorAddSlotMessage(category));
		}

		// Token: 0x06000EFF RID: 3839 RVA: 0x0005A5BC File Offset: 0x000587BC
		protected override void ReceiveMessage(BoundUserInterfaceMessage message)
		{
			base.ReceiveMessage(message);
			MagicMirrorUiData magicMirrorUiData = message as MagicMirrorUiData;
			if (magicMirrorUiData == null || this._window == null)
			{
				return;
			}
			this._window.UpdateState(magicMirrorUiData);
		}

		// Token: 0x04000776 RID: 1910
		[Nullable(2)]
		private MagicMirrorWindow _window;
	}
}
