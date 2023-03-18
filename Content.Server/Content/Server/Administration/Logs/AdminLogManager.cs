using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Administration.Logs.Converters;
using Content.Server.Database;
using Content.Server.GameTicking;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Prometheus;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Reflection;
using Robust.Shared.Timing;

namespace Content.Server.Administration.Logs
{
	// Token: 0x02000818 RID: 2072
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminLogManager : SharedAdminLogManager, IAdminLogManager, ISharedAdminLogManager
	{
		// Token: 0x06002D53 RID: 11603 RVA: 0x000EEA90 File Offset: 0x000ECC90
		public void CacheNewRound()
		{
			int oldestRound = this._currentRoundId - 3;
			List<SharedAdminLog> oldestList;
			List<SharedAdminLog> list;
			if (this._roundsLogCache.Remove(oldestRound, out oldestList))
			{
				list = oldestList;
				list.Clear();
			}
			else
			{
				list = new List<SharedAdminLog>(30000);
			}
			this._roundsLogCache.Add(this._currentRoundId, list);
			AdminLogManager.CacheRoundCount.Set((double)this._roundsLogCache.Count);
		}

		// Token: 0x06002D54 RID: 11604 RVA: 0x000EEAF4 File Offset: 0x000ECCF4
		private void CacheLog(AdminLog log)
		{
			Guid[] players = (from player in log.Players
			select player.PlayerUserId).ToArray<Guid>();
			SharedAdminLog record = new SharedAdminLog(log.Id, log.Type, log.Impact, log.Date, log.Message, players);
			this.CacheLog(record);
		}

		// Token: 0x06002D55 RID: 11605 RVA: 0x000EEB5E File Offset: 0x000ECD5E
		private void CacheLog(QueuedLog log)
		{
			this.CacheLog(log.Log);
		}

		// Token: 0x06002D56 RID: 11606 RVA: 0x000EEB70 File Offset: 0x000ECD70
		private void CacheLog(SharedAdminLog log)
		{
			List<SharedAdminLog> cache = this._roundsLogCache[this._currentRoundId];
			cache.Add(log);
			AdminLogManager.CacheLogCount.Set((double)cache.Count);
		}

		// Token: 0x06002D57 RID: 11607 RVA: 0x000EEBA8 File Offset: 0x000ECDA8
		private void CacheLogs(IEnumerable<SharedAdminLog> logs)
		{
			List<SharedAdminLog> cache = this._roundsLogCache[this._currentRoundId];
			cache.AddRange(logs);
			AdminLogManager.CacheLogCount.Set((double)cache.Count);
		}

		// Token: 0x06002D58 RID: 11608 RVA: 0x000EEBDF File Offset: 0x000ECDDF
		[NullableContext(2)]
		private bool TryGetCache(int roundId, [NotNullWhen(true)] out List<SharedAdminLog> cache)
		{
			return this._roundsLogCache.TryGetValue(roundId, out cache);
		}

		// Token: 0x06002D59 RID: 11609 RVA: 0x000EEBF0 File Offset: 0x000ECDF0
		[NullableContext(2)]
		private bool TrySearchCache(LogFilter filter, [NotNullWhen(true)] out List<SharedAdminLog> results)
		{
			LogFilter filter2 = filter;
			List<SharedAdminLog> cache;
			if (filter2 == null || filter2.Round == null || !this.TryGetCache(filter.Round.Value, out cache))
			{
				results = null;
				return false;
			}
			IEnumerable<SharedAdminLog> query = cache.AsEnumerable<SharedAdminLog>();
			DateOrder dateOrder = filter.DateOrder;
			IEnumerable<SharedAdminLog> enumerable;
			if (dateOrder != DateOrder.Ascending)
			{
				if (dateOrder != DateOrder.Descending)
				{
					string paramName = "filter";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Unknown ");
					defaultInterpolatedStringHandler.AppendFormatted("DateOrder");
					defaultInterpolatedStringHandler.AppendLiteral(" value ");
					defaultInterpolatedStringHandler.AppendFormatted<DateOrder>(filter.DateOrder);
					throw new ArgumentOutOfRangeException(paramName, defaultInterpolatedStringHandler.ToStringAndClear());
				}
				enumerable = query.Reverse<SharedAdminLog>();
			}
			else
			{
				enumerable = query;
			}
			query = enumerable;
			if (filter.Search != null)
			{
				query = from log in query
				where log.Message.Contains(filter.Search, StringComparison.OrdinalIgnoreCase)
				select log;
			}
			if (filter.Types != null && filter.Types.Count != this._logTypes)
			{
				query = from log in query
				where filter.Types.Contains(log.Type)
				select log;
			}
			if (filter.Impacts != null)
			{
				query = from log in query
				where filter.Impacts.Contains(log.Impact)
				select log;
			}
			if (filter.Before != null)
			{
				query = from log in query
				where log.Date < filter.Before
				select log;
			}
			if (filter.After != null)
			{
				query = from log in query
				where log.Date > filter.After
				select log;
			}
			if (filter.AnyPlayers != null)
			{
				query = from log in query
				where filter.AnyPlayers.Any((Guid filterPlayer) => log.Players.Contains(filterPlayer))
				select log;
			}
			if (filter.AllPlayers != null)
			{
				query = from log in query
				where filter.AllPlayers.All((Guid filterPlayer) => log.Players.Contains(filterPlayer))
				select log;
			}
			if (filter.LogsSent != 0)
			{
				query = query.Skip(filter.LogsSent);
			}
			if (filter.Limit != null)
			{
				query = query.Take(filter.Limit.Value);
			}
			results = query.ToList<SharedAdminLog>();
			return true;
		}

		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x06002D5A RID: 11610 RVA: 0x000EEE2F File Offset: 0x000ED02F
		private int NextLogId
		{
			get
			{
				return Interlocked.Increment(ref this._currentLogId);
			}
		}

		// Token: 0x06002D5B RID: 11611 RVA: 0x000EEE3C File Offset: 0x000ED03C
		public void Initialize()
		{
			this._sawmill = this._logManager.GetSawmill("admin.logs");
			this.InitializeJson();
			this._configuration.OnValueChanged<bool>(CVars.MetricsEnabled, delegate(bool value)
			{
				this._metricsEnabled = value;
			}, true);
			this._configuration.OnValueChanged<bool>(CCVars.AdminLogsEnabled, delegate(bool value)
			{
				this._enabled = value;
			}, true);
			this._configuration.OnValueChanged<float>(CCVars.AdminLogsQueueSendDelay, delegate(float value)
			{
				this._queueSendDelay = TimeSpan.FromSeconds((double)value);
			}, true);
			this._configuration.OnValueChanged<int>(CCVars.AdminLogsQueueMax, delegate(int value)
			{
				this._queueMax = value;
			}, true);
			this._configuration.OnValueChanged<int>(CCVars.AdminLogsPreRoundQueueMax, delegate(int value)
			{
				this._preRoundQueueMax = value;
			}, true);
			if (this._metricsEnabled)
			{
				AdminLogManager.PreRoundQueueCapReached.Set(0.0);
				AdminLogManager.QueueCapReached.Set(0.0);
				AdminLogManager.LogsSent.Set(0.0);
			}
		}

		// Token: 0x06002D5C RID: 11612 RVA: 0x000EEF38 File Offset: 0x000ED138
		public Task Shutdown()
		{
			AdminLogManager.<Shutdown>d__43 <Shutdown>d__;
			<Shutdown>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Shutdown>d__.<>4__this = this;
			<Shutdown>d__.<>1__state = -1;
			<Shutdown>d__.<>t__builder.Start<AdminLogManager.<Shutdown>d__43>(ref <Shutdown>d__);
			return <Shutdown>d__.<>t__builder.Task;
		}

		// Token: 0x06002D5D RID: 11613 RVA: 0x000EEF7C File Offset: 0x000ED17C
		public void Update()
		{
			AdminLogManager.<Update>d__44 <Update>d__;
			<Update>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Update>d__.<>4__this = this;
			<Update>d__.<>1__state = -1;
			<Update>d__.<>t__builder.Start<AdminLogManager.<Update>d__44>(ref <Update>d__);
		}

		// Token: 0x06002D5E RID: 11614 RVA: 0x000EEFB4 File Offset: 0x000ED1B4
		private Task PreRoundUpdate()
		{
			AdminLogManager.<PreRoundUpdate>d__45 <PreRoundUpdate>d__;
			<PreRoundUpdate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<PreRoundUpdate>d__.<>4__this = this;
			<PreRoundUpdate>d__.<>1__state = -1;
			<PreRoundUpdate>d__.<>t__builder.Start<AdminLogManager.<PreRoundUpdate>d__45>(ref <PreRoundUpdate>d__);
			return <PreRoundUpdate>d__.<>t__builder.Task;
		}

		// Token: 0x06002D5F RID: 11615 RVA: 0x000EEFF8 File Offset: 0x000ED1F8
		private Task SaveLogs()
		{
			AdminLogManager.<SaveLogs>d__46 <SaveLogs>d__;
			<SaveLogs>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveLogs>d__.<>4__this = this;
			<SaveLogs>d__.<>1__state = -1;
			<SaveLogs>d__.<>t__builder.Start<AdminLogManager.<SaveLogs>d__46>(ref <SaveLogs>d__);
			return <SaveLogs>d__.<>t__builder.Task;
		}

		// Token: 0x06002D60 RID: 11616 RVA: 0x000EF03B File Offset: 0x000ED23B
		public void RoundStarting(int id)
		{
			this._currentRoundId = id;
			this.CacheNewRound();
		}

		// Token: 0x06002D61 RID: 11617 RVA: 0x000EF04C File Offset: 0x000ED24C
		public void RunLevelChanged(GameRunLevel level)
		{
			this._runLevel = level;
			if (level == GameRunLevel.PreRoundLobby)
			{
				Interlocked.Exchange(ref this._currentLogId, 0);
				if (this._metricsEnabled)
				{
					AdminLogManager.PreRoundQueueCapReached.Set(0.0);
					AdminLogManager.QueueCapReached.Set(0.0);
					AdminLogManager.LogsSent.Set(0.0);
				}
			}
		}

		// Token: 0x06002D62 RID: 11618 RVA: 0x000EF0B4 File Offset: 0x000ED2B4
		private void Add(LogType type, LogImpact impact, string message, JsonDocument json, HashSet<Guid> players, [Nullable(new byte[]
		{
			1,
			2
		})] Dictionary<int, string> entities)
		{
			AdminLogManager.<Add>d__49 <Add>d__;
			<Add>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Add>d__.<>4__this = this;
			<Add>d__.type = type;
			<Add>d__.impact = impact;
			<Add>d__.message = message;
			<Add>d__.json = json;
			<Add>d__.players = players;
			<Add>d__.entities = entities;
			<Add>d__.<>1__state = -1;
			<Add>d__.<>t__builder.Start<AdminLogManager.<Add>d__49>(ref <Add>d__);
		}

		// Token: 0x06002D63 RID: 11619 RVA: 0x000EF120 File Offset: 0x000ED320
		public override void Add(LogType type, LogImpact impact, ref LogStringHandler handler)
		{
			if (!this._enabled)
			{
				handler.ToStringAndClear();
				return;
			}
			ValueTuple<JsonDocument, HashSet<Guid>, Dictionary<int, string>> valueTuple = this.ToJson(handler.Values);
			JsonDocument json = valueTuple.Item1;
			HashSet<Guid> players = valueTuple.Item2;
			Dictionary<int, string> entities = valueTuple.Item3;
			string message = handler.ToStringAndClear();
			this.Add(type, impact, message, json, players, entities);
		}

		// Token: 0x06002D64 RID: 11620 RVA: 0x000EF170 File Offset: 0x000ED370
		public override void Add(LogType type, ref LogStringHandler handler)
		{
			this.Add(type, LogImpact.Medium, ref handler);
		}

		// Token: 0x06002D65 RID: 11621 RVA: 0x000EF17C File Offset: 0x000ED37C
		public Task<List<SharedAdminLog>> All([Nullable(2)] LogFilter filter = null, [Nullable(new byte[]
		{
			2,
			1
		})] Func<List<SharedAdminLog>> listProvider = null)
		{
			AdminLogManager.<All>d__52 <All>d__;
			<All>d__.<>t__builder = AsyncTaskMethodBuilder<List<SharedAdminLog>>.Create();
			<All>d__.<>4__this = this;
			<All>d__.filter = filter;
			<All>d__.listProvider = listProvider;
			<All>d__.<>1__state = -1;
			<All>d__.<>t__builder.Start<AdminLogManager.<All>d__52>(ref <All>d__);
			return <All>d__.<>t__builder.Task;
		}

		// Token: 0x06002D66 RID: 11622 RVA: 0x000EF1CF File Offset: 0x000ED3CF
		public IAsyncEnumerable<string> AllMessages([Nullable(2)] LogFilter filter = null)
		{
			return this._db.GetAdminLogMessages(filter);
		}

		// Token: 0x06002D67 RID: 11623 RVA: 0x000EF1DD File Offset: 0x000ED3DD
		public IAsyncEnumerable<JsonDocument> AllJson([Nullable(2)] LogFilter filter = null)
		{
			return this._db.GetAdminLogsJson(filter);
		}

		// Token: 0x06002D68 RID: 11624 RVA: 0x000EF1EB File Offset: 0x000ED3EB
		public Task<Round> Round(int roundId)
		{
			return this._db.GetRound(roundId);
		}

		// Token: 0x06002D69 RID: 11625 RVA: 0x000EF1F9 File Offset: 0x000ED3F9
		public Task<List<SharedAdminLog>> CurrentRoundLogs([Nullable(2)] LogFilter filter = null)
		{
			if (filter == null)
			{
				filter = new LogFilter();
			}
			filter.Round = new int?(this._currentRoundId);
			return this.All(filter, null);
		}

		// Token: 0x06002D6A RID: 11626 RVA: 0x000EF21E File Offset: 0x000ED41E
		public IAsyncEnumerable<string> CurrentRoundMessages([Nullable(2)] LogFilter filter = null)
		{
			if (filter == null)
			{
				filter = new LogFilter();
			}
			filter.Round = new int?(this._currentRoundId);
			return this.AllMessages(filter);
		}

		// Token: 0x06002D6B RID: 11627 RVA: 0x000EF242 File Offset: 0x000ED442
		public IAsyncEnumerable<JsonDocument> CurrentRoundJson([Nullable(2)] LogFilter filter = null)
		{
			if (filter == null)
			{
				filter = new LogFilter();
			}
			filter.Round = new int?(this._currentRoundId);
			return this.AllJson(filter);
		}

		// Token: 0x06002D6C RID: 11628 RVA: 0x000EF266 File Offset: 0x000ED466
		public Task<Round> CurrentRound()
		{
			return this.Round(this._currentRoundId);
		}

		// Token: 0x06002D6D RID: 11629 RVA: 0x000EF274 File Offset: 0x000ED474
		private void InitializeJson()
		{
			this._jsonOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = AdminLogManager.NamingPolicy
			};
			foreach (Type converter2 in this._reflection.FindTypesWithAttribute<AdminLogConverterAttribute>())
			{
				JsonConverter instance = DynamicTypeFactoryExt.CreateInstance<JsonConverter>(this._typeFactory, converter2);
				IAdminLogConverter adminLogConverter = instance as IAdminLogConverter;
				if (adminLogConverter != null)
				{
					adminLogConverter.Init(this._dependencies);
				}
				this._jsonOptions.Converters.Add(instance);
			}
			IEnumerable<string> converterNames = from converter in this._jsonOptions.Converters
			select converter.GetType().Name;
			this._sawmill.Debug("Admin log converters found: " + string.Join(" ", converterNames));
		}

