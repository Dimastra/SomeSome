using System;
using System.Runtime.CompilerServices;
using Content.Server.Ghost.Roles.Events;
using Content.Server.Mind.Commands;
using Content.Server.Mind.Components;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Ghost.Roles.Components
{
	// Token: 0x0200049C RID: 1180
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(GhostRoleComponent))]
	public sealed class GhostRoleMobSpawnerComponent : GhostRoleComponent
	{
		// Token: 0x17000342 RID: 834
		// (get) Token: 0x060017C5 RID: 6085 RVA: 0x0007C170 File Offset: 0x0007A370
		// (set) Token: 0x060017C6 RID: 6086 RVA: 0x0007C178 File Offset: 0x0007A378
		[ViewVariables]
		[DataField("prototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype { get; private set; }

		// Token: 0x060017C7 RID: 6087 RVA: 0x0007C184 File Offset: 0x0007A384
		[NullableContext(1)]
		public override bool Take(IPlayerSession session)
		{
			if (base.Taken)
			{
				return false;
			}
			if (string.IsNullOrEmpty(this.Prototype))
			{
				throw new NullReferenceException("Prototype string cannot be null or empty!");
			}
			EntityUid mob = this._entMan.SpawnEntity(this.Prototype, this._entMan.GetComponent<TransformComponent>(base.Owner).Coordinates);
			this._entMan.GetComponent<TransformComponent>(mob).AttachToGridOrMap();
			GhostRoleSpawnerUsedEvent spawnedEvent = new GhostRoleSpawnerUsedEvent(base.Owner, mob);
			this._entMan.EventBus.RaiseLocalEvent<GhostRoleSpawnerUsedEvent>(mob, spawnedEvent, false);
			if (this.MakeSentient)
			{
				MakeSentientCommand.MakeSentient(mob, this._entMan, base.AllowMovement, base.AllowSpeech);
			}
			ComponentExt.EnsureComponent<MindComponent>(mob);
			EntitySystem.Get<GhostRoleSystem>().GhostRoleInternalCreateMindAndTransfer(session, base.Owner, mob, this);
			int num = this._currentTakeovers + 1;
			this._currentTakeovers = num;
			if (num < this._availableTakeovers)
			{
				return true;
			}
			base.Taken = true;
			if (this._deleteOnSpawn)
			{
				this._entMan.QueueDeleteEntity(base.Owner);
			}
			return true;
		}

		// Token: 0x04000EB9 RID: 3769
		[Nullable(1)]
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000EBA RID: 3770
		[ViewVariables]
		[DataField("deleteOnSpawn", false, 1, false, false, null)]
		private bool _deleteOnSpawn = true;

		// Token: 0x04000EBB RID: 3771
		[ViewVariables]
		[DataField("availableTakeovers", false, 1, false, false, null)]
		private int _availableTakeovers = 1;

		// Token: 0x04000EBC RID: 3772
		[ViewVariables]
		private int _currentTakeovers;
	}
}
