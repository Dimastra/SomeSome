using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Logs.Converters
{
	// Token: 0x02000824 RID: 2084
	[NullableContext(1)]
	[Nullable(0)]
	[AdminLogConverter]
	public sealed class PlayerSessionConverter : AdminLogConverter<SerializablePlayer>
	{
		// Token: 0x06002DC3 RID: 11715 RVA: 0x000EFC6F File Offset: 0x000EDE6F
		public override void Init(IDependencyCollection dependencies)
		{
			this._entityManager = new WeakReference<IEntityManager>(dependencies.Resolve<IEntityManager>());
		}

		// Token: 0x06002DC4 RID: 11716 RVA: 0x000EFC84 File Offset: 0x000EDE84
		public override void Write(Utf8JsonWriter writer, SerializablePlayer value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			EntityUid? attachedEntity = value.Player.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid playerEntity = attachedEntity.GetValueOrDefault();
				if (playerEntity.Valid)
				{
					IEntityManager entityManager;
					if (!this._entityManager.TryGetTarget(out entityManager))
					{
						throw new InvalidOperationException("EntityManager got garbage collected!");
					}
					writer.WriteNumber("id", (int)value.Player.AttachedEntity.Value);
					writer.WriteString("name", entityManager.GetComponent<MetaDataComponent>(playerEntity).EntityName);
				}
			}
			writer.WriteString("player", value.Player.UserId.UserId);
			writer.WriteEndObject();
		}

		// Token: 0x04001C42 RID: 7234
		private WeakReference<IEntityManager> _entityManager;
	}
}
