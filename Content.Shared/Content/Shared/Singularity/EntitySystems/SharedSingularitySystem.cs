using System;
using System.Runtime.CompilerServices;
using Content.Shared.Radiation.Components;
using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.EntitySystems
{
	// Token: 0x0200019F RID: 415
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedSingularitySystem : EntitySystem
	{
		// Token: 0x060004E6 RID: 1254 RVA: 0x00012CAC File Offset: 0x00010EAC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SingularityComponent, ComponentStartup>(new ComponentEventHandler<SingularityComponent, ComponentStartup>(this.OnSingularityStartup), null, null);
			base.SubscribeLocalEvent<AppearanceComponent, SingularityLevelChangedEvent>(new ComponentEventHandler<AppearanceComponent, SingularityLevelChangedEvent>(this.UpdateAppearance), null, null);
			base.SubscribeLocalEvent<RadiationSourceComponent, SingularityLevelChangedEvent>(new ComponentEventHandler<RadiationSourceComponent, SingularityLevelChangedEvent>(this.UpdateRadiation), null, null);
			base.SubscribeLocalEvent<PhysicsComponent, SingularityLevelChangedEvent>(new ComponentEventHandler<PhysicsComponent, SingularityLevelChangedEvent>(this.UpdateBody), null, null);
			base.SubscribeLocalEvent<EventHorizonComponent, SingularityLevelChangedEvent>(new ComponentEventHandler<EventHorizonComponent, SingularityLevelChangedEvent>(this.UpdateEventHorizon), null, null);
			base.SubscribeLocalEvent<SingularityDistortionComponent, SingularityLevelChangedEvent>(new ComponentEventHandler<SingularityDistortionComponent, SingularityLevelChangedEvent>(this.UpdateDistortion), null, null);
			base.SubscribeLocalEvent<SingularityDistortionComponent, EntGotInsertedIntoContainerMessage>(new ComponentEventHandler<SingularityDistortionComponent, EntGotInsertedIntoContainerMessage>(this.UpdateDistortion), null, null);
			base.SubscribeLocalEvent<SingularityDistortionComponent, EntGotRemovedFromContainerMessage>(new ComponentEventHandler<SingularityDistortionComponent, EntGotRemovedFromContainerMessage>(this.UpdateDistortion), null, null);
			ViewVariablesTypeHandler<SingularityComponent> typeHandler = this.Vvm.GetTypeHandler<SingularityComponent>();
			typeHandler.AddPath<byte>("Level", (EntityUid _, SingularityComponent comp) => comp.Level, new ComponentPropertySetter<SingularityComponent, byte>(this.SetLevel));
			typeHandler.AddPath<float>("RadsPerLevel", (EntityUid _, SingularityComponent comp) => comp.RadsPerLevel, new ComponentPropertySetter<SingularityComponent, float>(this.SetRadsPerLevel));
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x00012DD8 File Offset: 0x00010FD8
		public override void Shutdown()
		{
			ViewVariablesTypeHandler<SingularityComponent> typeHandler = this.Vvm.GetTypeHandler<SingularityComponent>();
			typeHandler.RemovePath("Level");
			typeHandler.RemovePath("RadsPerLevel");
			base.Shutdown();
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x00012E04 File Offset: 0x00011004
		[NullableContext(2)]
		public void SetLevel(EntityUid uid, byte value, SingularityComponent singularity = null)
		{
			if (!base.Resolve<SingularityComponent>(uid, ref singularity, true))
			{
				return;
			}
			value = MathHelper.Clamp(value, 0, 6);
			byte oldValue = singularity.Level;
			if (oldValue == value)
			{
				return;
			}
			singularity.Level = value;
			this.UpdateSingularityLevel(uid, oldValue, singularity);
			if (!this.EntityManager.Deleted(singularity.Owner))
			{
				this.EntityManager.Dirty(singularity, null);
			}
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00012E64 File Offset: 0x00011064
		[NullableContext(2)]
		public void SetRadsPerLevel(EntityUid uid, float value, SingularityComponent singularity = null)
		{
			if (!base.Resolve<SingularityComponent>(uid, ref singularity, true))
			{
				return;
			}
			if (singularity.RadsPerLevel == value)
			{
				return;
			}
			singularity.RadsPerLevel = value;
			this.UpdateRadiation(uid, singularity, null);
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x00012E8D File Offset: 0x0001108D
		[NullableContext(2)]
		public void UpdateSingularityLevel(EntityUid uid, byte oldValue, SingularityComponent singularity = null)
		{
			if (!base.Resolve<SingularityComponent>(uid, ref singularity, true))
			{
				return;
			}
			base.RaiseLocalEvent<SingularityLevelChangedEvent>(uid, new SingularityLevelChangedEvent(singularity.Level, oldValue, singularity), false);
			if (singularity.Level <= 0)
			{
				this.EntityManager.DeleteEntity(singularity.Owner);
			}
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x00012ECB File Offset: 0x000110CB
		[NullableContext(2)]
		public void UpdateSingularityLevel(EntityUid uid, SingularityComponent singularity = null)
		{
			if (base.Resolve<SingularityComponent>(uid, ref singularity, true))
			{
				this.UpdateSingularityLevel(uid, singularity.Level, singularity);
			}
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x00012EE7 File Offset: 0x000110E7
		[NullableContext(2)]
		private void UpdateRadiation(EntityUid uid, SingularityComponent singularity = null, RadiationSourceComponent rads = null)
		{
			if (!base.Resolve<SingularityComponent, RadiationSourceComponent>(uid, ref singularity, ref rads, false))
			{
				return;
			}
			rads.Intensity = (float)singularity.Level * singularity.RadsPerLevel;
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x00012F0C File Offset: 0x0001110C
		public float GravPulseRange(SingularityComponent singulo)
		{
			return 2f * (float)(singulo.Level + 1);
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x00012F1D File Offset: 0x0001111D
		[NullableContext(0)]
		public ValueTuple<float, float> GravPulseAcceleration([Nullable(1)] SingularityComponent singulo)
		{
			return new ValueTuple<float, float>(10f * (float)singulo.Level, 0f);
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x00012F36 File Offset: 0x00011136
		public float EventHorizonRadius(SingularityComponent singulo)
		{
			return (float)singulo.Level - 0.5f;
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x00012F45 File Offset: 0x00011145
		public bool CanBreachContainment(SingularityComponent singulo)
		{
			return singulo.Level >= 5;
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x00012F54 File Offset: 0x00011154
		public float GetFalloff(float level)
		{
			float result;
			if (level != 0f)
			{
				if (level != 1f)
				{
					if (level != 2f)
					{
						if (level != 3f)
						{
							if (level != 4f)
							{
								if (level != 5f)
								{
									if (level != 6f)
									{
										result = -1f;
									}
									else
									{
										result = MathF.Sqrt(12f);
									}
								}
								else
								{
									result = MathF.Sqrt(12f);
								}
							}
							else
							{
								result = MathF.Sqrt(10f);
							}
						}
						else
						{
							result = MathF.Sqrt(8f);
						}
					}
					else
					{
						result = MathF.Sqrt(7f);
					}
				}
				else
				{
					result = MathF.Sqrt(6.4f);
				}
			}
			else
			{
				result = 9999f;
			}
			return result;
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x00012FF8 File Offset: 0x000111F8
		public float GetIntensity(float level)
		{
			float result;
			if (level != 0f)
			{
				if (level != 1f)
				{
					if (level != 2f)
					{
						if (level != 3f)
						{
							if (level != 4f)
							{
								if (level != 5f)
								{
									if (level != 6f)
									{
										result = -1f;
									}
									else
									{
										result = 180000000f;
									}
								}
								else
								{
									result = 180000000f;
								}
							}
							else
							{
								result = 16200000f;
							}
						}
						else
						{
							result = 1113920f;
						}
					}
					else
					{
						result = 103680f;
					}
				}
				else
				{
					result = 3645f;
				}
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x0001307E File Offset: 0x0001127E
		protected virtual void OnSingularityStartup(EntityUid uid, SingularityComponent comp, ComponentStartup args)
		{
			this.UpdateSingularityLevel(uid, comp);
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00013088 File Offset: 0x00011288
		private void UpdateEventHorizon(EntityUid uid, EventHorizonComponent comp, SingularityLevelChangedEvent args)
		{
			SingularityComponent singulo = args.Singularity;
			this._horizons.SetRadius(uid, this.EventHorizonRadius(singulo), false, comp);
			this._horizons.SetCanBreachContainment(uid, this.CanBreachContainment(singulo), false, comp);
			this._horizons.UpdateEventHorizonFixture(uid, null, comp);
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x000130D4 File Offset: 0x000112D4
		private void UpdateDistortion(EntityUid uid, SingularityDistortionComponent comp, SingularityLevelChangedEvent args)
		{
			float newFalloffPower = this.GetFalloff((float)args.NewValue);
			float newIntensity = this.GetIntensity((float)args.NewValue);
			if (this._containers.IsEntityInContainer(uid, null))
			{
				float absFalloffPower = MathF.Abs(newFalloffPower);
				float absIntensity = MathF.Abs(newIntensity);
				float factor = -0.75f;
				newFalloffPower = ((absFalloffPower > 1f) ? (newFalloffPower * MathF.Pow(absFalloffPower, factor)) : newFalloffPower);
				newIntensity = ((absIntensity > 1f) ? (newIntensity * MathF.Pow(absIntensity, factor)) : newIntensity);
			}
			comp.FalloffPower = newFalloffPower;
			comp.Intensity = newIntensity;
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x0001315C File Offset: 0x0001135C
		private void UpdateDistortion(EntityUid uid, SingularityDistortionComponent comp, EntGotInsertedIntoContainerMessage args)
		{
			float absFalloffPower = MathF.Abs(comp.FalloffPower);
			float absIntensity = MathF.Abs(comp.Intensity);
			float factor = -0.75f;
			comp.FalloffPower = ((absFalloffPower > 1f) ? (comp.FalloffPower * MathF.Pow(absFalloffPower, factor)) : comp.FalloffPower);
			comp.Intensity = ((absIntensity > 1f) ? (comp.Intensity * MathF.Pow(absIntensity, factor)) : comp.Intensity);
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x000131D0 File Offset: 0x000113D0
		private void UpdateDistortion(EntityUid uid, SingularityDistortionComponent comp, EntGotRemovedFromContainerMessage args)
		{
			float absFalloffPower = MathF.Abs(comp.FalloffPower);
			float absIntensity = MathF.Abs(comp.Intensity);
			float factor = 3f;
			comp.FalloffPower = ((absFalloffPower > 1f) ? (comp.FalloffPower * MathF.Pow(absFalloffPower, factor)) : comp.FalloffPower);
			comp.Intensity = ((absIntensity > 1f) ? (comp.Intensity * MathF.Pow(absIntensity, factor)) : comp.Intensity);
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x00013244 File Offset: 0x00011444
		private void UpdateBody(EntityUid uid, PhysicsComponent comp, SingularityLevelChangedEvent args)
		{
			this._physics.SetBodyStatus(comp, (args.NewValue > 1) ? 1 : 0, true);
			if (args.NewValue <= 1 && args.OldValue > 1)
			{
				this._physics.SetLinearVelocity(uid, Vector2.Zero, true, true, null, comp);
			}
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x00013292 File Offset: 0x00011492
		private void UpdateAppearance(EntityUid uid, AppearanceComponent comp, SingularityLevelChangedEvent args)
		{
			this._visualizer.SetData(uid, SingularityVisuals.Level, args.NewValue, comp);
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x000132B2 File Offset: 0x000114B2
		private void UpdateRadiation(EntityUid uid, RadiationSourceComponent comp, SingularityLevelChangedEvent args)
		{
			this.UpdateRadiation(uid, args.Singularity, comp);
		}

		// Token: 0x04000484 RID: 1156
		[Dependency]
		private readonly SharedAppearanceSystem _visualizer;

		// Token: 0x04000485 RID: 1157
		[Dependency]
		private readonly SharedContainerSystem _containers;

		// Token: 0x04000486 RID: 1158
		[Dependency]
		private readonly SharedEventHorizonSystem _horizons;

		// Token: 0x04000487 RID: 1159
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000488 RID: 1160
		[Dependency]
		protected readonly IViewVariablesManager Vvm;

		// Token: 0x04000489 RID: 1161
		public const byte MinSingularityLevel = 0;

		// Token: 0x0400048A RID: 1162
		public const byte MaxSingularityLevel = 6;

		// Token: 0x0400048B RID: 1163
		public const float DistortionContainerScaling = 4f;

		// Token: 0x0400048C RID: 1164
		public const float BaseGravityWellRadius = 2f;

		// Token: 0x0400048D RID: 1165
		public const float BaseGravityWellAcceleration = 10f;

		// Token: 0x0400048E RID: 1166
		public const byte SingularityBreachThreshold = 5;

		// Token: 0x020007A9 RID: 1961
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class SingularityComponentState : ComponentState
		{
			// Token: 0x060017F4 RID: 6132 RVA: 0x0004D36A File Offset: 0x0004B56A
			[NullableContext(1)]
			public SingularityComponentState(SingularityComponent singulo)
			{
				this.Level = singulo.Level;
			}

			// Token: 0x040017CC RID: 6092
			public readonly byte Level;
		}
	}
}