		// Token: 0x06002D6E RID: 11630 RVA: 0x000EF35C File Offset: 0x000ED55C
		[return: TupleElementNames(new string[]
		{
			"json",
			"players",
			"entities"
		})]
		[return: Nullable(new byte[]
		{
			0,
			1,
			1,
			1,
			2
		})]
		private ValueTuple<JsonDocument, HashSet<Guid>, Dictionary<int, string>> ToJson([Nullable(new byte[]
		{
			1,
			1,
			2
		})] Dictionary<string, object> properties)
		{
			Dictionary<int, string> entities = new Dictionary<int, string>();
			HashSet<Guid> players = new HashSet<Guid>();
			Dictionary<string, object> parsed = new Dictionary<string, object>();
			foreach (string key in properties.Keys)
			{
				object value = properties[key];
				IPlayerSession player = value as IPlayerSession;
				object obj;
				if (player != null)
				{
					obj = new SerializablePlayer(player);
				}
				else
				{
					obj = value;
				}
				value = obj;
				string parsedKey = AdminLogManager.NamingPolicy.ConvertName(key);
				parsed.Add(parsedKey, value);
				obj = properties[key];
				EntityUid? entityUid;
				if (obj is EntityUid)
				{
					EntityUid id = (EntityUid)obj;
					entityUid = new EntityUid?(id);
				}
				else if (obj is EntityStringRepresentation)
				{
					entityUid = new EntityUid?(((EntityStringRepresentation)obj).Uid);
				}
				else
				{
					IPlayerSession session = obj as IPlayerSession;
					if (session != null)
					{
						EntityUid? attachedEntity = session.AttachedEntity;
						if (attachedEntity != null && attachedEntity.GetValueOrDefault().Valid)
						{
							entityUid = session.AttachedEntity;
							goto IL_127;
						}
					}
					IComponent component = obj as IComponent;
					if (component == null)
					{
						entityUid = null;
					}
					else
					{
						entityUid = new EntityUid?(component.Owner);
					}
				}
				IL_127:
				EntityUid? entityId = entityUid;
				if (entityId != null)
				{
					EntityUid uid = entityId.GetValueOrDefault();
					MetaDataComponent metadata;
					string entityName = this._entityManager.TryGetComponent<MetaDataComponent>(uid, ref metadata) ? metadata.EntityName : null;
					entities.TryAdd((int)uid, entityName);
					ActorComponent actor;
					if (this._entityManager.TryGetComponent<ActorComponent>(uid, ref actor))
					{
						players.Add(actor.PlayerSession.UserId.UserId);
					}
				}
			}
			return new ValueTuple<JsonDocument, HashSet<Guid>, Dictionary<int, string>>(JsonSerializer.SerializeToDocument<Dictionary<string, object>>(parsed, this._jsonOptions), players, entities);
		}

