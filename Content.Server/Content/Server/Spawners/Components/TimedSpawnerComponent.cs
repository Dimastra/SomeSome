using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.Spawners.Components
{
	// Token: 0x020001D9 RID: 473
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class TimedSpawnerComponent : Component, ISerializationHooks
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060008FC RID: 2300 RVA: 0x0002D86E File Offset: 0x0002BA6E
		// (set) Token: 0x060008FD RID: 2301 RVA: 0x0002D876 File Offset: 0x0002BA76
		[ViewVariables]
		[DataField("prototypes", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		public List<string> Prototypes { get; set; } = new List<string>();

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060008FE RID: 2302 RVA: 0x0002D87F File Offset: 0x0002BA7F
		// (set) Token: 0x060008FF RID: 2303 RVA: 0x0002D887 File Offset: 0x0002BA87
		[ViewVariables]
		[DataField("chance", false, 1, false, false, null)]
		public float Chance { get; set; } = 1f;

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000900 RID: 2304 RVA: 0x0002D890 File Offset: 0x0002BA90
		// (set) Token: 0x06000901 RID: 2305 RVA: 0x0002D898 File Offset: 0x0002BA98
		[ViewVariables]
		[DataField("intervalSeconds", false, 1, false, false, null)]
		public int IntervalSeconds { get; set; } = 60;

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000902 RID: 2306 RVA: 0x0002D8A1 File Offset: 0x0002BAA1
		// (set) Token: 0x06000903 RID: 2307 RVA: 0x0002D8A9 File Offset: 0x0002BAA9
		[ViewVariables]
		[DataField("MinimumEntitiesSpawned", false, 1, false, false, null)]
		public int MinimumEntitiesSpawned { get; set; } = 1;

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000904 RID: 2308 RVA: 0x0002D8B2 File Offset: 0x0002BAB2
		// (set) Token: 0x06000905 RID: 2309 RVA: 0x0002D8BA File Offset: 0x0002BABA
		[ViewVariables]
		[DataField("MaximumEntitiesSpawned", false, 1, false, false, null)]
		public int MaximumEntitiesSpawned { get; set; } = 1;

		// Token: 0x06000906 RID: 2310 RVA: 0x0002D8C3 File Offset: 0x0002BAC3
		void ISerializationHooks.AfterDeserialization()
		{
			if (this.MinimumEntitiesSpawned > this.MaximumEntitiesSpawned)
			{
				throw new ArgumentException("MaximumEntitiesSpawned can't be lower than MinimumEntitiesSpawned!");
			}
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x0002D8DE File Offset: 0x0002BADE
		protected override void Initialize()
		{
			base.Initialize();
			this.SetupTimer();
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x0002D8EC File Offset: 0x0002BAEC
		private void SetupTimer()
		{
			CancellationTokenSource tokenSource = this.TokenSource;
			if (tokenSource != null)
			{
				tokenSource.Cancel();
			}
			this.TokenSource = new CancellationTokenSource();
			TimerExtensions.SpawnRepeatingTimer(base.Owner, TimeSpan.FromSeconds((double)this.IntervalSeconds), new Action(this.OnTimerFired), this.TokenSource.Token);
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x0002D944 File Offset: 0x0002BB44
		private void OnTimerFired()
		{
			if (!RandomExtensions.Prob(this._robustRandom, this.Chance))
			{
				return;
			}
			int number = this._robustRandom.Next(this.MinimumEntitiesSpawned, this.MaximumEntitiesSpawned);
			for (int i = 0; i < number; i++)
			{
				string entity = RandomExtensions.Pick<string>(this._robustRandom, this.Prototypes);
				IoCManager.Resolve<IEntityManager>().SpawnEntity(entity, IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(base.Owner).Coordinates);
			}
		}

		// Token: 0x04000573 RID: 1395
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000579 RID: 1401
		[Nullable(2)]
		public CancellationTokenSource TokenSource;
	}
}
