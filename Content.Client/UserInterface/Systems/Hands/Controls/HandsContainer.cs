using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Hands.Controls
{
	// Token: 0x02000083 RID: 131
	[NullableContext(2)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class HandsContainer : ItemSlotUIContainer<HandButton>
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060002DF RID: 735 RVA: 0x00012AC3 File Offset: 0x00010CC3
		// (set) Token: 0x060002E0 RID: 736 RVA: 0x00012AD0 File Offset: 0x00010CD0
		public int ColumnLimit
		{
			get
			{
				return this._grid.Columns;
			}
			set
			{
				this._grid.Columns = value;
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x00012ADE File Offset: 0x00010CDE
		// (set) Token: 0x060002E2 RID: 738 RVA: 0x00012AE6 File Offset: 0x00010CE6
		public int MaxButtonCount { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x00012AEF File Offset: 0x00010CEF
		// (set) Token: 0x060002E4 RID: 740 RVA: 0x00012AF7 File Offset: 0x00010CF7
		public string Indexer { get; set; }

		// Token: 0x060002E5 RID: 741 RVA: 0x00012B00 File Offset: 0x00010D00
		public HandsContainer()
		{
			base.AddChild(this._grid = new GridContainer());
			this._grid.ExpandBackwards = true;
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00012B33 File Offset: 0x00010D33
		[NullableContext(1)]
		[return: Nullable(2)]
		public override HandButton AddButton(HandButton newButton)
		{
			if (this.MaxButtonCount > 0)
			{
				if (this.ButtonCount >= this.MaxButtonCount)
				{
					return null;
				}
				this._grid.AddChild(newButton);
			}
			else
			{
				this._grid.AddChild(newButton);
			}
			return base.AddButton(newButton);
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00012B70 File Offset: 0x00010D70
		[NullableContext(1)]
		public override void RemoveButton(string handName)
		{
			HandButton button = this.GetButton(handName);
			if (button == null)
			{
				return;
			}
			base.RemoveButton(button);
			this._grid.RemoveChild(button);
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00012B9C File Offset: 0x00010D9C
		public bool TryGetLastButton(out HandButton control)
		{
			if (this.Buttons.Count == 0)
			{
				control = null;
				return false;
			}
			control = this.Buttons.Values.Last<HandButton>();
			return true;
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00012BC3 File Offset: 0x00010DC3
		public bool TryRemoveLastHand(out HandButton control)
		{
			bool result = this.TryGetLastButton(out control);
			if (control != null)
			{
				this.RemoveButton(control);
			}
			return result;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00012BD8 File Offset: 0x00010DD8
		public void Clear()
		{
			base.ClearButtons();
			this._grid.DisposeAllChildren();
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00012BEB File Offset: 0x00010DEB
		[NullableContext(1)]
		public IEnumerable<HandButton> GetButtons()
		{
			foreach (Control control in this._grid.Children)
			{
				HandButton handButton = control as HandButton;
				if (handButton != null)
				{
					yield return handButton;
				}
			}
			Control.OrderedChildCollection.Enumerator enumerator = default(Control.OrderedChildCollection.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060002EC RID: 748 RVA: 0x00012BFB File Offset: 0x00010DFB
		public bool IsFull
		{
			get
			{
				return this.MaxButtonCount != 0 && this.ButtonCount >= this.MaxButtonCount;
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060002ED RID: 749 RVA: 0x00012C18 File Offset: 0x00010E18
		public int ButtonCount
		{
			get
			{
				return this._grid.ChildCount;
			}
		}

		// Token: 0x0400017E RID: 382
		[Nullable(1)]
		private readonly GridContainer _grid;
	}
}