		// Token: 0x04001C03 RID: 7171
		private const int MaxRoundsCached = 3;

		// Token: 0x04001C04 RID: 7172
		private const int LogListInitialSize = 30000;

		// Token: 0x04001C05 RID: 7173
		private readonly int _logTypes = Enum.GetValues<LogType>().Length;

		// Token: 0x04001C06 RID: 7174
		private readonly Dictionary<int, List<SharedAdminLog>> _roundsLogCache = new Dictionary<int, List<SharedAdminLog>>(3);

		// Token: 0x04001C07 RID: 7175
		private static readonly Gauge CacheRoundCount = Metrics.CreateGauge("admin_logs_cache_round_count", "How many rounds are in cache.", null);

		// Token: 0x04001C08 RID: 7176
		private static readonly Gauge CacheLogCount = Metrics.CreateGauge("admin_logs_cache_log_count", "How many logs are in cache.", null);

		// Token: 0x04001C09 RID: 7177
		[Dependency]
		private readonly IConfigurationManager _configuration;

		// Token: 0x04001C0A RID: 7178
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04001C0B RID: 7179
		[Dependency]
		private readonly ILogManager _logManager;

		// Token: 0x04001C0C RID: 7180
		[Dependency]
		private readonly IServerDbManager _db;

		// Token: 0x04001C0D RID: 7181
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04001C0E RID: 7182
		[Dependency]
		private readonly IDynamicTypeFactory _typeFactory;

