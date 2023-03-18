using System;
using System.Runtime.CompilerServices;
using Content.Shared.Beam;
using Content.Shared.Beam.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Beam
{
	// Token: 0x02000423 RID: 1059
	public sealed class BeamSystem : SharedBeamSystem
	{
		// Token: 0x060019E0 RID: 6624 RVA: 0x000944E7 File Offset: 0x000926E7
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<BeamVisualizerEvent>(new EntityEventHandler<BeamVisualizerEvent>(this.BeamVisualizerMessage), null, null);
		}

		// Token: 0x060019E1 RID: 6625 RVA: 0x00094504 File Offset: 0x00092704
		[NullableContext(1)]
		private void BeamVisualizerMessage(BeamVisualizerEvent args)
		{
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(args.Beam, ref spriteComponent))
			{
				spriteComponent.Rotation = args.UserAngle;
				if (args.BodyState != null)
				{
					spriteComponent.LayerSetState(0, args.BodyState);
					spriteComponent.LayerSetShader(0, args.Shader);
				}
			}
		}
	}
}
