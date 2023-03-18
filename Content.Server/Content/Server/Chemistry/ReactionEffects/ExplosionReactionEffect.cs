using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.Explosion;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Chemistry.ReactionEffects
{
	// Token: 0x0200068D RID: 1677
	[DataDefinition]
	public sealed class ExplosionReactionEffect : ReagentEffect
	{
		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x060022CC RID: 8908 RVA: 0x000B51D3 File Offset: 0x000B33D3
		public override bool ShouldLog
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x060022CD RID: 8909 RVA: 0x000B51D6 File Offset: 0x000B33D6
		public override LogImpact LogImpact
		{
			get
			{
				return LogImpact.High;
			}
		}

		// Token: 0x060022CE RID: 8910 RVA: 0x000B51DC File Offset: 0x000B33DC
		public override void Effect(ReagentEffectArgs args)
		{
			float intensity = MathF.Min((float)args.Quantity * this.IntensityPerUnit, this.MaxTotalIntensity);
			EntitySystem.Get<ExplosionSystem>().QueueExplosion(args.SolutionEntity, this.ExplosionType, intensity, this.IntensitySlope, this.MaxIntensity, 1f, int.MaxValue, true, false, null, true);
		}

		// Token: 0x0400158E RID: 5518
		[Nullable(1)]
		[DataField("explosionType", false, 1, true, false, typeof(PrototypeIdSerializer<ExplosionPrototype>))]
		[JsonIgnore]
		public string ExplosionType;

		// Token: 0x0400158F RID: 5519
		[DataField("maxIntensity", false, 1, false, false, null)]
		[JsonIgnore]
		public float MaxIntensity = 5f;

		// Token: 0x04001590 RID: 5520
		[DataField("intensitySlope", false, 1, false, false, null)]
		[JsonIgnore]
		public float IntensitySlope = 1f;

		// Token: 0x04001591 RID: 5521
		[DataField("maxTotalIntensity", false, 1, false, false, null)]
		[JsonIgnore]
		public float MaxTotalIntensity = 100f;

		// Token: 0x04001592 RID: 5522
		[DataField("intensityPerUnit", false, 1, false, false, null)]
		[JsonIgnore]
		public float IntensityPerUnit = 1f;
	}
}
