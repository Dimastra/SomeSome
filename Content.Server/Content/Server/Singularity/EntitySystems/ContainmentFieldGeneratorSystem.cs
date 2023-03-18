using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Server.Popups;
using Content.Server.Singularity.Events;
using Content.Shared.Construction.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Singularity.Components;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;

namespace Content.Server.Singularity.EntitySystems
{
	// Token: 0x020001E7 RID: 487
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ContainmentFieldGeneratorSystem : EntitySystem
	{
		// Token: 0x0600092C RID: 2348 RVA: 0x0002E23C File Offset: 0x0002C43C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ContainmentFieldGeneratorComponent, StartCollideEvent>(new ComponentEventRefHandler<ContainmentFieldGeneratorComponent, StartCollideEvent>(this.HandleGeneratorCollide), null, null);
			base.SubscribeLocalEvent<ContainmentFieldGeneratorComponent, ExaminedEvent>(new ComponentEventHandler<ContainmentFieldGeneratorComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<ContainmentFieldGeneratorComponent, InteractHandEvent>(new ComponentEventHandler<ContainmentFieldGeneratorComponent, InteractHandEvent>(this.OnInteract), null, null);
			base.SubscribeLocalEvent<ContainmentFieldGeneratorComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<ContainmentFieldGeneratorComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
			base.SubscribeLocalEvent<ContainmentFieldGeneratorComponent, ReAnchorEvent>(new ComponentEventRefHandler<ContainmentFieldGeneratorComponent, ReAnchorEvent>(this.OnReanchorEvent), null, null);
			base.SubscribeLocalEvent<ContainmentFieldGeneratorComponent, UnanchorAttemptEvent>(new ComponentEventHandler<ContainmentFieldGeneratorComponent, UnanchorAttemptEvent>(this.OnUnanchorAttempt), null, null);
			base.SubscribeLocalEvent<ContainmentFieldGeneratorComponent, ComponentRemove>(new ComponentEventHandler<ContainmentFieldGeneratorComponent, ComponentRemove>(this.OnComponentRemoved), null, null);
			base.SubscribeLocalEvent<ContainmentFieldGeneratorComponent, EventHorizonAttemptConsumeEntityEvent>(new ComponentEventHandler<ContainmentFieldGeneratorComponent, EventHorizonAttemptConsumeEntityEvent>(this.PreventBreach), null, null);
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x0002E2F0 File Offset: 0x0002C4F0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ContainmentFieldGeneratorComponent generator in base.EntityQuery<ContainmentFieldGeneratorComponent>(false))
			{
				if (generator.PowerBuffer > 0)
				{
					generator.Accumulator += frameTime;
					if (generator.Accumulator >= generator.Threshold)
					{
						this.LosePower(generator.PowerLoss, generator);
						generator.Accumulator -= generator.Threshold;
					}
				}
			}
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x0002E384 File Offset: 0x0002C584
		private void HandleGeneratorCollide(EntityUid uid, ContainmentFieldGeneratorComponent component, ref StartCollideEvent args)
		{
			if (this._tags.HasTag(args.OtherFixture.Body.Owner, component.IDTag))
			{
				this.ReceivePower(component.PowerReceived, component);
				component.Accumulator = 0f;
			}
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x0002E3C1 File Offset: 0x0002C5C1
		private void OnExamine(EntityUid uid, ContainmentFieldGeneratorComponent component, ExaminedEvent args)
		{
			if (component.Enabled)
			{
				args.PushMarkup(Loc.GetString("comp-containment-on"));
				return;
			}
			args.PushMarkup(Loc.GetString("comp-containment-off"));
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x0002E3EC File Offset: 0x0002C5EC
		private void OnInteract(EntityUid uid, ContainmentFieldGeneratorComponent component, InteractHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TransformComponent transformComp;
			if (base.TryComp<TransformComponent>(component.Owner, ref transformComp) && transformComp.Anchored)
			{
				if (!component.Enabled)
				{
					this.TurnOn(component);
				}
				else
				{
					if (component.Enabled && component.IsConnected)
					{
						this._popupSystem.PopupEntity(Loc.GetString("comp-containment-toggle-warning"), args.User, args.User, PopupType.LargeCaution);
						return;
					}
					this.TurnOff(component);
				}
			}
			args.Handled = true;
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x0002E46C File Offset: 0x0002C66C
		private void OnAnchorChanged(EntityUid uid, ContainmentFieldGeneratorComponent component, ref AnchorStateChangedEvent args)
		{
			if (!args.Anchored)
			{
				this.RemoveConnections(component);
			}
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x0002E47D File Offset: 0x0002C67D
		private void OnReanchorEvent(EntityUid uid, ContainmentFieldGeneratorComponent component, ref ReAnchorEvent args)
		{
			this.GridCheck(component);
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x0002E486 File Offset: 0x0002C686
		private void OnUnanchorAttempt(EntityUid uid, ContainmentFieldGeneratorComponent component, UnanchorAttemptEvent args)
		{
			if (component.Enabled)
			{
				this._popupSystem.PopupEntity(Loc.GetString("comp-containment-anchor-warning"), args.User, args.User, PopupType.LargeCaution);
				args.Cancel();
			}
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x0002E4B8 File Offset: 0x0002C6B8
		private void TurnOn(ContainmentFieldGeneratorComponent component)
		{
			component.Enabled = true;
			this.ChangeFieldVisualizer(component);
			this._popupSystem.PopupEntity(Loc.GetString("comp-containment-turned-on"), component.Owner, PopupType.Small);
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x0002E4E4 File Offset: 0x0002C6E4
		private void TurnOff(ContainmentFieldGeneratorComponent component)
		{
			component.Enabled = false;
			this.ChangeFieldVisualizer(component);
			this._popupSystem.PopupEntity(Loc.GetString("comp-containment-turned-off"), component.Owner, PopupType.Small);
		}

		// Token: 0x06000936 RID: 2358 RVA: 0x0002E510 File Offset: 0x0002C710
		private void OnComponentRemoved(EntityUid uid, ContainmentFieldGeneratorComponent component, ComponentRemove args)
		{
			this.RemoveConnections(component);
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x0002E51C File Offset: 0x0002C71C
		private void RemoveConnections(ContainmentFieldGeneratorComponent component)
		{
			foreach (KeyValuePair<Direction, ValueTuple<ContainmentFieldGeneratorComponent, List<EntityUid>>> keyValuePair in component.Connections)
			{
				Direction direction2;
				ValueTuple<ContainmentFieldGeneratorComponent, List<EntityUid>> valueTuple;
				keyValuePair.Deconstruct(out direction2, out valueTuple);
				Direction direction = direction2;
				ValueTuple<ContainmentFieldGeneratorComponent, List<EntityUid>> value = valueTuple;
				foreach (EntityUid field in value.Item2)
				{
					base.QueueDel(field);
				}
				value.Item1.Connections.Remove(DirectionExtensions.GetOpposite(direction));
				if (value.Item1.Connections.Count == 0)
				{
					value.Item1.IsConnected = false;
					this.ChangeOnLightVisualizer(value.Item1);
				}
				this.ChangeFieldVisualizer(value.Item1);
			}
			component.Connections.Clear();
			component.IsConnected = false;
			this.ChangeOnLightVisualizer(component);
			this.ChangeFieldVisualizer(component);
			this._popupSystem.PopupEntity(Loc.GetString("comp-containment-disconnected"), component.Owner, PopupType.LargeCaution);
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x0002E650 File Offset: 0x0002C850
		public void ReceivePower(int power, ContainmentFieldGeneratorComponent component)
		{
			component.PowerBuffer += power;
			TransformComponent genXForm = base.Transform(component.Owner);
			if (component.PowerBuffer >= component.PowerMinimum)
			{
				int directions = Enum.GetValues<Direction>().Length;
				for (int i = 0; i < directions - 1; i += 2)
				{
					Direction dir = (sbyte)i;
					if (!component.Connections.ContainsKey(dir))
					{
						this.TryGenerateFieldConnection(dir, component, genXForm);
					}
				}
			}
			this.ChangePowerVisualizer(power, component);
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x0002E6C0 File Offset: 0x0002C8C0
		public void LosePower(int power, ContainmentFieldGeneratorComponent component)
		{
			component.PowerBuffer -= power;
			if (component.PowerBuffer < component.PowerMinimum && component.Connections.Count != 0)
			{
				this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-singularity-field-down", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("fieldgenerator", base.ToPrettyString(component.Owner))
				}));
				this.RemoveConnections(component);
			}
			this.ChangePowerVisualizer(power, component);
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x0002E744 File Offset: 0x0002C944
		private bool TryGenerateFieldConnection(Direction dir, ContainmentFieldGeneratorComponent component, TransformComponent gen1XForm)
		{
			if (!component.Enabled)
			{
				return false;
			}
			if (!gen1XForm.Anchored)
			{
				return false;
			}
			ValueTuple<Vector2, Angle> genWorldPosRot = gen1XForm.GetWorldPositionRotation();
			Angle dirRad = DirectionExtensions.ToAngle(dir) + genWorldPosRot.Item2;
			CollisionRay ray;
			ray..ctor(genWorldPosRot.Item1, dirRad.ToVec(), component.CollisionMask);
			IEnumerable<RayCastResults> enumerable = this._physics.IntersectRay(gen1XForm.MapID, ray, component.MaxLength, new EntityUid?(component.Owner), false);
			EntityQuery<ContainmentFieldGeneratorComponent> genQuery = base.GetEntityQuery<ContainmentFieldGeneratorComponent>();
			RayCastResults? closestResult = null;
			using (IEnumerator<RayCastResults> enumerator = enumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					RayCastResults result = enumerator.Current;
					if (genQuery.HasComponent(result.HitEntity))
					{
						closestResult = new RayCastResults?(result);
					}
				}
			}
			if (closestResult == null)
			{
				return false;
			}
			EntityUid ent = closestResult.Value.HitEntity;
			ContainmentFieldGeneratorComponent otherFieldGeneratorComponent;
			PhysicsComponent collidableComponent;
			if (!base.TryComp<ContainmentFieldGeneratorComponent>(ent, ref otherFieldGeneratorComponent) || otherFieldGeneratorComponent == component || !base.TryComp<PhysicsComponent>(ent, ref collidableComponent) || collidableComponent.BodyType != 4 || gen1XForm.ParentUid != base.Transform(otherFieldGeneratorComponent.Owner).ParentUid)
			{
				return false;
			}
			List<EntityUid> fields = this.GenerateFieldConnection(component, otherFieldGeneratorComponent);
			component.Connections[dir] = new ValueTuple<ContainmentFieldGeneratorComponent, List<EntityUid>>(otherFieldGeneratorComponent, fields);
			otherFieldGeneratorComponent.Connections[DirectionExtensions.GetOpposite(dir)] = new ValueTuple<ContainmentFieldGeneratorComponent, List<EntityUid>>(component, fields);
			this.ChangeFieldVisualizer(otherFieldGeneratorComponent);
			if (!component.IsConnected)
			{
				component.IsConnected = true;
				this.ChangeOnLightVisualizer(component);
			}
			if (!otherFieldGeneratorComponent.IsConnected)
			{
				otherFieldGeneratorComponent.IsConnected = true;
				this.ChangeOnLightVisualizer(otherFieldGeneratorComponent);
			}
			this.ChangeFieldVisualizer(component);
			this.UpdateConnectionLights(component);
			this._popupSystem.PopupEntity(Loc.GetString("comp-containment-connected"), component.Owner, PopupType.Small);
			return true;
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x0002E920 File Offset: 0x0002CB20
		private List<EntityUid> GenerateFieldConnection(ContainmentFieldGeneratorComponent firstGenComp, ContainmentFieldGeneratorComponent secondGenComp)
		{
			List<EntityUid> fieldList = new List<EntityUid>();
			EntityCoordinates gen1Coords = base.Transform(firstGenComp.Owner).Coordinates;
			Vector2 delta = (base.Transform(secondGenComp.Owner).Coordinates - gen1Coords).Position;
			Vector2 dirVec = delta.Normalized;
			float stopDist = delta.Length;
			Vector2 currentOffset = dirVec;
			while (currentOffset.Length < stopDist)
			{
				EntityCoordinates currentCoords = gen1Coords.Offset(currentOffset);
				EntityUid newField = base.Spawn(firstGenComp.CreatedField, currentCoords);
				TransformComponent fieldXForm = base.Transform(newField);
				fieldXForm.AttachParent(firstGenComp.Owner);
				if (DirectionExtensions.GetDir(dirVec) == 2 || DirectionExtensions.GetDir(dirVec) == 6)
				{
					Angle rotatedAngle = Angle.FromDegrees(DirectionExtensions.ToAngle(fieldXForm.LocalPosition).Degrees + 90.0);
					fieldXForm.LocalRotation = rotatedAngle;
				}
				fieldList.Add(newField);
				currentOffset += dirVec;
			}
			return fieldList;
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x0002EA10 File Offset: 0x0002CC10
		public void UpdateConnectionLights(ContainmentFieldGeneratorComponent component)
		{
			PointLightComponent pointLightComponent;
			if (this.EntityManager.TryGetComponent<PointLightComponent>(component.Owner, ref pointLightComponent))
			{
				pointLightComponent.Enabled = (component.Connections.Count > 0);
			}
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x0002EA48 File Offset: 0x0002CC48
		public void GridCheck(ContainmentFieldGeneratorComponent component)
		{
			EntityQuery<TransformComponent> xFormQuery = base.GetEntityQuery<TransformComponent>();
			foreach (KeyValuePair<Direction, ValueTuple<ContainmentFieldGeneratorComponent, List<EntityUid>>> keyValuePair in component.Connections)
			{
				Direction direction;
				ValueTuple<ContainmentFieldGeneratorComponent, List<EntityUid>> valueTuple;
				keyValuePair.Deconstruct(out direction, out valueTuple);
				ValueTuple<ContainmentFieldGeneratorComponent, List<EntityUid>> generators = valueTuple;
				EntityUid parentUid = xFormQuery.GetComponent(component.Owner).ParentUid;
				EntityUid gent2ParentGrid = xFormQuery.GetComponent(generators.Item1.Owner).ParentUid;
				if (parentUid != gent2ParentGrid)
				{
					this.RemoveConnections(component);
				}
			}
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x0002EAE4 File Offset: 0x0002CCE4
		private void ChangePowerVisualizer(int power, ContainmentFieldGeneratorComponent component)
		{
			SharedAppearanceSystem visualizer = this._visualizer;
			EntityUid owner = component.Owner;
			Enum @enum = ContainmentFieldGeneratorVisuals.PowerLight;
			int powerBuffer = component.PowerBuffer;
			PowerLevelVisuals powerLevelVisuals;
			if (powerBuffer > 0)
			{
				if (powerBuffer < 25)
				{
					powerLevelVisuals = ((component.PowerBuffer < component.PowerMinimum) ? PowerLevelVisuals.LowPower : PowerLevelVisuals.MediumPower);
				}
				else
				{
					powerLevelVisuals = PowerLevelVisuals.HighPower;
				}
			}
			else
			{
				powerLevelVisuals = PowerLevelVisuals.NoPower;
			}
			visualizer.SetData(owner, @enum, powerLevelVisuals, null);
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x0002EB40 File Offset: 0x0002CD40
		private void ChangeFieldVisualizer(ContainmentFieldGeneratorComponent component)
		{
			AppearanceSystem visualizer = this._visualizer;
			EntityUid owner = component.Owner;
			Enum @enum = ContainmentFieldGeneratorVisuals.FieldLight;
			int count = component.Connections.Count;
			FieldLevelVisuals fieldLevelVisuals;
			if (count <= 1)
			{
				if (count != 1)
				{
					fieldLevelVisuals = (component.Enabled ? FieldLevelVisuals.On : FieldLevelVisuals.NoLevel);
				}
				else
				{
					fieldLevelVisuals = FieldLevelVisuals.OneField;
				}
			}
			else
			{
				fieldLevelVisuals = FieldLevelVisuals.MultipleFields;
			}
			visualizer.SetData(owner, @enum, fieldLevelVisuals, null);
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x0002EB9F File Offset: 0x0002CD9F
		private void ChangeOnLightVisualizer(ContainmentFieldGeneratorComponent component)
		{
			this._visualizer.SetData(component.Owner, ContainmentFieldGeneratorVisuals.OnLight, component.IsConnected, null);
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x0002EBC4 File Offset: 0x0002CDC4
		private void PreventBreach(EntityUid uid, ContainmentFieldGeneratorComponent comp, EventHorizonAttemptConsumeEntityEvent args)
		{
			if (args.Cancelled)
			{
				return;
			}
			if (comp.IsConnected && !args.EventHorizon.CanBreachContainment)
			{
				args.Cancel();
			}
		}

		// Token: 0x04000598 RID: 1432
		[Dependency]
		private readonly TagSystem _tags;

		// Token: 0x04000599 RID: 1433
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400059A RID: 1434
		[Dependency]
		private readonly PhysicsSystem _physics;

		// Token: 0x0400059B RID: 1435
		[Dependency]
		private readonly AppearanceSystem _visualizer;

		// Token: 0x0400059C RID: 1436
		[Dependency]
		private readonly IChatManager _chatManager;
	}
}
