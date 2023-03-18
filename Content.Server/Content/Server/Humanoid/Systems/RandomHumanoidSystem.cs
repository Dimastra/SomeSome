using System;
using System.Runtime.CompilerServices;
using Content.Server.Humanoid.Components;
using Content.Server.RandomMetadata;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Server.Humanoid.Systems
{
	// Token: 0x0200045B RID: 1115
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomHumanoidSystem : EntitySystem
	{
		// Token: 0x06001689 RID: 5769 RVA: 0x00077142 File Offset: 0x00075342
		public override void Initialize()
		{
			base.SubscribeLocalEvent<RandomHumanoidSpawnerComponent, MapInitEvent>(new ComponentEventHandler<RandomHumanoidSpawnerComponent, MapInitEvent>(this.OnMapInit), null, new Type[]
			{
				typeof(RandomMetadataSystem)
			});
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x0007716A File Offset: 0x0007536A
		private void OnMapInit(EntityUid uid, RandomHumanoidSpawnerComponent component, MapInitEvent args)
		{
			base.QueueDel(uid);
			this.SpawnRandomHumanoid(component.SettingsPrototypeId, base.Transform(uid).Coordinates, base.MetaData(uid).EntityName);
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x00077198 File Offset: 0x00075398
		public EntityUid SpawnRandomHumanoid(string prototypeId, EntityCoordinates coordinates, string name)
		{
			RandomHumanoidSettingsPrototype prototype;
			if (!this._prototypeManager.TryIndex<RandomHumanoidSettingsPrototype>(prototypeId, ref prototype))
			{
				throw new ArgumentException("Could not get random humanoid settings");
			}
			HumanoidCharacterProfile profile = HumanoidCharacterProfile.Random(prototype.SpeciesBlacklist);
			SpeciesPrototype speciesProto = this._prototypeManager.Index<SpeciesPrototype>(profile.Species);
			EntityUid humanoid = base.Spawn(speciesProto.Prototype, coordinates);
			base.MetaData(humanoid).EntityName = (prototype.RandomizeName ? profile.Name : name);
			this._humanoid.LoadProfile(humanoid, profile, null);
			if (prototype.Components == null)
			{
				return humanoid;
			}
			foreach (EntityPrototype.ComponentRegistryEntry entry in prototype.Components.Values)
			{
				Component comp = (Component)this._serialization.CreateCopy<IComponent>(entry.Component, null, false, true);
				comp.Owner = humanoid;
				this.EntityManager.AddComponent<Component>(humanoid, comp, true);
			}
			return humanoid;
		}

		// Token: 0x04000E11 RID: 3601
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000E12 RID: 3602
		[Dependency]
		private readonly IComponentFactory _compFactory;

		// Token: 0x04000E13 RID: 3603
		[Dependency]
		private readonly ISerializationManager _serialization;

		// Token: 0x04000E14 RID: 3604
		[Dependency]
		private readonly HumanoidAppearanceSystem _humanoid;
	}
}
