using System;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.ViewVariables;

namespace Content.Client.Body.UI
{
	// Token: 0x0200041C RID: 1052
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BodyScannerBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060019CA RID: 6602 RVA: 0x000021BC File Offset: 0x000003BC
		public BodyScannerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x00093E96 File Offset: 0x00092096
		protected override void Open()
		{
			base.Open();
			this._display = new BodyScannerDisplay(this);
			this._display.OnClose += base.Close;
			this._display.OpenCentered();
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x00093ECC File Offset: 0x000920CC
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			BodyScannerUIState bodyScannerUIState = state as BodyScannerUIState;
			if (bodyScannerUIState == null)
			{
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			if (!entityManager.EntityExists(bodyScannerUIState.Uid))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(65, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Received an invalid entity with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(bodyScannerUIState.Uid);
				defaultInterpolatedStringHandler.AppendLiteral(" for body scanner with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(base.Owner.Owner);
				defaultInterpolatedStringHandler.AppendLiteral(" at ");
				defaultInterpolatedStringHandler.AppendFormatted<MapCoordinates>(entityManager.GetComponent<TransformComponent>(base.Owner.Owner).MapPosition);
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			BodyScannerDisplay display = this._display;
			if (display == null)
			{
				return;
			}
			display.UpdateDisplay(bodyScannerUIState.Uid);
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x00093F8C File Offset: 0x0009218C
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				BodyScannerDisplay display = this._display;
				if (display == null)
				{
					return;
				}
				display.Dispose();
			}
		}

		// Token: 0x04000D13 RID: 3347
		[Nullable(2)]
		[ViewVariables]
		private BodyScannerDisplay _display;
	}
}
