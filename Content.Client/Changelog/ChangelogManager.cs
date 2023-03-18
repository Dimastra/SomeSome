using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Client.Changelog
{
	// Token: 0x020003EE RID: 1006
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChangelogManager
	{
		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x060018BF RID: 6335 RVA: 0x0008E528 File Offset: 0x0008C728
		// (set) Token: 0x060018C0 RID: 6336 RVA: 0x0008E530 File Offset: 0x0008C730
		public bool NewChangelogEntries { get; private set; }

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x060018C1 RID: 6337 RVA: 0x0008E539 File Offset: 0x0008C739
		// (set) Token: 0x060018C2 RID: 6338 RVA: 0x0008E541 File Offset: 0x0008C741
		public int LastReadId { get; private set; }

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x060018C3 RID: 6339 RVA: 0x0008E54A File Offset: 0x0008C74A
		// (set) Token: 0x060018C4 RID: 6340 RVA: 0x0008E552 File Offset: 0x0008C752
		public int MaxId { get; private set; }

		// Token: 0x14000093 RID: 147
		// (add) Token: 0x060018C5 RID: 6341 RVA: 0x0008E55C File Offset: 0x0008C75C
		// (remove) Token: 0x060018C6 RID: 6342 RVA: 0x0008E594 File Offset: 0x0008C794
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action NewChangelogEntriesChanged;

		// Token: 0x060018C7 RID: 6343 RVA: 0x0008E5CC File Offset: 0x0008C7CC
		public void SaveNewReadId()
		{
			this.NewChangelogEntries = false;
			Action newChangelogEntriesChanged = this.NewChangelogEntriesChanged;
			if (newChangelogEntriesChanged != null)
			{
				newChangelogEntriesChanged();
			}
			using (StreamWriter streamWriter = WritableDirProviderExt.OpenWriteText(this._resource.UserData, new ResourcePath("/changelog_last_seen_" + this._configManager.GetCVar<string>(CCVars.ServerId), "/")))
			{
				streamWriter.Write(this.MaxId.ToString());
			}
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x0008E658 File Offset: 0x0008C858
		public void Initialize()
		{
			ChangelogManager.<Initialize>d__19 <Initialize>d__;
			<Initialize>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Initialize>d__.<>4__this = this;
			<Initialize>d__.<>1__state = -1;
			<Initialize>d__.<>t__builder.Start<ChangelogManager.<Initialize>d__19>(ref <Initialize>d__);
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x0008E690 File Offset: 0x0008C890
		public Task<List<ChangelogManager.ChangelogEntry>> LoadChangelog()
		{
			ChangelogManager.<LoadChangelog>d__20 <LoadChangelog>d__;
			<LoadChangelog>d__.<>t__builder = AsyncTaskMethodBuilder<List<ChangelogManager.ChangelogEntry>>.Create();
			<LoadChangelog>d__.<>4__this = this;
			<LoadChangelog>d__.<>1__state = -1;
			<LoadChangelog>d__.<>t__builder.Start<ChangelogManager.<LoadChangelog>d__20>(ref <LoadChangelog>d__);
			return <LoadChangelog>d__.<>t__builder.Task;
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x0008E6D3 File Offset: 0x0008C8D3
		private Task<List<ChangelogManager.ChangelogEntry>> LoadChangelogFile(ResourcePath path)
		{
			return Task.Run<List<ChangelogManager.ChangelogEntry>>(delegate()
			{
				YamlStream yamlStream = this._resource.ContentFileReadYaml(path);
				if (yamlStream.Documents.Count == 0)
				{
					return new List<ChangelogManager.ChangelogEntry>();
				}
				MappingDataNode mappingDataNode = (MappingDataNode)YamlNodeHelpers.ToDataNode(yamlStream.Documents[0].RootNode);
				return this._serialization.Read<List<ChangelogManager.ChangelogEntry>>(mappingDataNode["Entries"], null, false, null, true);
			});
		}

		// Token: 0x04000C9D RID: 3229
		[Dependency]
		private readonly IResourceManager _resource;

		// Token: 0x04000C9E RID: 3230
		[Dependency]
		private readonly ISerializationManager _serialization;

		// Token: 0x04000C9F RID: 3231
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x020003EF RID: 1007
		[Nullable(0)]
		[DataDefinition]
		public sealed class ChangelogEntry : ISerializationHooks
		{
			// Token: 0x17000519 RID: 1305
			// (get) Token: 0x060018CC RID: 6348 RVA: 0x0008E6F8 File Offset: 0x0008C8F8
			// (set) Token: 0x060018CD RID: 6349 RVA: 0x0008E700 File Offset: 0x0008C900
			[DataField("id", false, 1, false, false, null)]
			public int Id { get; private set; }

			// Token: 0x1700051A RID: 1306
			// (get) Token: 0x060018CE RID: 6350 RVA: 0x0008E709 File Offset: 0x0008C909
			[DataField("author", false, 1, false, false, null)]
			public string Author { get; } = "";

			// Token: 0x1700051B RID: 1307
			// (get) Token: 0x060018CF RID: 6351 RVA: 0x0008E711 File Offset: 0x0008C911
			// (set) Token: 0x060018D0 RID: 6352 RVA: 0x0008E719 File Offset: 0x0008C919
			public DateTime Time { get; private set; }

			// Token: 0x1700051C RID: 1308
			// (get) Token: 0x060018D1 RID: 6353 RVA: 0x0008E722 File Offset: 0x0008C922
			[DataField("changes", false, 1, false, false, null)]
			public List<ChangelogManager.ChangelogChange> Changes { get; }

			// Token: 0x060018D2 RID: 6354 RVA: 0x0008E72A File Offset: 0x0008C92A
			void ISerializationHooks.AfterDeserialization()
			{
				this.Time = DateTime.Parse(this._time, null, DateTimeStyles.RoundtripKind);
			}

			// Token: 0x04000CA6 RID: 3238
			[DataField("time", false, 1, false, false, null)]
			private string _time;
		}

		// Token: 0x020003F0 RID: 1008
		[Nullable(0)]
		[DataDefinition]
		public sealed class ChangelogChange : ISerializationHooks
		{
			// Token: 0x1700051D RID: 1309
			// (get) Token: 0x060018D4 RID: 6356 RVA: 0x0008E756 File Offset: 0x0008C956
			// (set) Token: 0x060018D5 RID: 6357 RVA: 0x0008E75E File Offset: 0x0008C95E
			[DataField("type", false, 1, false, false, null)]
			public ChangelogManager.ChangelogLineType Type { get; private set; }

			// Token: 0x1700051E RID: 1310
			// (get) Token: 0x060018D6 RID: 6358 RVA: 0x0008E767 File Offset: 0x0008C967
			// (set) Token: 0x060018D7 RID: 6359 RVA: 0x0008E76F File Offset: 0x0008C96F
			[DataField("message", false, 1, false, false, null)]
			public string Message { get; private set; } = "";
		}

		// Token: 0x020003F1 RID: 1009
		[NullableContext(0)]
		public enum ChangelogLineType
		{
			// Token: 0x04000CAC RID: 3244
			Add,
			// Token: 0x04000CAD RID: 3245
			Remove,
			// Token: 0x04000CAE RID: 3246
			Fix,
			// Token: 0x04000CAF RID: 3247
			Tweak
		}
	}
}