		// Token: 0x04001C0F RID: 7183
		[Dependency]
		private readonly IReflectionManager _reflection;

		// Token: 0x04001C10 RID: 7184
		[Dependency]
		private readonly IDependencyCollection _dependencies;

		// Token: 0x04001C11 RID: 7185
		public const string SawmillId = "admin.logs";

		// Token: 0x04001C12 RID: 7186
		private static readonly Histogram DatabaseUpdateTime = Metrics.CreateHistogram("admin_logs_database_time", "Time used to send logs to the database in ms", new HistogramConfiguration
		{
			Buckets = Histogram.LinearBuckets(0.0, 0.5, 20)
		});

		// Token: 0x04001C13 RID: 7187
		private static readonly Gauge Queue = Metrics.CreateGauge("admin_logs_queue", "How many logs are in the queue.", null);

		// Token: 0x04001C14 RID: 7188
		private static readonly Gauge PreRoundQueue = Metrics.CreateGauge("admin_logs_pre_round_queue", "How many logs are in the pre-round queue.", null);

		// Token: 0x04001C15 RID: 7189
		private static readonly Gauge QueueCapReached = Metrics.CreateGauge("admin_logs_queue_cap_reached", "Number of times the log queue cap has been reached in a round.", null);

		// Token: 0x04001C16 RID: 7190
		private static readonly Gauge PreRoundQueueCapReached = Metrics.CreateGauge("admin_logs_queue_cap_reached", "Number of times the pre-round log queue cap has been reached in a round.", null);

