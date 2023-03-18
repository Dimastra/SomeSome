using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Client.Parallax.Data;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Parallax.Managers
{
	// Token: 0x020001DE RID: 478
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ParallaxManager : IParallaxManager
	{
		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06000C5E RID: 3166 RVA: 0x00048504 File Offset: 0x00046704
		// (set) Token: 0x06000C5F RID: 3167 RVA: 0x0004850C File Offset: 0x0004670C
		public Vector2 ParallaxAnchor { get; set; }

		// Token: 0x06000C60 RID: 3168 RVA: 0x00048515 File Offset: 0x00046715
		public bool IsLoaded(string name)
		{
			return this._parallaxesLQ.ContainsKey(name);
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x00048524 File Offset: 0x00046724
		public ParallaxLayerPrepared[] GetParallaxLayers(string name)
		{
			if (this._configurationManager.GetCVar<bool>(CCVars.ParallaxLowQuality))
			{
				ParallaxLayerPrepared[] result;
				if (this._parallaxesLQ.TryGetValue(name, out result))
				{
					return result;
				}
				return Array.Empty<ParallaxLayerPrepared>();
			}
			else
			{
				ParallaxLayerPrepared[] result2;
				if (this._parallaxesHQ.TryGetValue(name, out result2))
				{
					return result2;
				}
				return Array.Empty<ParallaxLayerPrepared>();
			}
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x00048574 File Offset: 0x00046774
		public void UnloadParallax(string name)
		{
			CancellationTokenSource cancellationTokenSource;
			if (this._loadingParallaxes.TryGetValue(name, out cancellationTokenSource))
			{
				cancellationTokenSource.Cancel();
				CancellationTokenSource cancellationTokenSource2;
				this._loadingParallaxes.Remove(name, out cancellationTokenSource2);
				return;
			}
			if (!this._parallaxesLQ.ContainsKey(name))
			{
				return;
			}
			this._parallaxesLQ.Remove(name);
			this._parallaxesHQ.Remove(name);
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x000485D0 File Offset: 0x000467D0
		public void LoadDefaultParallax()
		{
			ParallaxManager.<LoadDefaultParallax>d__13 <LoadDefaultParallax>d__;
			<LoadDefaultParallax>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LoadDefaultParallax>d__.<>4__this = this;
			<LoadDefaultParallax>d__.<>1__state = -1;
			<LoadDefaultParallax>d__.<>t__builder.Start<ParallaxManager.<LoadDefaultParallax>d__13>(ref <LoadDefaultParallax>d__);
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x00048608 File Offset: 0x00046808
		public Task LoadParallaxByName(string name)
		{
			ParallaxManager.<LoadParallaxByName>d__14 <LoadParallaxByName>d__;
			<LoadParallaxByName>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadParallaxByName>d__.<>4__this = this;
			<LoadParallaxByName>d__.name = name;
			<LoadParallaxByName>d__.<>1__state = -1;
			<LoadParallaxByName>d__.<>t__builder.Start<ParallaxManager.<LoadParallaxByName>d__14>(ref <LoadParallaxByName>d__);
			return <LoadParallaxByName>d__.<>t__builder.Task;
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x00048654 File Offset: 0x00046854
		private Task<ParallaxLayerPrepared[]> LoadParallaxLayers(List<ParallaxLayerConfig> layersIn, CancellationToken cancel = default(CancellationToken))
		{
			ParallaxManager.<LoadParallaxLayers>d__15 <LoadParallaxLayers>d__;
			<LoadParallaxLayers>d__.<>t__builder = AsyncTaskMethodBuilder<ParallaxLayerPrepared[]>.Create();
			<LoadParallaxLayers>d__.<>4__this = this;
			<LoadParallaxLayers>d__.layersIn = layersIn;
			<LoadParallaxLayers>d__.cancel = cancel;
			<LoadParallaxLayers>d__.<>1__state = -1;
			<LoadParallaxLayers>d__.<>t__builder.Start<ParallaxManager.<LoadParallaxLayers>d__15>(ref <LoadParallaxLayers>d__);
			return <LoadParallaxLayers>d__.<>t__builder.Task;
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x000486A8 File Offset: 0x000468A8
		private Task<ParallaxLayerPrepared> LoadParallaxLayer(ParallaxLayerConfig config, CancellationToken cancel = default(CancellationToken))
		{
			ParallaxManager.<LoadParallaxLayer>d__16 <LoadParallaxLayer>d__;
			<LoadParallaxLayer>d__.<>t__builder = AsyncTaskMethodBuilder<ParallaxLayerPrepared>.Create();
			<LoadParallaxLayer>d__.config = config;
			<LoadParallaxLayer>d__.cancel = cancel;
			<LoadParallaxLayer>d__.<>1__state = -1;
			<LoadParallaxLayer>d__.<>t__builder.Start<ParallaxManager.<LoadParallaxLayer>d__16>(ref <LoadParallaxLayer>d__);
			return <LoadParallaxLayer>d__.<>t__builder.Task;
		}

		// Token: 0x0400061C RID: 1564
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400061D RID: 1565
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x0400061E RID: 1566
		private ISawmill _sawmill = Logger.GetSawmill("parallax");

		// Token: 0x04000620 RID: 1568
		private readonly Dictionary<string, ParallaxLayerPrepared[]> _parallaxesLQ = new Dictionary<string, ParallaxLayerPrepared[]>();

		// Token: 0x04000621 RID: 1569
		private readonly Dictionary<string, ParallaxLayerPrepared[]> _parallaxesHQ = new Dictionary<string, ParallaxLayerPrepared[]>();

		// Token: 0x04000622 RID: 1570
		private readonly Dictionary<string, CancellationTokenSource> _loadingParallaxes = new Dictionary<string, CancellationTokenSource>();
	}
}
