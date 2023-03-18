using System;
using System.Runtime.CompilerServices;
using Content.Client.GPS.Components;
using Content.Client.Message;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Client.GPS.UI
{
	// Token: 0x020002FD RID: 765
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HandheldGpsStatusControl : Control
	{
		// Token: 0x06001335 RID: 4917 RVA: 0x000726D8 File Offset: 0x000708D8
		public HandheldGpsStatusControl(HandheldGPSComponent parent)
		{
			this._parent = parent;
			this._entMan = IoCManager.Resolve<IEntityManager>();
			this._label = new RichTextLabel
			{
				StyleClasses = 
				{
					"ItemStatus"
				}
			};
			base.AddChild(this._label);
			this.UpdateGpsDetails();
		}

		// Token: 0x06001336 RID: 4918 RVA: 0x0007272C File Offset: 0x0007092C
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			this._updateDif += args.DeltaSeconds;
			if (this._updateDif < this._parent.UpdateRate)
			{
				return;
			}
			this._updateDif -= this._parent.UpdateRate;
			this.UpdateGpsDetails();
		}

		// Token: 0x06001337 RID: 4919 RVA: 0x00072788 File Offset: 0x00070988
		private void UpdateGpsDetails()
		{
			string item = "Error";
			TransformComponent transformComponent;
			if (this._entMan.TryGetComponent<TransformComponent>(this._parent.Owner, ref transformComponent))
			{
				MapCoordinates mapPosition = transformComponent.MapPosition;
				int value = (int)mapPosition.X;
				int value2 = (int)mapPosition.Y;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
				defaultInterpolatedStringHandler.AppendLiteral("(");
				defaultInterpolatedStringHandler.AppendFormatted<int>(value);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(value2);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				item = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			this._label.SetMarkup(Loc.GetString("handheld-gps-coordinates-title", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("coordinates", item)
			}));
		}

		// Token: 0x0400099A RID: 2458
		private readonly HandheldGPSComponent _parent;

		// Token: 0x0400099B RID: 2459
		private readonly RichTextLabel _label;

		// Token: 0x0400099C RID: 2460
		private float _updateDif;

		// Token: 0x0400099D RID: 2461
		private readonly IEntityManager _entMan;
	}
}
