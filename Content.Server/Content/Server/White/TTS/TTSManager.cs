using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using Prometheus;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Content.Server.White.TTS
{
	// Token: 0x02000084 RID: 132
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TTSManager
	{
		// Token: 0x060001EA RID: 490 RVA: 0x0000AC12 File Offset: 0x00008E12
		public void Initialize()
		{
			this._sawmill = Logger.GetSawmill("tts");
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000AC24 File Offset: 0x00008E24
		public Task<byte[]> ConvertTextToSpeech(string entityName, string speaker, string text)
		{
			TTSManager.<ConvertTextToSpeech>d__9 <ConvertTextToSpeech>d__;
			<ConvertTextToSpeech>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
			<ConvertTextToSpeech>d__.<>4__this = this;
			<ConvertTextToSpeech>d__.entityName = entityName;
			<ConvertTextToSpeech>d__.speaker = speaker;
			<ConvertTextToSpeech>d__.text = text;
			<ConvertTextToSpeech>d__.<>1__state = -1;
			<ConvertTextToSpeech>d__.<>t__builder.Start<TTSManager.<ConvertTextToSpeech>d__9>(ref <ConvertTextToSpeech>d__);
			return <ConvertTextToSpeech>d__.<>t__builder.Task;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000AC80 File Offset: 0x00008E80
		private static string CreateRequestLink(string url, TTSManager.GenerateVoiceRequest body)
		{
			UriBuilder uriBuilder = new UriBuilder(url);
			NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
			query["ckey"] = body.Ckey;
			query["speaker"] = body.Speaker;
			query["text"] = body.Text;
			query["file"] = "1";
			uriBuilder.Query = query.ToString();
			return uriBuilder.ToString();
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000ACF3 File Offset: 0x00008EF3
		public void ResetCache()
		{
			this._cache.Clear();
			TTSManager.CachedCount.Set(0.0);
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000AD14 File Offset: 0x00008F14
		private string GenerateCacheKey(string speaker, string text)
		{
			string key = speaker + "/" + text;
			byte[] keyData = Encoding.UTF8.GetBytes(key);
			return Convert.ToHexString(SHA256.Create().ComputeHash(keyData));
		}

		// Token: 0x04000164 RID: 356
		private static readonly Histogram RequestTimings = Metrics.CreateHistogram("tts_req_timings", "Timings of TTS API requests", new HistogramConfiguration
		{
			LabelNames = new string[]
			{
				"type"
			},
			Buckets = Histogram.ExponentialBuckets(0.1, 1.5, 10)
		});

		// Token: 0x04000165 RID: 357
		private static readonly Counter WantedCount = Metrics.CreateCounter("tts_wanted_count", "Amount of wanted TTS audio.", null);

		// Token: 0x04000166 RID: 358
		private static readonly Counter ReusedCount = Metrics.CreateCounter("tts_reused_count", "Amount of reused TTS audio from cache.", null);

		// Token: 0x04000167 RID: 359
		private static readonly Gauge CachedCount = Metrics.CreateGauge("tts_cached_count", "Amount of cached TTS audio.", null);

		// Token: 0x04000168 RID: 360
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000169 RID: 361
		private readonly HttpClient _httpClient = new HttpClient();

		// Token: 0x0400016A RID: 362
		private ISawmill _sawmill;

		// Token: 0x0400016B RID: 363
		private readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();

		// Token: 0x020008A0 RID: 2208
		[Nullable(0)]
		private class GenerateVoiceRequest : IEquatable<TTSManager.GenerateVoiceRequest>
		{
			// Token: 0x170007EB RID: 2027
			// (get) Token: 0x06002FEB RID: 12267 RVA: 0x000F7643 File Offset: 0x000F5843
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(TTSManager.GenerateVoiceRequest);
				}
			}

			// Token: 0x170007EC RID: 2028
			// (get) Token: 0x06002FEC RID: 12268 RVA: 0x000F764F File Offset: 0x000F584F
			// (set) Token: 0x06002FED RID: 12269 RVA: 0x000F7657 File Offset: 0x000F5857
			[JsonPropertyName("text")]
			public string Text { get; set; }

			// Token: 0x170007ED RID: 2029
			// (get) Token: 0x06002FEE RID: 12270 RVA: 0x000F7660 File Offset: 0x000F5860
			// (set) Token: 0x06002FEF RID: 12271 RVA: 0x000F7668 File Offset: 0x000F5868
			[JsonPropertyName("speaker")]
			public string Speaker { get; set; }

			// Token: 0x170007EE RID: 2030
			// (get) Token: 0x06002FF0 RID: 12272 RVA: 0x000F7671 File Offset: 0x000F5871
			// (set) Token: 0x06002FF1 RID: 12273 RVA: 0x000F7679 File Offset: 0x000F5879
			[JsonPropertyName("ckey")]
			public string Ckey { get; set; }

			// Token: 0x06002FF2 RID: 12274 RVA: 0x000F7684 File Offset: 0x000F5884
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("GenerateVoiceRequest");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06002FF3 RID: 12275 RVA: 0x000F76D0 File Offset: 0x000F58D0
			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("Text = ");
				builder.Append(this.Text);
				builder.Append(", Speaker = ");
				builder.Append(this.Speaker);
				builder.Append(", Ckey = ");
				builder.Append(this.Ckey);
				return true;
			}

			// Token: 0x06002FF4 RID: 12276 RVA: 0x000F772E File Offset: 0x000F592E
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(TTSManager.GenerateVoiceRequest left, TTSManager.GenerateVoiceRequest right)
			{
				return !(left == right);
			}

			// Token: 0x06002FF5 RID: 12277 RVA: 0x000F773A File Offset: 0x000F593A
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(TTSManager.GenerateVoiceRequest left, TTSManager.GenerateVoiceRequest right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			// Token: 0x06002FF6 RID: 12278 RVA: 0x000F7750 File Offset: 0x000F5950
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Text>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Speaker>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Ckey>k__BackingField);
			}

			// Token: 0x06002FF7 RID: 12279 RVA: 0x000F77B2 File Offset: 0x000F59B2
			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as TTSManager.GenerateVoiceRequest);
			}

			// Token: 0x06002FF8 RID: 12280 RVA: 0x000F77C0 File Offset: 0x000F59C0
			[NullableContext(2)]
			[CompilerGenerated]
			public virtual bool Equals(TTSManager.GenerateVoiceRequest other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.<Text>k__BackingField, other.<Text>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<Speaker>k__BackingField, other.<Speaker>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<Ckey>k__BackingField, other.<Ckey>k__BackingField));
			}

			// Token: 0x06002FFA RID: 12282 RVA: 0x000F7839 File Offset: 0x000F5A39
			[CompilerGenerated]
			protected GenerateVoiceRequest(TTSManager.GenerateVoiceRequest original)
			{
				this.Text = original.<Text>k__BackingField;
				this.Speaker = original.<Speaker>k__BackingField;
				this.Ckey = original.<Ckey>k__BackingField;
			}

			// Token: 0x06002FFB RID: 12283 RVA: 0x000F7865 File Offset: 0x000F5A65
			public GenerateVoiceRequest()
			{
			}
		}

		// Token: 0x020008A1 RID: 2209
		[Nullable(0)]
		private struct GenerateVoiceResponse
		{
			// Token: 0x170007EF RID: 2031
			// (get) Token: 0x06002FFC RID: 12284 RVA: 0x000F786D File Offset: 0x000F5A6D
			// (set) Token: 0x06002FFD RID: 12285 RVA: 0x000F7875 File Offset: 0x000F5A75
			[JsonPropertyName("results")]
			public List<TTSManager.VoiceResult> Results { readonly get; set; }

			// Token: 0x170007F0 RID: 2032
			// (get) Token: 0x06002FFE RID: 12286 RVA: 0x000F787E File Offset: 0x000F5A7E
			// (set) Token: 0x06002FFF RID: 12287 RVA: 0x000F7886 File Offset: 0x000F5A86
			[JsonPropertyName("original_sha1")]
			public string Hash { readonly get; set; }
		}

		// Token: 0x020008A2 RID: 2210
		[Nullable(0)]
		private struct VoiceResult
		{
			// Token: 0x170007F1 RID: 2033
			// (get) Token: 0x06003000 RID: 12288 RVA: 0x000F788F File Offset: 0x000F5A8F
			// (set) Token: 0x06003001 RID: 12289 RVA: 0x000F7897 File Offset: 0x000F5A97
			[JsonPropertyName("audio")]
			public string Audio { readonly get; set; }
		}
	}
}
