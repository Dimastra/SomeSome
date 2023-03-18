using System;
using System.Runtime.CompilerServices;
using Content.Server.Disease;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Traits.Assorted
{
	// Token: 0x02000108 RID: 264
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UncontrollableSnoughSystem : EntitySystem
	{
		// Token: 0x060004C1 RID: 1217 RVA: 0x00016ACB File Offset: 0x00014CCB
		public override void Initialize()
		{
			base.SubscribeLocalEvent<UncontrollableSnoughComponent, ComponentStartup>(new ComponentEventHandler<UncontrollableSnoughComponent, ComponentStartup>(this.SetupSnough), null, null);
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x00016AE1 File Offset: 0x00014CE1
		private void SetupSnough(EntityUid uid, UncontrollableSnoughComponent component, ComponentStartup args)
		{
			component.NextIncidentTime = this._random.NextFloat(component.TimeBetweenIncidents.X, component.TimeBetweenIncidents.Y);
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x00016B0C File Offset: 0x00014D0C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (UncontrollableSnoughComponent snough in base.EntityQuery<UncontrollableSnoughComponent>(false))
			{
				snough.NextIncidentTime -= frameTime;
				if (snough.NextIncidentTime < 0f)
				{
					snough.NextIncidentTime += this._random.NextFloat(snough.TimeBetweenIncidents.X, snough.TimeBetweenIncidents.Y);
					this._diseaseSystem.SneezeCough(snough.Owner, null, snough.SnoughMessage, snough.SnoughSound, false, null);
				}
			}
		}

		// Token: 0x040002C4 RID: 708
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040002C5 RID: 709
		[Dependency]
		private readonly DiseaseSystem _diseaseSystem;
	}
}
