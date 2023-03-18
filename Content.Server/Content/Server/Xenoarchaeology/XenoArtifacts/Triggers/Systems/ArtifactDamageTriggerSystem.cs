using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x02000022 RID: 34
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactDamageTriggerSystem : EntitySystem
	{
		// Token: 0x06000083 RID: 131 RVA: 0x00004C79 File Offset: 0x00002E79
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ArtifactDamageTriggerComponent, DamageChangedEvent>(new ComponentEventHandler<ArtifactDamageTriggerComponent, DamageChangedEvent>(this.OnDamageChanged), null, null);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00004C90 File Offset: 0x00002E90
		private void OnDamageChanged(EntityUid uid, ArtifactDamageTriggerComponent component, DamageChangedEvent args)
		{
			if (!args.DamageIncreased)
			{
				return;
			}
			if (args.DamageDelta == null)
			{
				return;
			}
			foreach (KeyValuePair<string, FixedPoint2> keyValuePair in args.DamageDelta.DamageDict)
			{
				string text;
				FixedPoint2 fixedPoint;
				keyValuePair.Deconstruct(out text, out fixedPoint);
				string type = text;
				FixedPoint2 amount = fixedPoint;
				if (component.DamageTypes == null || component.DamageTypes.Contains(type))
				{
					component.AccumulatedDamage += (float)amount;
				}
			}
			if (component.AccumulatedDamage >= component.DamageThreshold)
			{
				this._artifact.TryActivateArtifact(uid, args.Origin, null);
			}
		}

		// Token: 0x04000063 RID: 99
		[Dependency]
		private readonly ArtifactSystem _artifact;
	}
}
