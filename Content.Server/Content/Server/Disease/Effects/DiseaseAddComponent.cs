using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Disease.Effects
{
	// Token: 0x02000566 RID: 1382
	public sealed class DiseaseAddComponent : DiseaseEffect
	{
		// Token: 0x06001D4B RID: 7499 RVA: 0x0009C3C4 File Offset: 0x0009A5C4
		public override void Effect(DiseaseEffectArgs args)
		{
			if (this.Comp == null)
			{
				return;
			}
			EntityUid uid = args.DiseasedEntity;
			Component newComponent = (Component)IoCManager.Resolve<IComponentFactory>().GetComponent(this.Comp, false);
			newComponent.Owner = uid;
			if (!args.EntityManager.HasComponent(uid, newComponent.GetType()))
			{
				args.EntityManager.AddComponent<Component>(uid, newComponent, false);
			}
		}

		// Token: 0x040012B5 RID: 4789
		[Nullable(2)]
		[DataField("comp", false, 1, false, false, null)]
		public string Comp;
	}
}
