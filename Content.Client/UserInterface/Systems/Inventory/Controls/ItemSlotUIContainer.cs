using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Log;

namespace Content.Client.UserInterface.Systems.Inventory.Controls
{
	// Token: 0x0200007C RID: 124
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public abstract class ItemSlotUIContainer<[Nullable(0)] T> : GridContainer, IItemslotUIContainer where T : SlotControl
	{
		// Token: 0x06000295 RID: 661 RVA: 0x000110C4 File Offset: 0x0000F2C4
		public virtual bool TryAddButton(T newButton, out T button)
		{
			if (this.AddButton(newButton) == null)
			{
				button = newButton;
				return false;
			}
			button = newButton;
			return true;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x000110E8 File Offset: 0x0000F2E8
		public void ClearButtons()
		{
			foreach (T t in this.Buttons.Values)
			{
				t.Dispose();
			}
			this.Buttons.Clear();
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00011150 File Offset: 0x0000F350
		public bool TryRegisterButton(SlotControl control, string newSlotName)
		{
			if (newSlotName == "")
			{
				return false;
			}
			T t = control as T;
			if (t == null)
			{
				return false;
			}
			T t2;
			if (!this.Buttons.TryGetValue(newSlotName, out t2))
			{
				this.Buttons.Remove(t.SlotName);
				this.AddButton(t);
				return true;
			}
			if (control == t2)
			{
				return true;
			}
			throw new Exception("Could not update button to slot:" + newSlotName + " slot already assigned!");
		}

		// Token: 0x06000298 RID: 664 RVA: 0x000111D4 File Offset: 0x0000F3D4
		public bool TryAddButton(SlotControl control)
		{
			T t = control as T;
			return t != null && this.AddButton(t) != null;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00011208 File Offset: 0x0000F408
		[return: Nullable(2)]
		public virtual T AddButton(T newButton)
		{
			if (!base.Children.Contains(newButton) && newButton.Parent == null && newButton.SlotName != "")
			{
				base.AddChild(newButton);
			}
			return this.AddButtonToDict(newButton);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00011260 File Offset: 0x0000F460
		[return: Nullable(2)]
		protected virtual T AddButtonToDict(T newButton)
		{
			if (newButton.SlotName == "")
			{
				Logger.Warning("Could not add button " + newButton.Name + "No slotname");
			}
			if (this.Buttons.TryAdd(newButton.SlotName, newButton))
			{
				return newButton;
			}
			return default(T);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x000112C8 File Offset: 0x0000F4C8
		public virtual void RemoveButton(string slotName)
		{
			T button;
			if (!this.Buttons.TryGetValue(slotName, out button))
			{
				return;
			}
			this.RemoveButton(button);
		}

		// Token: 0x0600029C RID: 668 RVA: 0x000112F0 File Offset: 0x0000F4F0
		public virtual void RemoveButtons(params string[] slotNames)
		{
			foreach (string slotName in slotNames)
			{
				this.RemoveButton(slotName);
			}
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00011318 File Offset: 0x0000F518
		public virtual void RemoveButtons([Nullable(new byte[]
		{
			1,
			2
		})] params T[] buttons)
		{
			foreach (T t in buttons)
			{
				if (t != null)
				{
					this.RemoveButton(t);
				}
			}
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0001134C File Offset: 0x0000F54C
		protected virtual void RemoveButtonFromDict(T button)
		{
			this.Buttons.Remove(button.SlotName);
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00011365 File Offset: 0x0000F565
		public virtual void RemoveButton(T button)
		{
			this.RemoveButtonFromDict(button);
			base.Children.Remove(button);
			button.Dispose();
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0001138C File Offset: 0x0000F58C
		[return: Nullable(2)]
		public virtual T GetButton(string slotName)
		{
			T result;
			if (this.Buttons.TryGetValue(slotName, out result))
			{
				return result;
			}
			return default(T);
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x000113B4 File Offset: 0x0000F5B4
		public virtual bool TryGetButton(string slotName, [Nullable(2)] [NotNullWhen(true)] out T button)
		{
			return (button = this.GetButton(slotName)) != null;
		}

		// Token: 0x04000170 RID: 368
		protected readonly Dictionary<string, T> Buttons = new Dictionary<string, T>();
	}
}
