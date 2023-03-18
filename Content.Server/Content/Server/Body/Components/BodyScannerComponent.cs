using System;
using System.Runtime.CompilerServices;
using Content.Server.UserInterface;
using Content.Shared.Body.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Body.Components
{
	// Token: 0x02000711 RID: 1809
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedBodyScannerComponent))]
	public sealed class BodyScannerComponent : SharedBodyScannerComponent
	{
		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x0600261A RID: 9754 RVA: 0x000C9640 File Offset: 0x000C7840
		[Nullable(2)]
		[ViewVariables]
		private BoundUserInterface UserInterface
		{
			[NullableContext(2)]
			get
			{
				return base.Owner.GetUIOrNull(BodyScannerUiKey.Key);
			}
		}

		// Token: 0x0600261B RID: 9755 RVA: 0x000C9653 File Offset: 0x000C7853
		protected override void Initialize()
		{
			base.Initialize();
			ComponentExt.EnsureComponentWarn<ServerUserInterfaceComponent>(base.Owner, null);
			if (this.UserInterface != null)
			{
				this.UserInterface.OnReceiveMessage += this.UserInterfaceOnOnReceiveMessage;
			}
		}

		// Token: 0x0600261C RID: 9756 RVA: 0x000C9687 File Offset: 0x000C7887
		private void UserInterfaceOnOnReceiveMessage(ServerBoundUserInterfaceMessage serverMsg)
		{
		}

		// Token: 0x0600261D RID: 9757 RVA: 0x000C9689 File Offset: 0x000C7889
		private BodyScannerUIState InterfaceState(BodyComponent body)
		{
			return new BodyScannerUIState(body.Owner);
		}
	}
}
