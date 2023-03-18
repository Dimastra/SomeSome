using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.Utility;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Clickable
{
	// Token: 0x020003C1 RID: 961
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ClickMapManager : IClickMapManager, IPostInjectInit
	{
		// Token: 0x060017D7 RID: 6103 RVA: 0x0008946B File Offset: 0x0008766B
		public void PostInject()
		{
			this._resourceCache.OnRawTextureLoaded += this.OnRawTextureLoaded;
			this._resourceCache.OnRsiLoaded += this.OnOnRsiLoaded;
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x0008949C File Offset: 0x0008769C
		private void OnOnRsiLoaded(RsiLoadedEventArgs obj)
		{
			Image<Rgba32> image = obj.Atlas as Image<Rgba32>;
			if (image != null)
			{
				ClickMapManager.RsiClickMapData value = new ClickMapManager.RsiClickMapData(ClickMapManager.ClickMap.FromImage<Rgba32>(image, 0.25f), obj.AtlasOffsets);
				this._rsiMaps[obj.Resource.RSI] = value;
			}
		}

		// Token: 0x060017D9 RID: 6105 RVA: 0x000894EC File Offset: 0x000876EC
		private void OnRawTextureLoaded(TextureLoadedEventArgs obj)
		{
			Image<Rgba32> image = obj.Image as Image<Rgba32>;
			if (image != null)
			{
				string text = obj.Path.ToString();
				foreach (string value in ClickMapManager.IgnoreTexturePaths)
				{
					if (text.StartsWith(value, StringComparison.Ordinal))
					{
						return;
					}
				}
				this._textureMaps[obj.Resource] = ClickMapManager.ClickMap.FromImage<Rgba32>(image, 0.25f);
			}
		}

		// Token: 0x060017DA RID: 6106 RVA: 0x00089560 File Offset: 0x00087760
		public bool IsOccluding(Texture texture, Vector2i pos)
		{
			ClickMapManager.ClickMap clickMap;
			return this._textureMaps.TryGetValue(texture, out clickMap) && ClickMapManager.SampleClickMap(clickMap, pos, clickMap.Size, Vector2i.Zero);
		}

		// Token: 0x060017DB RID: 6107 RVA: 0x00089594 File Offset: 0x00087794
		public bool IsOccluding(RSI rsi, RSI.StateId state, RSI.State.Direction dir, int frame, Vector2i pos)
		{
			ClickMapManager.RsiClickMapData rsiClickMapData;
			if (!this._rsiMaps.TryGetValue(rsi, out rsiClickMapData))
			{
				return false;
			}
			Vector2i[][] array;
			if (!rsiClickMapData.Offsets.TryGetValue(state, out array) || array.Length <= dir)
			{
				return false;
			}
			Vector2i[] array2 = array[dir];
			if (array2.Length <= frame)
			{
				return false;
			}
			Vector2i offset = array2[frame];
			return ClickMapManager.SampleClickMap(rsiClickMapData.ClickMap, pos, rsi.Size, offset);
		}

		// Token: 0x060017DC RID: 6108 RVA: 0x000895F8 File Offset: 0x000877F8
		private static bool SampleClickMap(ClickMapManager.ClickMap map, Vector2i pos, Vector2i bounds, Vector2i offset)
		{
			Vector2i vector2i = bounds;
			int num;
			int num2;
			vector2i.Deconstruct(ref num, ref num2);
			int num3 = num;
			int num4 = num2;
			vector2i = pos;
			vector2i.Deconstruct(ref num2, ref num);
			int num5 = num2;
			int num6 = num;
			for (int i = -2; i <= 2; i++)
			{
				int num7 = num5 + i;
				if (num7 >= 0 && num7 < num3)
				{
					for (int j = -2; j <= 2; j++)
					{
						int num8 = num6 + j;
						if (num8 >= 0 && num8 < num4 && map.IsOccluded(new ValueTuple<int, int>(num7, num8) + offset))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x04000C27 RID: 3111
		private static readonly string[] IgnoreTexturePaths = new string[]
		{
			"/Textures/Interface",
			"/Textures/LobbyScreens",
			"/Textures/Parallaxes",
			"/Textures/Logo"
		};

		// Token: 0x04000C28 RID: 3112
		private const float Threshold = 0.25f;

		// Token: 0x04000C29 RID: 3113
		private const int ClickRadius = 2;

		// Token: 0x04000C2A RID: 3114
		[Dependency]
		private readonly IResourceCache _resourceCache;

		// Token: 0x04000C2B RID: 3115
		[ViewVariables]
		private readonly Dictionary<Texture, ClickMapManager.ClickMap> _textureMaps = new Dictionary<Texture, ClickMapManager.ClickMap>();

		// Token: 0x04000C2C RID: 3116
		[ViewVariables]
		private readonly Dictionary<RSI, ClickMapManager.RsiClickMapData> _rsiMaps = new Dictionary<RSI, ClickMapManager.RsiClickMapData>();

		// Token: 0x020003C2 RID: 962
		[Nullable(0)]
		private sealed class RsiClickMapData
		{
			// Token: 0x060017DF RID: 6111 RVA: 0x000896D9 File Offset: 0x000878D9
			public RsiClickMapData(ClickMapManager.ClickMap clickMap, Dictionary<RSI.StateId, Vector2i[][]> offsets)
			{
				this.ClickMap = clickMap;
				this.Offsets = offsets;
			}

			// Token: 0x04000C2D RID: 3117
			public readonly ClickMapManager.ClickMap ClickMap;

			// Token: 0x04000C2E RID: 3118
			public readonly Dictionary<RSI.StateId, Vector2i[][]> Offsets;
		}

		// Token: 0x020003C3 RID: 963
		[Nullable(0)]
		internal sealed class ClickMap
		{
			// Token: 0x170004E9 RID: 1257
			// (get) Token: 0x060017E0 RID: 6112 RVA: 0x000896EF File Offset: 0x000878EF
			public int Width { get; }

			// Token: 0x170004EA RID: 1258
			// (get) Token: 0x060017E1 RID: 6113 RVA: 0x000896F7 File Offset: 0x000878F7
			public int Height { get; }

			// Token: 0x170004EB RID: 1259
			// (get) Token: 0x060017E2 RID: 6114 RVA: 0x000896FF File Offset: 0x000878FF
			[ViewVariables]
			public Vector2i Size
			{
				get
				{
					return new ValueTuple<int, int>(this.Width, this.Height);
				}
			}

			// Token: 0x060017E3 RID: 6115 RVA: 0x00089718 File Offset: 0x00087918
			public bool IsOccluded(int x, int y)
			{
				int num = y * this.Width + x;
				return ((int)this._data[num / 8] & 1 << num % 8) != 0;
			}

			// Token: 0x060017E4 RID: 6116 RVA: 0x00089748 File Offset: 0x00087948
			public bool IsOccluded(Vector2i vector)
			{
				Vector2i vector2i = vector;
				int num;
				int num2;
				vector2i.Deconstruct(ref num, ref num2);
				int x = num;
				int y = num2;
				return this.IsOccluded(x, y);
			}

			// Token: 0x060017E5 RID: 6117 RVA: 0x0008976F File Offset: 0x0008796F
			private ClickMap(byte[] data, int width, int height)
			{
				this.Width = width;
				this.Height = height;
				this._data = data;
			}

			// Token: 0x060017E6 RID: 6118 RVA: 0x0008978C File Offset: 0x0008798C
			[NullableContext(0)]
			[return: Nullable(1)]
			public static ClickMapManager.ClickMap FromImage<[IsUnmanaged] T>([Nullable(new byte[]
			{
				1,
				0
			})] Image<T> image, float threshold) where T : struct, ValueType, IPixel<T>
			{
				byte b = (byte)(threshold * 255f);
				int width = image.Width;
				int height = image.Height;
				byte[] array = new byte[(int)Math.Ceiling((double)((float)(width * height) / 8f))];
				Span<T> pixelSpan = ImageSharpExt.GetPixelSpan<T>(image);
				for (int i = 0; i < pixelSpan.Length; i++)
				{
					Rgba32 rgba = default(Rgba32);
					pixelSpan[i].ToRgba32(ref rgba);
					if (rgba.A >= b)
					{
						byte[] array2 = array;
						int num = i / 8;
						array2[num] |= (byte)(1 << i % 8);
					}
				}
				return new ClickMapManager.ClickMap(array, width, height);
			}

			// Token: 0x060017E7 RID: 6119 RVA: 0x00089830 File Offset: 0x00087A30
			public string DumpText()
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.Height; i++)
				{
					for (int j = 0; j < this.Width; j++)
					{
						stringBuilder.Append(this.IsOccluded(j, i) ? "1" : "0");
					}
					stringBuilder.AppendLine();
				}
				return stringBuilder.ToString();
			}

			// Token: 0x04000C2F RID: 3119
			[ViewVariables]
			private readonly byte[] _data;
		}
	}
}
