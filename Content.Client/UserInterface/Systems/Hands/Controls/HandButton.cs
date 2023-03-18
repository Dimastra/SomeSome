using System;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Controls;
using Content.Shared.Hands.Components;

namespace Content.Client.UserInterface.Systems.Hands.Controls
{
	// Token: 0x02000082 RID: 130
	public sealed class HandButton : SlotControl
	{
		// Token: 0x060002DD RID: 733 RVA: 0x00012A55 File Offset: 0x00010C55
		[NullableContext(1)]
		public HandButton(string handName, HandLocation handLocation)
		{
			base.Name = "hand_" + handName;
			base.SlotName = handName;
			this.SetBackground(handLocation);
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00012A7C File Offset: 0x00010C7C
		private void SetBackground(HandLocation handLoc)
		{
			string buttonTexturePath;
			switch (handLoc)
			{
			case HandLocation.Left:
				buttonTexturePath = "Slots/hand_l";
				break;
			case HandLocation.Middle:
				buttonTexturePath = "Slots/hand_m";
				break;
			case HandLocation.Right:
				buttonTexturePath = "Slots/hand_r";
				break;
			default:
				buttonTexturePath = base.ButtonTexturePath;
				break;
			}
			base.ButtonTexturePath = buttonTexturePath;
		}
	}
}
