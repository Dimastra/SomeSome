using System;
using System.Runtime.CompilerServices;
using Content.Shared.Conveyor;
using Content.Shared.Recycling;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Recycling
{
	// Token: 0x02000175 RID: 373
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RecyclerVisualizer : AppearanceVisualizer
	{
		// Token: 0x060009C3 RID: 2499 RVA: 0x00038CBC File Offset: 0x00036EBC
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			base.InitializeEntity(entity);
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			SpriteComponent sprite;
			AppearanceComponent component;
			if (!entityManager.TryGetComponent<SpriteComponent>(entity, ref sprite) || !entityManager.TryGetComponent<AppearanceComponent>(entity, ref component))
			{
				return;
			}
			this.UpdateAppearance(component, sprite);
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x00038CF8 File Offset: 0x00036EF8
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent sprite;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(component.Owner, ref sprite))
			{
				return;
			}
			this.UpdateAppearance(component, sprite);
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x00038D2C File Offset: 0x00036F2C
		private void UpdateAppearance(AppearanceComponent component, SpriteComponent sprite)
		{
			string text = this._stateOff;
			ConveyorState conveyorState;
			if (component.TryGetData<ConveyorState>(ConveyorVisuals.State, ref conveyorState) && conveyorState != ConveyorState.Off)
			{
				text = this._stateOn;
			}
			bool flag;
			if (component.TryGetData<bool>(RecyclerVisuals.Bloody, ref flag) && flag)
			{
				text += "bld";
			}
			sprite.LayerSetState(RecyclerVisualLayers.Main, text);
		}

		// Token: 0x040004D8 RID: 1240
		[DataField("state_on", false, 1, false, false, null)]
		private string _stateOn = "grinder-o1";

		// Token: 0x040004D9 RID: 1241
		[DataField("state_off", false, 1, false, false, null)]
		private string _stateOff = "grinder-o0";
	}
}
