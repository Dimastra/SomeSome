using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Radiation.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Radiation.Overlays
{
	// Token: 0x0200017C RID: 380
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadiationPulseOverlay : Overlay
	{
		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x060009DE RID: 2526 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x060009DF RID: 2527 RVA: 0x00003C56 File Offset: 0x00001E56
		public override bool RequestScreenTexture
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x00039804 File Offset: 0x00037A04
		public RadiationPulseOverlay()
		{
			IoCManager.InjectDependencies<RadiationPulseOverlay>(this);
			this._baseShader = this._prototypeManager.Index<ShaderPrototype>("Radiation").Instance().Duplicate();
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x0003983E File Offset: 0x00037A3E
		protected override bool BeforeDraw(in OverlayDrawArgs args)
		{
			this.RadiationQuery(args.Viewport.Eye);
			return this._pulses.Count > 0;
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x00039860 File Offset: 0x00037A60
		protected override void Draw(in OverlayDrawArgs args)
		{
			if (this.ScreenTexture == null)
			{
				return;
			}
			DrawingHandleWorld worldHandle = args.WorldHandle;
			IClydeViewport viewport = args.Viewport;
			foreach (ValueTuple<ShaderInstance, RadiationPulseOverlay.RadiationShaderInstance> valueTuple in this._pulses.Values)
			{
				ShaderInstance item = valueTuple.Item1;
				RadiationPulseOverlay.RadiationShaderInstance item2 = valueTuple.Item2;
				if (!(item2.CurrentMapCoords.MapId != args.MapId))
				{
					Vector2 vector = viewport.WorldToLocal(item2.CurrentMapCoords.Position);
					vector.Y = (float)viewport.Size.Y - vector.Y;
					if (item != null)
					{
						item.SetParameter("renderScale", viewport.RenderScale);
					}
					if (item != null)
					{
						item.SetParameter("positionInput", vector);
					}
					if (item != null)
					{
						item.SetParameter("range", item2.Range);
					}
					double num = (this._gameTiming.RealTime - item2.Start).TotalSeconds / (double)item2.Duration;
					if (item != null)
					{
						item.SetParameter("life", (float)num);
					}
					if (item != null)
					{
						item.SetParameter("SCREEN_TEXTURE", viewport.RenderTarget.Texture);
					}
					worldHandle.UseShader(item);
					worldHandle.DrawRect(Box2.CenteredAround(item2.CurrentMapCoords.Position, new Vector2(item2.Range, item2.Range) * 2f), Color.White, true);
				}
			}
			worldHandle.UseShader(null);
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x00039A08 File Offset: 0x00037C08
		[NullableContext(2)]
		private void RadiationQuery(IEye currentEye)
		{
			if (currentEye == null)
			{
				this._pulses.Clear();
				return;
			}
			MapCoordinates position = currentEye.Position;
			foreach (RadiationPulseComponent radiationPulseComponent in this._entityManager.EntityQuery<RadiationPulseComponent>(false))
			{
				EntityUid owner = radiationPulseComponent.Owner;
				if (!this._pulses.ContainsKey(owner) && this.PulseQualifies(owner, position))
				{
					this._pulses.Add(owner, new ValueTuple<ShaderInstance, RadiationPulseOverlay.RadiationShaderInstance>(this._baseShader.Duplicate(), new RadiationPulseOverlay.RadiationShaderInstance(this._entityManager.GetComponent<TransformComponent>(owner).MapPosition, radiationPulseComponent.VisualRange, radiationPulseComponent.StartTime, radiationPulseComponent.VisualDuration)));
				}
			}
			foreach (EntityUid entityUid in this._pulses.Keys)
			{
				RadiationPulseComponent radiationPulseComponent2;
				if (this._entityManager.EntityExists(entityUid) && this.PulseQualifies(entityUid, position) && this._entityManager.TryGetComponent<RadiationPulseComponent>(entityUid, ref radiationPulseComponent2))
				{
					ValueTuple<ShaderInstance, RadiationPulseOverlay.RadiationShaderInstance> valueTuple = this._pulses[entityUid];
					valueTuple.Item2.CurrentMapCoords = this._entityManager.GetComponent<TransformComponent>(entityUid).MapPosition;
					valueTuple.Item2.Range = radiationPulseComponent2.VisualRange;
				}
				else
				{
					this._pulses[entityUid].Item1.Dispose();
					this._pulses.Remove(entityUid);
				}
			}
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x00039BA4 File Offset: 0x00037DA4
		private bool PulseQualifies(EntityUid pulseEntity, MapCoordinates currentEyeLoc)
		{
			return this._entityManager.GetComponent<TransformComponent>(pulseEntity).MapID == currentEyeLoc.MapId && this._entityManager.GetComponent<TransformComponent>(pulseEntity).Coordinates.InRange(this._entityManager, EntityCoordinates.FromMap(this._entityManager, this._entityManager.GetComponent<TransformComponent>(pulseEntity).ParentUid, currentEyeLoc), 15f);
		}

		// Token: 0x040004E7 RID: 1255
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x040004E8 RID: 1256
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040004E9 RID: 1257
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040004EA RID: 1258
		private const float MaxDist = 15f;

		// Token: 0x040004EB RID: 1259
		private readonly ShaderInstance _baseShader;

		// Token: 0x040004EC RID: 1260
		[TupleElementNames(new string[]
		{
			"shd",
			"instance"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		private readonly Dictionary<EntityUid, ValueTuple<ShaderInstance, RadiationPulseOverlay.RadiationShaderInstance>> _pulses = new Dictionary<EntityUid, ValueTuple<ShaderInstance, RadiationPulseOverlay.RadiationShaderInstance>>();

		// Token: 0x0200017D RID: 381
		[Nullable(0)]
		private sealed class RadiationShaderInstance : IEquatable<RadiationPulseOverlay.RadiationShaderInstance>
		{
			// Token: 0x060009E5 RID: 2533 RVA: 0x00039C12 File Offset: 0x00037E12
			public RadiationShaderInstance(MapCoordinates CurrentMapCoords, float Range, TimeSpan Start, float Duration)
			{
				this.CurrentMapCoords = CurrentMapCoords;
				this.Range = Range;
				this.Start = Start;
				this.Duration = Duration;
				base..ctor();
			}

			// Token: 0x170001CB RID: 459
			// (get) Token: 0x060009E6 RID: 2534 RVA: 0x00039C37 File Offset: 0x00037E37
			[CompilerGenerated]
			private Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(RadiationPulseOverlay.RadiationShaderInstance);
				}
			}

			// Token: 0x060009E7 RID: 2535 RVA: 0x00039C44 File Offset: 0x00037E44
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("RadiationShaderInstance");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060009E8 RID: 2536 RVA: 0x00039C90 File Offset: 0x00037E90
			[CompilerGenerated]
			private bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("CurrentMapCoords = ");
				builder.Append(this.CurrentMapCoords.ToString());
				builder.Append(", Range = ");
				builder.Append(this.Range.ToString());
				builder.Append(", Start = ");
				builder.Append(this.Start.ToString());
				builder.Append(", Duration = ");
				builder.Append(this.Duration.ToString());
				return true;
			}

			// Token: 0x060009E9 RID: 2537 RVA: 0x00039D33 File Offset: 0x00037F33
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(RadiationPulseOverlay.RadiationShaderInstance left, RadiationPulseOverlay.RadiationShaderInstance right)
			{
				return !(left == right);
			}

			// Token: 0x060009EA RID: 2538 RVA: 0x00039D3F File Offset: 0x00037F3F
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(RadiationPulseOverlay.RadiationShaderInstance left, RadiationPulseOverlay.RadiationShaderInstance right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			// Token: 0x060009EB RID: 2539 RVA: 0x00039D54 File Offset: 0x00037F54
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<MapCoordinates>.Default.GetHashCode(this.CurrentMapCoords)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Range)) * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(this.Start)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Duration);
			}

			// Token: 0x060009EC RID: 2540 RVA: 0x00039DCD File Offset: 0x00037FCD
			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as RadiationPulseOverlay.RadiationShaderInstance);
			}

			// Token: 0x060009ED RID: 2541 RVA: 0x00039DDC File Offset: 0x00037FDC
			[NullableContext(2)]
			[CompilerGenerated]
			public bool Equals(RadiationPulseOverlay.RadiationShaderInstance other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<MapCoordinates>.Default.Equals(this.CurrentMapCoords, other.CurrentMapCoords) && EqualityComparer<float>.Default.Equals(this.Range, other.Range) && EqualityComparer<TimeSpan>.Default.Equals(this.Start, other.Start) && EqualityComparer<float>.Default.Equals(this.Duration, other.Duration));
			}

			// Token: 0x060009EF RID: 2543 RVA: 0x00039E6D File Offset: 0x0003806D
			[CompilerGenerated]
			private RadiationShaderInstance(RadiationPulseOverlay.RadiationShaderInstance original)
			{
				this.CurrentMapCoords = original.CurrentMapCoords;
				this.Range = original.Range;
				this.Start = original.Start;
				this.Duration = original.Duration;
			}

			// Token: 0x060009F0 RID: 2544 RVA: 0x00039EA5 File Offset: 0x000380A5
			[CompilerGenerated]
			public void Deconstruct(out MapCoordinates CurrentMapCoords, out float Range, out TimeSpan Start, out float Duration)
			{
				CurrentMapCoords = this.CurrentMapCoords;
				Range = this.Range;
				Start = this.Start;
				Duration = this.Duration;
			}

			// Token: 0x040004ED RID: 1261
			public MapCoordinates CurrentMapCoords;

			// Token: 0x040004EE RID: 1262
			public float Range;

			// Token: 0x040004EF RID: 1263
			public TimeSpan Start;

			// Token: 0x040004F0 RID: 1264
			public float Duration;
		}
	}
}
