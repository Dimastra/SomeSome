using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Markings;
using Content.Shared.IoC;
using Content.Shared.Maps;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Entry
{
	// Token: 0x020004B6 RID: 1206
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EntryPoint : GameShared
	{
		// Token: 0x06000E97 RID: 3735 RVA: 0x0002F0FC File Offset: 0x0002D2FC
		public override void PreInit()
		{
			IoCManager.InjectDependencies<EntryPoint>(this);
			SharedContentIoC.Register();
		}

		// Token: 0x06000E98 RID: 3736 RVA: 0x0002F10A File Offset: 0x0002D30A
		public override void Init()
		{
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x0002F10C File Offset: 0x0002D30C
		public override void PostInit()
		{
			base.PostInit();
			this.InitTileDefinitions();
			IoCManager.Resolve<MarkingManager>().Initialize();
			IoCManager.Resolve<IConfigurationManager>();
		}

		// Token: 0x06000E9A RID: 3738 RVA: 0x0002F12C File Offset: 0x0002D32C
		private void InitTileDefinitions()
		{
			ContentTileDefinition spaceDef = this._prototypeManager.Index<ContentTileDefinition>("Space");
			this._tileDefinitionManager.Register(spaceDef);
			List<ContentTileDefinition> prototypeList = new List<ContentTileDefinition>();
			foreach (ContentTileDefinition tileDef in this._prototypeManager.EnumeratePrototypes<ContentTileDefinition>())
			{
				if (!(tileDef.ID == "Space"))
				{
					prototypeList.Add(tileDef);
				}
			}
			prototypeList.Sort((ContentTileDefinition a, ContentTileDefinition b) => string.Compare(a.ID, b.ID, StringComparison.Ordinal));
			foreach (ContentTileDefinition tileDef2 in prototypeList)
			{
				this._tileDefinitionManager.Register(tileDef2);
			}
			this._tileDefinitionManager.Initialize();
		}

		// Token: 0x04000DBE RID: 3518
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000DBF RID: 3519
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;
	}
}