		// Token: 0x04001C17 RID: 7191
		private static readonly Gauge LogsSent = Metrics.CreateGauge("admin_logs_sent", "Amount of logs sent to the database in a round.", null);

		// Token: 0x04001C18 RID: 7192
		private ISawmill _sawmill;

		// Token: 0x04001C19 RID: 7193
		private bool _metricsEnabled;

		// Token: 0x04001C1A RID: 7194
		private bool _enabled;

		// Token: 0x04001C1B RID: 7195
		private TimeSpan _queueSendDelay;

		// Token: 0x04001C1C RID: 7196
		private int _queueMax;

		// Token: 0x04001C1D RID: 7197
		private int _preRoundQueueMax;

		// Token: 0x04001C1E RID: 7198
		private TimeSpan _nextUpdateTime;

		// Token: 0x04001C1F RID: 7199
		private readonly ConcurrentQueue<QueuedLog> _logQueue = new ConcurrentQueue<QueuedLog>();

		// Token: 0x04001C20 RID: 7200
		private readonly ConcurrentQueue<QueuedLog> _preRoundLogQueue = new ConcurrentQueue<QueuedLog>();

		// Token: 0x04001C21 RID: 7201
		private int _currentRoundId;

		// Token: 0x04001C22 RID: 7202
		private int _currentLogId;

		// Token: 0x04001C23 RID: 7203
		private GameRunLevel _runLevel;

		// Token: 0x04001C24 RID: 7204
		private static readonly JsonNamingPolicy NamingPolicy = JsonNamingPolicy.CamelCase;

		// Token: 0x04001C25 RID: 7205
		private JsonSerializerOptions _jsonOptions;
	}
}
