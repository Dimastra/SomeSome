using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Spawners
{
	// Token: 0x0200013B RID: 315
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentProtoName("ClientEntitySpawner")]
	public sealed class ClientEntitySpawnerComponent : Component
	{
		// Token: 0x0600085A RID: 2138 RVA: 0x000309C7 File Offset: 0x0002EBC7
		protected override void Initialize()
		{
			base.Initialize();
			this.SpawnEntities();
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x000309D5 File Offset: 0x0002EBD5
		protected override void OnRemove()
		{
			this.RemoveEntities();
			base.OnRemove();
		}

		// Token: 0x0600085C RID: 2140 RVA: 0x000309E4 File Offset: 0x0002EBE4
		private void SpawnEntities()
		{
			foreach (string text in this._prototypes)
			{
				EntityUid item = this._entMan.SpawnEntity(text, this._entMan.GetComponent<TransformComponent>(base.Owner).Coordinates);
				this._entity.Add(item);
			}
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x00030A60 File Offset: 0x0002EC60
		private void RemoveEntities()
		{
			foreach (EntityUid entityUid in this._entity)
			{
				this._entMan.DeleteEntity(entityUid);
			}
		}

		// Token: 0x04000438 RID: 1080
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000439 RID: 1081
		[DataField("prototypes", false, 1, false, false, null)]
		private List<string> _prototypes = new List<string>
		{
			"HVDummyWire"
		};

		// Token: 0x0400043A RID: 1082
		private readonly List<EntityUid> _entity = new List<EntityUid>();
	}
}
