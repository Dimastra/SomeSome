using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Piping.Components;
using Content.Shared.Atmos.Piping;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Atmos.Piping.EntitySystems
{
	// Token: 0x02000760 RID: 1888
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosPipeColorSystem : EntitySystem
	{
		// Token: 0x06002802 RID: 10242 RVA: 0x000D1B44 File Offset: 0x000CFD44
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AtmosPipeColorComponent, ComponentStartup>(new ComponentEventHandler<AtmosPipeColorComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<AtmosPipeColorComponent, ComponentShutdown>(new ComponentEventHandler<AtmosPipeColorComponent, ComponentShutdown>(this.OnShutdown), null, null);
		}

		// Token: 0x06002803 RID: 10243 RVA: 0x000D1B74 File Offset: 0x000CFD74
		private void OnStartup(EntityUid uid, AtmosPipeColorComponent component, ComponentStartup args)
		{
			AppearanceComponent appearance;
			if (!this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, PipeColorVisuals.Color, component.Color, appearance);
		}

		// Token: 0x06002804 RID: 10244 RVA: 0x000D1BB0 File Offset: 0x000CFDB0
		private void OnShutdown(EntityUid uid, AtmosPipeColorComponent component, ComponentShutdown args)
		{
			AppearanceComponent appearance;
			if (!this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, PipeColorVisuals.Color, Color.White, appearance);
		}

		// Token: 0x06002805 RID: 10245 RVA: 0x000D1BEC File Offset: 0x000CFDEC
		public void SetColor(EntityUid uid, AtmosPipeColorComponent component, Color color)
		{
			component.Color = color;
			AppearanceComponent appearance;
			if (!this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, PipeColorVisuals.Color, color, appearance);
		}

		// Token: 0x040018EB RID: 6379
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
