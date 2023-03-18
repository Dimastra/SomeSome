using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Logs.Converters
{
	// Token: 0x02000822 RID: 2082
	[NullableContext(1)]
	[Nullable(0)]
	[AdminLogConverter]
	public sealed class EntityUidConverter : AdminLogConverter<EntityUid>
	{
		// Token: 0x06002DBD RID: 11709 RVA: 0x000EFBA0 File Offset: 0x000EDDA0
		public override void Init(IDependencyCollection dependencies)
		{
			this._entityManager = new WeakReference<IEntityManager>(dependencies.Resolve<IEntityManager>());
		}

		// Token: 0x06002DBE RID: 11710 RVA: 0x000EFBB4 File Offset: 0x000EDDB4
		public static void Write(Utf8JsonWriter writer, EntityUid value, JsonSerializerOptions options, IEntityManager entities)
		{
			writer.WriteStartObject();
			writer.WriteNumber("id", (int)value);
			MetaDataComponent metaData;
			if (entities.TryGetComponent<MetaDataComponent>(value, ref metaData))
			{
				writer.WriteString("name", metaData.EntityName);
			}
			ActorComponent actor;
			if (entities.TryGetComponent<ActorComponent>(value, ref actor))
			{
				writer.WriteString("player", actor.PlayerSession.UserId.UserId);
			}
			writer.WriteEndObject();
		}

		// Token: 0x06002DBF RID: 11711 RVA: 0x000EFC20 File Offset: 0x000EDE20
		public override void Write(Utf8JsonWriter writer, EntityUid value, JsonSerializerOptions options)
		{
			IEntityManager entityManager;
			if (!this._entityManager.TryGetTarget(out entityManager))
			{
				throw new InvalidOperationException("EntityManager got garbage collected!");
			}
			EntityUidConverter.Write(writer, value, options, entityManager);
		}

		// Token: 0x04001C41 RID: 7233
		private WeakReference<IEntityManager> _entityManager;
	}
}
