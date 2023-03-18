using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Temperature.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Temperature;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Temperature.Systems
{
	// Token: 0x02000124 RID: 292
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TemperatureSystem : EntitySystem
	{
		// Token: 0x06000542 RID: 1346 RVA: 0x00019998 File Offset: 0x00017B98
		public override void Initialize()
		{
			base.SubscribeLocalEvent<TemperatureComponent, OnTemperatureChangeEvent>(new ComponentEventHandler<TemperatureComponent, OnTemperatureChangeEvent>(this.EnqueueDamage), null, null);
			base.SubscribeLocalEvent<TemperatureComponent, AtmosExposedUpdateEvent>(new ComponentEventRefHandler<TemperatureComponent, AtmosExposedUpdateEvent>(this.OnAtmosExposedUpdate), null, null);
			base.SubscribeLocalEvent<AlertsComponent, OnTemperatureChangeEvent>(new ComponentEventHandler<AlertsComponent, OnTemperatureChangeEvent>(this.ServerAlert), null, null);
			base.SubscribeLocalEvent<TemperatureProtectionComponent, InventoryRelayedEvent<ModifyChangedTemperatureEvent>>(new ComponentEventHandler<TemperatureProtectionComponent, InventoryRelayedEvent<ModifyChangedTemperatureEvent>>(this.OnTemperatureChangeAttempt), null, null);
			base.SubscribeLocalEvent<TemperatureComponent, EntParentChangedMessage>(new ComponentEventRefHandler<TemperatureComponent, EntParentChangedMessage>(this.OnParentChange), null, null);
			base.SubscribeLocalEvent<ContainerTemperatureDamageThresholdsComponent, ComponentStartup>(new ComponentEventHandler<ContainerTemperatureDamageThresholdsComponent, ComponentStartup>(this.OnParentThresholdStartup), null, null);
			base.SubscribeLocalEvent<ContainerTemperatureDamageThresholdsComponent, ComponentShutdown>(new ComponentEventHandler<ContainerTemperatureDamageThresholdsComponent, ComponentShutdown>(this.OnParentThresholdShutdown), null, null);
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x00019A34 File Offset: 0x00017C34
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this._accumulatedFrametime += frameTime;
			if (this._accumulatedFrametime < this.UpdateInterval)
			{
				return;
			}
			this._accumulatedFrametime -= this.UpdateInterval;
			if (!this.ShouldUpdateDamage.Any<TemperatureComponent>())
			{
				return;
			}
			foreach (TemperatureComponent comp in this.ShouldUpdateDamage)
			{
				MetaDataComponent metaData = null;
				if (!base.Deleted(comp.Owner, metaData) && !base.Paused(comp.Owner, metaData))
				{
					this.ChangeDamage(comp.Owner, comp);
				}
			}
			this.ShouldUpdateDamage.Clear();
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x00019B00 File Offset: 0x00017D00
		[NullableContext(2)]
		public void ForceChangeTemperature(EntityUid uid, float temp, TemperatureComponent temperature = null)
		{
			if (base.Resolve<TemperatureComponent>(uid, ref temperature, true))
			{
				float lastTemp = temperature.CurrentTemperature;
				float delta = temperature.CurrentTemperature - temp;
				temperature.CurrentTemperature = temp;
				base.RaiseLocalEvent<OnTemperatureChangeEvent>(uid, new OnTemperatureChangeEvent(temperature.CurrentTemperature, lastTemp, delta), true);
			}
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x00019B48 File Offset: 0x00017D48
		[NullableContext(2)]
		public void ChangeHeat(EntityUid uid, float heatAmount, bool ignoreHeatResistance = false, TemperatureComponent temperature = null)
		{
			if (base.Resolve<TemperatureComponent>(uid, ref temperature, true))
			{
				if (!ignoreHeatResistance)
				{
					ModifyChangedTemperatureEvent ev = new ModifyChangedTemperatureEvent(heatAmount);
					base.RaiseLocalEvent<ModifyChangedTemperatureEvent>(uid, ev, false);
					heatAmount = ev.TemperatureDelta;
				}
				float lastTemp = temperature.CurrentTemperature;
				temperature.CurrentTemperature += heatAmount / temperature.HeatCapacity;
				float delta = temperature.CurrentTemperature - lastTemp;
				base.RaiseLocalEvent<OnTemperatureChangeEvent>(uid, new OnTemperatureChangeEvent(temperature.CurrentTemperature, lastTemp, delta), true);
			}
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x00019BBC File Offset: 0x00017DBC
		private void OnAtmosExposedUpdate(EntityUid uid, TemperatureComponent temperature, ref AtmosExposedUpdateEvent args)
		{
			TransformComponent transform = args.Transform;
			if (transform.MapUid == null)
			{
				return;
			}
			Vector2i position = this._transformSystem.GetGridOrMapTilePosition(uid, transform);
			float num = args.GasMixture.Temperature - temperature.CurrentTemperature;
			float tileHeatCapacity = this._atmosphereSystem.GetTileHeatCapacity(transform.GridUid, transform.MapUid.Value, position);
			float heat = num * (tileHeatCapacity * temperature.HeatCapacity / (tileHeatCapacity + temperature.HeatCapacity));
			this.ChangeHeat(uid, heat * temperature.AtmosTemperatureTransferEfficiency, false, temperature);
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x00019C48 File Offset: 0x00017E48
		private void ServerAlert(EntityUid uid, AlertsComponent status, OnTemperatureChangeEvent args)
		{
			float currentTemperature = args.CurrentTemperature;
			if (currentTemperature <= 260f)
			{
				this._alertsSystem.ShowAlert(uid, AlertType.Cold, new short?((short)3), null);
				return;
			}
			if (currentTemperature <= 280f)
			{
				this._alertsSystem.ShowAlert(uid, AlertType.Cold, new short?((short)2), null);
				return;
			}
			if (currentTemperature <= 292f)
			{
				this._alertsSystem.ShowAlert(uid, AlertType.Cold, new short?((short)1), null);
				return;
			}
			if (currentTemperature <= 327f)
			{
				this._alertsSystem.ClearAlertCategory(uid, AlertCategory.Temperature);
				return;
			}
			if (currentTemperature <= 335f)
			{
				this._alertsSystem.ShowAlert(uid, AlertType.Hot, new short?((short)1), null);
				return;
			}
			if (currentTemperature <= 360f)
			{
				this._alertsSystem.ShowAlert(uid, AlertType.Hot, new short?((short)2), null);
				return;
			}
			if (currentTemperature <= 360f)
			{
				return;
			}
			this._alertsSystem.ShowAlert(uid, AlertType.Hot, new short?((short)3), null);
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x00019D5C File Offset: 0x00017F5C
		private void EnqueueDamage(EntityUid uid, TemperatureComponent component, OnTemperatureChangeEvent args)
		{
			this.ShouldUpdateDamage.Add(component);
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x00019D6C File Offset: 0x00017F6C
		private void ChangeDamage(EntityUid uid, TemperatureComponent temperature)
		{
			if (!this.EntityManager.HasComponent<DamageableComponent>(uid))
			{
				return;
			}
			double heatK = 0.005;
			int a = 1;
			FixedPoint2 y = temperature.DamageCap;
			FixedPoint2 c = y * 2;
			float heatDamageThreshold = temperature.ParentHeatDamageThreshold ?? temperature.HeatDamageThreshold;
			float coldDamageThreshold = temperature.ParentColdDamageThreshold ?? temperature.ColdDamageThreshold;
			if (temperature.CurrentTemperature >= heatDamageThreshold)
			{
				if (!temperature.TakingDamage)
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Temperature;
					LogStringHandler logStringHandler = new LogStringHandler(39, 1);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(temperature.Owner), "entity", "ToPrettyString(temperature.Owner)");
					logStringHandler.AppendLiteral(" started taking high temperature damage");
					adminLogger.Add(type, ref logStringHandler);
					temperature.TakingDamage = true;
				}
				float diff = Math.Abs(temperature.CurrentTemperature - heatDamageThreshold);
				FixedPoint2 tempDamage = c / (1.0 + (double)a * Math.Pow(2.718281828459045, -heatK * (double)diff)) - y;
				this._damageableSystem.TryChangeDamage(new EntityUid?(uid), temperature.HeatDamage * tempDamage, false, false, null, null);
				return;
			}
			if (temperature.CurrentTemperature <= coldDamageThreshold)
			{
				if (!temperature.TakingDamage)
				{
					ISharedAdminLogManager adminLogger2 = this._adminLogger;
					LogType type2 = LogType.Temperature;
					LogStringHandler logStringHandler = new LogStringHandler(38, 1);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(temperature.Owner), "entity", "ToPrettyString(temperature.Owner)");
					logStringHandler.AppendLiteral(" started taking low temperature damage");
					adminLogger2.Add(type2, ref logStringHandler);
					temperature.TakingDamage = true;
				}
				double tempDamage2 = Math.Sqrt((double)Math.Abs(temperature.CurrentTemperature - coldDamageThreshold) * (Math.Pow(temperature.DamageCap.Double(), 2.0) / (double)coldDamageThreshold));
				this._damageableSystem.TryChangeDamage(new EntityUid?(uid), temperature.ColdDamage * tempDamage2, false, false, null, null);
				return;
			}
			if (temperature.TakingDamage)
			{
				ISharedAdminLogManager adminLogger3 = this._adminLogger;
				LogType type3 = LogType.Temperature;
				LogStringHandler logStringHandler = new LogStringHandler(34, 1);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(temperature.Owner), "entity", "ToPrettyString(temperature.Owner)");
				logStringHandler.AppendLiteral(" stopped taking temperature damage");
				adminLogger3.Add(type3, ref logStringHandler);
				temperature.TakingDamage = false;
			}
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x00019FCB File Offset: 0x000181CB
		private void OnTemperatureChangeAttempt(EntityUid uid, TemperatureProtectionComponent component, InventoryRelayedEvent<ModifyChangedTemperatureEvent> args)
		{
			args.Args.TemperatureDelta *= component.Coefficient;
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x00019FE8 File Offset: 0x000181E8
		private void OnParentChange(EntityUid uid, TemperatureComponent component, ref EntParentChangedMessage args)
		{
			EntityQuery<TemperatureComponent> temperatureQuery = base.GetEntityQuery<TemperatureComponent>();
			EntityQuery<TransformComponent> transformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<ContainerTemperatureDamageThresholdsComponent> thresholdsQuery = base.GetEntityQuery<ContainerTemperatureDamageThresholdsComponent>();
			ValueTuple<float?, float?> valueTuple = (args.OldParent != null) ? this.RecalculateParentThresholds(args.OldParent.Value, transformQuery, thresholdsQuery) : new ValueTuple<float?, float?>(null, null);
			ValueTuple<float?, float?> newThresholds = this.RecalculateParentThresholds(transformQuery.GetComponent(uid).ParentUid, transformQuery, thresholdsQuery);
			ValueTuple<float?, float?> valueTuple2 = valueTuple;
			ValueTuple<float?, float?> valueTuple3 = newThresholds;
			float? num = valueTuple2.Item1;
			float? num2 = valueTuple3.Item1;
			if (num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null))
			{
				num2 = valueTuple2.Item2;
				num = valueTuple3.Item2;
				if (num2.GetValueOrDefault() == num.GetValueOrDefault() & num2 != null == (num != null))
				{
					return;
				}
			}
			this.RecursiveThresholdUpdate(uid, temperatureQuery, transformQuery, thresholdsQuery);
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0001A0DC File Offset: 0x000182DC
		private void OnParentThresholdStartup(EntityUid uid, ContainerTemperatureDamageThresholdsComponent component, ComponentStartup args)
		{
			this.RecursiveThresholdUpdate(uid, base.GetEntityQuery<TemperatureComponent>(), base.GetEntityQuery<TransformComponent>(), base.GetEntityQuery<ContainerTemperatureDamageThresholdsComponent>());
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0001A0F7 File Offset: 0x000182F7
		private void OnParentThresholdShutdown(EntityUid uid, ContainerTemperatureDamageThresholdsComponent component, ComponentShutdown args)
		{
			this.RecursiveThresholdUpdate(uid, base.GetEntityQuery<TemperatureComponent>(), base.GetEntityQuery<TransformComponent>(), base.GetEntityQuery<ContainerTemperatureDamageThresholdsComponent>());
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0001A114 File Offset: 0x00018314
		private void RecursiveThresholdUpdate(EntityUid root, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TemperatureComponent> temperatureQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> transformQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<ContainerTemperatureDamageThresholdsComponent> tempThresholdsQuery)
		{
			this.RecalculateAndApplyParentThresholds(root, temperatureQuery, transformQuery, tempThresholdsQuery);
			foreach (EntityUid child in base.Transform(root).ChildEntities)
			{
				this.RecursiveThresholdUpdate(child, temperatureQuery, transformQuery, tempThresholdsQuery);
			}
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0001A178 File Offset: 0x00018378
		private void RecalculateAndApplyParentThresholds(EntityUid uid, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TemperatureComponent> temperatureQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> transformQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<ContainerTemperatureDamageThresholdsComponent> tempThresholdsQuery)
		{
			TemperatureComponent temperature;
			if (!temperatureQuery.TryGetComponent(uid, ref temperature))
			{
				return;
			}
			ValueTuple<float?, float?> newThresholds = this.RecalculateParentThresholds(transformQuery.GetComponent(uid).ParentUid, transformQuery, tempThresholdsQuery);
			temperature.ParentHeatDamageThreshold = newThresholds.Item1;
			temperature.ParentColdDamageThreshold = newThresholds.Item2;
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x0001A1C4 File Offset: 0x000183C4
		[NullableContext(0)]
		private ValueTuple<float?, float?> RecalculateParentThresholds(EntityUid initialParentUid, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> transformQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<ContainerTemperatureDamageThresholdsComponent> tempThresholdsQuery)
		{
			EntityUid parentUid = initialParentUid;
			float? newHeatThreshold = null;
			float? newColdThreshold = null;
			while (parentUid.IsValid())
			{
				ContainerTemperatureDamageThresholdsComponent newThresholds;
				if (tempThresholdsQuery.TryGetComponent(parentUid, ref newThresholds))
				{
					if (newThresholds.HeatDamageThreshold != null)
					{
						newHeatThreshold = new float?(Math.Max(newThresholds.HeatDamageThreshold.Value, newHeatThreshold.GetValueOrDefault()));
					}
					if (newThresholds.ColdDamageThreshold != null)
					{
						newColdThreshold = new float?(Math.Min(newThresholds.ColdDamageThreshold.Value, newColdThreshold ?? float.MaxValue));
					}
				}
				parentUid = transformQuery.GetComponent(parentUid).ParentUid;
			}
			return new ValueTuple<float?, float?>(newHeatThreshold, newColdThreshold);
		}

		// Token: 0x0400032B RID: 811
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x0400032C RID: 812
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x0400032D RID: 813
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x0400032E RID: 814
		[Dependency]
		private readonly AlertsSystem _alertsSystem;

		// Token: 0x0400032F RID: 815
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000330 RID: 816
		public HashSet<TemperatureComponent> ShouldUpdateDamage = new HashSet<TemperatureComponent>();

		// Token: 0x04000331 RID: 817
		public float UpdateInterval = 1f;

		// Token: 0x04000332 RID: 818
		private float _accumulatedFrametime;
	}
}
