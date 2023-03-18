using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Dataset;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.RandomMetadata
{
	// Token: 0x02000255 RID: 597
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomMetadataSystem : EntitySystem
	{
		// Token: 0x06000BD8 RID: 3032 RVA: 0x0003E8BC File Offset: 0x0003CABC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RandomMetadataComponent, MapInitEvent>(new ComponentEventHandler<RandomMetadataComponent, MapInitEvent>(this.OnMapInit), null, null);
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x0003E8D8 File Offset: 0x0003CAD8
		private void OnMapInit(EntityUid uid, RandomMetadataComponent component, MapInitEvent args)
		{
			MetaDataComponent meta = base.MetaData(uid);
			if (component.NameSegments != null)
			{
				meta.EntityName = this.GetRandomFromSegments(component.NameSegments, component.NameSeparator);
			}
			if (component.DescriptionSegments != null)
			{
				meta.EntityDescription = this.GetRandomFromSegments(component.DescriptionSegments, component.DescriptionSeparator);
			}
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x0003E930 File Offset: 0x0003CB30
		public string GetRandomFromSegments(List<string> segments, [Nullable(2)] string separator)
		{
			List<string> outputSegments = new List<string>();
			foreach (string segment in segments)
			{
				DatasetPrototype proto;
				outputSegments.Add(this._prototype.TryIndex<DatasetPrototype>(segment, ref proto) ? RandomExtensions.Pick<string>(this._random, proto.Values) : segment);
			}
			return string.Join(separator, outputSegments);
		}

		// Token: 0x04000766 RID: 1894
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04000767 RID: 1895
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
