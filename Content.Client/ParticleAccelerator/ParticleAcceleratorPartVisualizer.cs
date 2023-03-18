using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.ParticleAccelerator
{
	// Token: 0x020001CA RID: 458
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ParticleAcceleratorPartVisualizer : AppearanceVisualizer
	{
		// Token: 0x06000C0F RID: 3087 RVA: 0x00045C90 File Offset: 0x00043E90
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			base.InitializeEntity(entity);
			SpriteComponent spriteComponent;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(entity, ref spriteComponent))
			{
				throw new EntityCreationException("No sprite component found in entity that has ParticleAcceleratorPartVisualizer");
			}
			if (!spriteComponent.AllLayers.Any<ISpriteLayer>())
			{
				throw new EntityCreationException("No Layer set for entity that has ParticleAcceleratorPartVisualizer");
			}
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x00045CD8 File Offset: 0x00043ED8
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent spriteComponent;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				return;
			}
			ParticleAcceleratorVisualState particleAcceleratorVisualState;
			if (!component.TryGetData<ParticleAcceleratorVisualState>(ParticleAcceleratorVisuals.VisualState, ref particleAcceleratorVisualState))
			{
				particleAcceleratorVisualState = ParticleAcceleratorVisualState.Unpowered;
			}
			if (particleAcceleratorVisualState != ParticleAcceleratorVisualState.Unpowered)
			{
				spriteComponent.LayerSetVisible(ParticleAcceleratorVisualLayers.Unlit, true);
				spriteComponent.LayerSetState(ParticleAcceleratorVisualLayers.Unlit, this._baseState + ParticleAcceleratorPartVisualizer.StatesSuffixes[particleAcceleratorVisualState]);
				return;
			}
			spriteComponent.LayerSetVisible(ParticleAcceleratorVisualLayers.Unlit, false);
		}

		// Token: 0x040005C2 RID: 1474
		[DataField("baseState", false, 1, true, false, null)]
		private string _baseState;

		// Token: 0x040005C3 RID: 1475
		private static readonly Dictionary<ParticleAcceleratorVisualState, string> StatesSuffixes = new Dictionary<ParticleAcceleratorVisualState, string>
		{
			{
				ParticleAcceleratorVisualState.Powered,
				"p"
			},
			{
				ParticleAcceleratorVisualState.Level0,
				"p0"
			},
			{
				ParticleAcceleratorVisualState.Level1,
				"p1"
			},
			{
				ParticleAcceleratorVisualState.Level2,
				"p2"
			},
			{
				ParticleAcceleratorVisualState.Level3,
				"p3"
			}
		};
	}
}
