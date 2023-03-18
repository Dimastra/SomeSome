using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Whitelist
{
	// Token: 0x0200002D RID: 45
	[DataDefinition]
	[NetSerializable]
	[Serializable]
	public sealed class EntityWhitelist
	{
		// Token: 0x0600003A RID: 58 RVA: 0x000029F8 File Offset: 0x00000BF8
		public void UpdateRegistrations()
		{
			if (this.Components == null)
			{
				return;
			}
			IComponentFactory compfact = IoCManager.Resolve<IComponentFactory>();
			this._registrations = new List<ComponentRegistration>();
			foreach (string name in this.Components)
			{
				ComponentAvailability availability = compfact.GetComponentAvailability(name, false);
				ComponentRegistration registration;
				if (compfact.TryGetRegistration(name, ref registration, false) && availability == null)
				{
					this._registrations.Add(registration);
				}
				else if (availability == 2)
				{
					Logger.Warning("Unknown component name " + name + " passed to EntityWhitelist!");
				}
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002A7C File Offset: 0x00000C7C
		[NullableContext(2)]
		public bool IsValid(EntityUid uid, IEntityManager entityManager = null)
		{
			if (this.Components != null && this._registrations == null)
			{
				this.UpdateRegistrations();
			}
			IoCManager.Resolve<IEntityManager>(ref entityManager);
			if (this._registrations != null)
			{
				foreach (ComponentRegistration reg in this._registrations)
				{
					if (entityManager.HasComponent(uid, reg.Type))
					{
						if (!this.RequireAll)
						{
							return true;
						}
					}
					else if (this.RequireAll)
					{
						return false;
					}
				}
			}
			TagComponent tags;
			if (this.Tags == null || !entityManager.TryGetComponent<TagComponent>(uid, ref tags))
			{
				return false;
			}
			TagSystem tagSystem = entityManager.System<TagSystem>();
			if (!this.RequireAll)
			{
				return tagSystem.HasAnyTag(tags, this.Tags);
			}
			return tagSystem.HasAllTags(tags, this.Tags);
		}

		// Token: 0x04000086 RID: 134
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("components", false, 1, false, false, null)]
		public string[] Components;

		// Token: 0x04000087 RID: 135
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[NonSerialized]
		private List<ComponentRegistration> _registrations;

		// Token: 0x04000088 RID: 136
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("tags", false, 1, false, false, typeof(PrototypeIdListSerializer<TagPrototype>))]
		public List<string> Tags;

		// Token: 0x04000089 RID: 137
		[DataField("requireAll", false, 1, false, false, null)]
		public bool RequireAll;
	}
}
