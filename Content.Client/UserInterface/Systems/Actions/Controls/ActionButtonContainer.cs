using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.Actions.Controls
{
	// Token: 0x020000CA RID: 202
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public class ActionButtonContainer : GridContainer
	{
		// Token: 0x1400002A RID: 42
		// (add) Token: 0x060005A9 RID: 1449 RVA: 0x0001F248 File Offset: 0x0001D448
		// (remove) Token: 0x060005AA RID: 1450 RVA: 0x0001F280 File Offset: 0x0001D480
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public event Action<GUIBoundKeyEventArgs, ActionButton> ActionPressed;

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x060005AB RID: 1451 RVA: 0x0001F2B8 File Offset: 0x0001D4B8
		// (remove) Token: 0x060005AC RID: 1452 RVA: 0x0001F2F0 File Offset: 0x0001D4F0
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public event Action<GUIBoundKeyEventArgs, ActionButton> ActionUnpressed;

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x060005AD RID: 1453 RVA: 0x0001F328 File Offset: 0x0001D528
		// (remove) Token: 0x060005AE RID: 1454 RVA: 0x0001F360 File Offset: 0x0001D560
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<ActionButton> ActionFocusExited;

		// Token: 0x060005AF RID: 1455 RVA: 0x0001F395 File Offset: 0x0001D595
		public ActionButtonContainer()
		{
			IoCManager.InjectDependencies<ActionButtonContainer>(this);
			base.UserInterfaceManager.GetUIController<ActionUIController>().RegisterActionContainer(this);
		}

		// Token: 0x170000DF RID: 223
		public ActionButton this[int index]
		{
			get
			{
				return (ActionButton)base.GetChild(index);
			}
			set
			{
				base.AddChild(value);
				value.SetPositionInParent(index);
				value.ActionPressed += this.ActionPressed;
				value.ActionUnpressed += this.ActionUnpressed;
				value.ActionFocusExited += this.ActionFocusExited;
			}
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x0001F3F8 File Offset: 0x0001D5F8
		public void SetActionData([Nullable(new byte[]
		{
			1,
			2
		})] params ActionType[] actionTypes)
		{
			this.ClearActionData();
			for (int i = 0; i < actionTypes.Length; i++)
			{
				ActionType actionType = actionTypes[i];
				if (actionType != null)
				{
					((ActionButton)base.GetChild(i)).UpdateData(actionType);
				}
			}
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x0001F434 File Offset: 0x0001D634
		public void ClearActionData()
		{
			foreach (Control control in base.Children)
			{
				((ActionButton)control).ClearData();
			}
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x0001F48C File Offset: 0x0001D68C
		protected override void ChildAdded(Control newChild)
		{
			base.ChildAdded(newChild);
			ActionButton actionButton = newChild as ActionButton;
			if (actionButton == null)
			{
				return;
			}
			actionButton.ActionPressed += this.ActionPressed;
			actionButton.ActionUnpressed += this.ActionUnpressed;
			actionButton.ActionFocusExited += this.ActionFocusExited;
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x0001F4CF File Offset: 0x0001D6CF
		public bool TryGetButtonIndex(ActionButton button, out int position)
		{
			if (button.Parent != this)
			{
				position = 0;
				return false;
			}
			position = button.GetPositionInParent();
			return true;
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x0001F4E8 File Offset: 0x0001D6E8
		public IEnumerable<ActionButton> GetButtons()
		{
			foreach (Control control in base.Children)
			{
				ActionButton actionButton = control as ActionButton;
				if (actionButton != null)
				{
					yield return actionButton;
				}
			}
			Control.OrderedChildCollection.Enumerator enumerator = default(Control.OrderedChildCollection.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x0001F4F8 File Offset: 0x0001D6F8
		~ActionButtonContainer()
		{
			base.UserInterfaceManager.GetUIController<ActionUIController>().RemoveActionContainer();
		}
	}
}
