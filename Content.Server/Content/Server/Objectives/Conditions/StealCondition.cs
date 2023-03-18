using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Objectives.Conditions
{
	// Token: 0x02000302 RID: 770
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class StealCondition : IObjectiveCondition, IEquatable<IObjectiveCondition>, ISerializationHooks
	{
		// Token: 0x06000FDA RID: 4058 RVA: 0x00050D9E File Offset: 0x0004EF9E
		public IObjectiveCondition GetAssigned(Mind mind)
		{
			return new StealCondition
			{
				_mind = mind,
				_prototypeId = this._prototypeId,
				_owner = this._owner
			};
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000FDB RID: 4059 RVA: 0x00050DC4 File Offset: 0x0004EFC4
		private string PrototypeName
		{
			get
			{
				EntityPrototype prototype;
				if (!IoCManager.Resolve<IPrototypeManager>().TryIndex<EntityPrototype>(this._prototypeId, ref prototype))
				{
					return "[CANNOT FIND NAME]";
				}
				return prototype.Name;
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000FDC RID: 4060 RVA: 0x00050DF4 File Offset: 0x0004EFF4
		public string Title
		{
			get
			{
				if (this._owner != null)
				{
					return Loc.GetString("objective-condition-steal-title", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("owner", Loc.GetString(this._owner)),
						new ValueTuple<string, object>("itemName", Loc.GetString(this.PrototypeName))
					});
				}
				return Loc.GetString("objective-condition-steal-title-no-owner", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("itemName", Loc.GetString(this.PrototypeName))
				});
			}
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000FDD RID: 4061 RVA: 0x00050E7E File Offset: 0x0004F07E
		public string Description
		{
			get
			{
				return Loc.GetString("objective-condition-steal-description", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("itemName", Loc.GetString(this.PrototypeName))
				});
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000FDE RID: 4062 RVA: 0x00050EAC File Offset: 0x0004F0AC
		public SpriteSpecifier Icon
		{
			get
			{
				return new SpriteSpecifier.EntityPrototype(this._prototypeId);
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06000FDF RID: 4063 RVA: 0x00050EBC File Offset: 0x0004F0BC
		public float Progress
		{
			get
			{
				Mind mind = this._mind;
				EntityUid? uid = (mind != null) ? mind.OwnedEntity : null;
				IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
				EntityQuery<MetaDataComponent> metaQuery = entityManager.GetEntityQuery<MetaDataComponent>();
				EntityQuery<ContainerManagerComponent> managerQuery = entityManager.GetEntityQuery<ContainerManagerComponent>();
				Stack<ContainerManagerComponent> stack = new Stack<ContainerManagerComponent>();
				Mind mind2 = this._mind;
				MetaDataComponent meta;
				if (!metaQuery.TryGetComponent((mind2 != null) ? mind2.OwnedEntity : null, ref meta))
				{
					return 0f;
				}
				EntityPrototype entityPrototype = meta.EntityPrototype;
				if (((entityPrototype != null) ? entityPrototype.ID : null) == this._prototypeId)
				{
					return 1f;
				}
				ContainerManagerComponent currentManager;
				if (!managerQuery.TryGetComponent(uid, ref currentManager))
				{
					return 0f;
				}
				do
				{
					foreach (IContainer container in currentManager.Containers.Values)
					{
						foreach (EntityUid entity in container.ContainedEntities)
						{
							EntityPrototype entityPrototype2 = metaQuery.GetComponent(entity).EntityPrototype;
							if (((entityPrototype2 != null) ? entityPrototype2.ID : null) == this._prototypeId)
							{
								return 1f;
							}
							ContainerManagerComponent containerManager;
							if (managerQuery.TryGetComponent(entity, ref containerManager))
							{
								stack.Push(containerManager);
							}
						}
					}
				}
				while (stack.TryPop(out currentManager));
				return 0f;
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000FE0 RID: 4064 RVA: 0x00051040 File Offset: 0x0004F240
		public float Difficulty
		{
			get
			{
				return 2.25f;
			}
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x00051048 File Offset: 0x0004F248
		[NullableContext(2)]
		public bool Equals(IObjectiveCondition other)
		{
			StealCondition stealCondition = other as StealCondition;
			return stealCondition != null && object.Equals(this._mind, stealCondition._mind) && this._prototypeId == stealCondition._prototypeId;
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x00051085 File Offset: 0x0004F285
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((StealCondition)obj)));
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x000510B3 File Offset: 0x0004F2B3
		public override int GetHashCode()
		{
			return HashCode.Combine<Mind, string>(this._mind, this._prototypeId);
		}

		// Token: 0x0400092B RID: 2347
		[Nullable(2)]
		private Mind _mind;

		// Token: 0x0400092C RID: 2348
		[DataField("prototype", false, 1, false, false, null)]
		private string _prototypeId = string.Empty;

		// Token: 0x0400092D RID: 2349
		[Nullable(2)]
		[DataField("owner", false, 1, false, false, null)]
		private string _owner;
	}
}
