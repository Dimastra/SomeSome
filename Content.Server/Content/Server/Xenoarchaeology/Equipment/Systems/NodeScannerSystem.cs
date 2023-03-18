using System;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Server.Xenoarchaeology.Equipment.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Xenoarchaeology.Equipment.Systems
{
	// Token: 0x02000062 RID: 98
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NodeScannerSystem : EntitySystem
	{
		// Token: 0x06000127 RID: 295 RVA: 0x00007BAB File Offset: 0x00005DAB
		public override void Initialize()
		{
			base.SubscribeLocalEvent<NodeScannerComponent, AfterInteractEvent>(new ComponentEventHandler<NodeScannerComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00007BC4 File Offset: 0x00005DC4
		private void OnAfterInteract(EntityUid uid, NodeScannerComponent component, AfterInteractEvent args)
		{
			if (!args.CanReach || args.Target == null)
			{
				return;
			}
			ArtifactComponent artifact;
			if (!base.TryComp<ArtifactComponent>(args.Target, ref artifact) || artifact.CurrentNode == null)
			{
				return;
			}
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			EntityUid target = args.Target.Value;
			this._useDelay.BeginDelay(uid, null);
			SharedPopupSystem popupSystem = this._popupSystem;
			string text = "node-scan-popup";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[1];
			int num = 0;
			string item = "id";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<int>(artifact.CurrentNode.Id);
			array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
			popupSystem.PopupEntity(Loc.GetString(text, array), target, PopupType.Small);
		}

		// Token: 0x040000F0 RID: 240
		[Dependency]
		private readonly UseDelaySystem _useDelay;

		// Token: 0x040000F1 RID: 241
		[Dependency]
		private readonly PopupSystem _popupSystem;
	}
}
