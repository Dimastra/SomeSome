using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using Nett;
using Robust.Client.Utility;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Noise;
using Robust.Shared.Random;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Parallax
{
	// Token: 0x020001D3 RID: 467
	public sealed class ParallaxGenerator
	{
		// Token: 0x06000C35 RID: 3125 RVA: 0x000471CC File Offset: 0x000453CC
		[NullableContext(1)]
		public static Image<Rgba32> GenerateParallax(TomlTable config, Size size, ISawmill sawmill, [Nullable(new byte[]
		{
			2,
			1
		})] List<Image<Rgba32>> debugLayerDump, CancellationToken cancel = default(CancellationToken))
		{
			sawmill.Debug("Generating parallax!");
			ParallaxGenerator parallaxGenerator = new ParallaxGenerator();
			parallaxGenerator._loadConfig(config);
			sawmill.Debug("Timing start!");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Image<Rgba32> image = new Image<Rgba32>(Configuration.Default, size.Width, size.Height, new Rgba32(0, 0, 0, 0));
			int num = 0;
			foreach (ParallaxGenerator.Layer layer in parallaxGenerator.Layers)
			{
				cancel.ThrowIfCancellationRequested();
				layer.Apply(image);
				if (debugLayerDump != null)
				{
					debugLayerDump.Add(image.Clone());
				}
				sawmill.Debug("Layer {0} done!", new object[]
				{
					num++
				});
			}
			stopwatch.Stop();
			sawmill.Debug("Total time: {0}", new object[]
			{
				stopwatch.Elapsed.TotalSeconds
			});
			return image;
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x000472D4 File Offset: 0x000454D4
		[NullableContext(1)]
		private void _loadConfig(TomlTable config)
		{
			foreach (TomlTable tomlTable in ((TomlTableArray)config.Get("layers")).Items)
			{
				string value = ((TomlValue<string>)tomlTable.Get("type")).Value;
				if (!(value == "clear"))
				{
					if (!(value == "toalpha"))
					{
						if (!(value == "noise"))
						{
							if (!(value == "points"))
							{
								throw new NotSupportedException();
							}
							ParallaxGenerator.LayerPoints item = new ParallaxGenerator.LayerPoints(tomlTable);
							this.Layers.Add(item);
						}
						else
						{
							ParallaxGenerator.LayerNoise item2 = new ParallaxGenerator.LayerNoise(tomlTable);
							this.Layers.Add(item2);
						}
					}
					else
					{
						ParallaxGenerator.LayerToAlpha item3 = new ParallaxGenerator.LayerToAlpha(tomlTable);
						this.Layers.Add(item3);
					}
				}
				else
				{
					ParallaxGenerator.LayerClear item4 = new ParallaxGenerator.LayerClear(tomlTable);
					this.Layers.Add(item4);
				}
			}
		}

		// Token: 0x040005EF RID: 1519
		[Nullable(1)]
		private readonly List<ParallaxGenerator.Layer> Layers = new List<ParallaxGenerator.Layer>();

		// Token: 0x020001D4 RID: 468
		private abstract class Layer
		{
			// Token: 0x06000C38 RID: 3128
			[NullableContext(1)]
			public abstract void Apply(Image<Rgba32> bitmap);
		}

		// Token: 0x020001D5 RID: 469
		private abstract class LayerConversion : ParallaxGenerator.Layer
		{
			// Token: 0x06000C3A RID: 3130
			public abstract Color ConvertColor(Color input);

			// Token: 0x06000C3B RID: 3131 RVA: 0x000473F8 File Offset: 0x000455F8
			[NullableContext(1)]
			public unsafe override void Apply(Image<Rgba32> bitmap)
			{
				Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(bitmap);
				for (int i = 0; i < bitmap.Height; i++)
				{
					for (int j = 0; j < bitmap.Width; j++)
					{
						int index = i * bitmap.Width + j;
						*pixelSpan[index] = ImageSharpExt.ConvertImgSharp(this.ConvertColor(ImageSharpExt.ConvertImgSharp(*pixelSpan[index])));
					}
				}
			}
		}

		// Token: 0x020001D6 RID: 470
		private sealed class LayerClear : ParallaxGenerator.LayerConversion
		{
			// Token: 0x06000C3D RID: 3133 RVA: 0x0004746C File Offset: 0x0004566C
			[NullableContext(1)]
			public LayerClear(TomlTable table)
			{
				TomlObject tomlObject;
				if (table.TryGetValue("color", ref tomlObject))
				{
					this.Color = Color.FromHex(((TomlValue<string>)tomlObject).Value, null);
				}
			}

			// Token: 0x06000C3E RID: 3134 RVA: 0x000474BD File Offset: 0x000456BD
			public override Color ConvertColor(Color input)
			{
				return this.Color;
			}

			// Token: 0x040005F0 RID: 1520
			private readonly Color Color = Color.Black;
		}

		// Token: 0x020001D7 RID: 471
		private sealed class LayerToAlpha : ParallaxGenerator.LayerConversion
		{
			// Token: 0x06000C3F RID: 3135 RVA: 0x000474C5 File Offset: 0x000456C5
			[NullableContext(1)]
			public LayerToAlpha(TomlTable table)
			{
			}

			// Token: 0x06000C40 RID: 3136 RVA: 0x000474CD File Offset: 0x000456CD
			public override Color ConvertColor(Color input)
			{
				return new Color(input.R, input.G, input.B, MathF.Min(input.R + input.G + input.B, 1f));
			}
		}

		// Token: 0x020001D8 RID: 472
		[NullableContext(1)]
		[Nullable(0)]
		private sealed class LayerNoise : ParallaxGenerator.Layer
		{
			// Token: 0x06000C41 RID: 3137 RVA: 0x00047504 File Offset: 0x00045704
			public LayerNoise(TomlTable table)
			{
				TomlObject tomlObject;
				if (table.TryGetValue("innercolor", ref tomlObject))
				{
					this.InnerColor = Color.FromHex(((TomlValue<string>)tomlObject).Value, null);
				}
				if (table.TryGetValue("outercolor", ref tomlObject))
				{
					this.OuterColor = Color.FromHex(((TomlValue<string>)tomlObject).Value, null);
				}
				if (table.TryGetValue("seed", ref tomlObject))
				{
					this.Seed = (uint)((TomlValue<long>)tomlObject).Value;
				}
				if (table.TryGetValue("persistence", ref tomlObject))
				{
					this.Persistence = float.Parse(((TomlValue<string>)tomlObject).Value, CultureInfo.InvariantCulture);
				}
				if (table.TryGetValue("lacunarity", ref tomlObject))
				{
					this.Lacunarity = float.Parse(((TomlValue<string>)tomlObject).Value, CultureInfo.InvariantCulture);
				}
				if (table.TryGetValue("frequency", ref tomlObject))
				{
					this.Frequency = float.Parse(((TomlValue<string>)tomlObject).Value, CultureInfo.InvariantCulture);
				}
				if (table.TryGetValue("octaves", ref tomlObject))
				{
					this.Octaves = (uint)((TomlValue<long>)tomlObject).Value;
				}
				if (table.TryGetValue("threshold", ref tomlObject))
				{
					this.Threshold = float.Parse(((TomlValue<string>)tomlObject).Value, CultureInfo.InvariantCulture);
				}
				if (table.TryGetValue("sourcefactor", ref tomlObject))
				{
					this.SrcFactor = (Color.BlendFactor)Enum.Parse(typeof(Color.BlendFactor), ((TomlValue<string>)tomlObject).Value);
				}
				if (table.TryGetValue("destfactor", ref tomlObject))
				{
					this.DstFactor = (Color.BlendFactor)Enum.Parse(typeof(Color.BlendFactor), ((TomlValue<string>)tomlObject).Value);
				}
				if (table.TryGetValue("power", ref tomlObject))
				{
					this.Power = float.Parse(((TomlValue<string>)tomlObject).Value, CultureInfo.InvariantCulture);
				}
				if (!table.TryGetValue("noise_type", ref tomlObject))
				{
					return;
				}
				string value = ((TomlValue<string>)tomlObject).Value;
				if (value == "fbm")
				{
					this.NoiseType = 0;
					return;
				}
				if (!(value == "ridged"))
				{
					throw new InvalidOperationException();
				}
				this.NoiseType = 1;
			}

			// Token: 0x06000C42 RID: 3138 RVA: 0x000477A8 File Offset: 0x000459A8
			public unsafe override void Apply(Image<Rgba32> bitmap)
			{
				NoiseGenerator noiseGenerator = new NoiseGenerator(this.NoiseType);
				noiseGenerator.SetSeed(this.Seed);
				noiseGenerator.SetFrequency(this.Frequency);
				noiseGenerator.SetPersistence(this.Persistence);
				noiseGenerator.SetLacunarity(this.Lacunarity);
				noiseGenerator.SetOctaves(this.Octaves);
				noiseGenerator.SetPeriodX((float)bitmap.Width);
				noiseGenerator.SetPeriodY((float)bitmap.Height);
				float num = 1f / (1f - this.Threshold);
				float y = 1f / this.Power;
				Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(bitmap);
				for (int i = 0; i < bitmap.Height; i++)
				{
					for (int j = 0; j < bitmap.Width; j++)
					{
						float num2 = MathF.Min(1f, MathF.Max(0f, (noiseGenerator.GetNoiseTiled((float)j, (float)i) + 1f) / 2f));
						num2 = MathF.Max(0f, num2 - this.Threshold);
						num2 *= num;
						num2 = MathF.Pow(num2, y);
						Color color = Color.InterpolateBetween(this.OuterColor, this.InnerColor, num2).WithAlpha(num2);
						int index = i * bitmap.Width + j;
						Color color2 = ImageSharpExt.ConvertImgSharp(*pixelSpan[index]);
						*pixelSpan[index] = ImageSharpExt.ConvertImgSharp(Color.Blend(color2, color, this.DstFactor, this.SrcFactor));
					}
				}
			}

			// Token: 0x040005F1 RID: 1521
			private readonly Color InnerColor = Color.White;

			// Token: 0x040005F2 RID: 1522
			private readonly Color OuterColor = Color.Black;

			// Token: 0x040005F3 RID: 1523
			private readonly NoiseGenerator.NoiseType NoiseType;

			// Token: 0x040005F4 RID: 1524
			private readonly uint Seed = 1234U;

			// Token: 0x040005F5 RID: 1525
			private readonly float Persistence = 0.5f;

			// Token: 0x040005F6 RID: 1526
			private readonly float Lacunarity = 1.0471976f;

			// Token: 0x040005F7 RID: 1527
			private readonly float Frequency = 1f;

			// Token: 0x040005F8 RID: 1528
			private readonly uint Octaves = 3U;

			// Token: 0x040005F9 RID: 1529
			private readonly float Threshold;

			// Token: 0x040005FA RID: 1530
			private readonly float Power = 1f;

			// Token: 0x040005FB RID: 1531
			private readonly Color.BlendFactor SrcFactor = 1;

			// Token: 0x040005FC RID: 1532
			private readonly Color.BlendFactor DstFactor = 1;
		}

		// Token: 0x020001D9 RID: 473
		[NullableContext(1)]
		[Nullable(0)]
		private sealed class LayerPoints : ParallaxGenerator.Layer
		{
			// Token: 0x06000C43 RID: 3139 RVA: 0x00047934 File Offset: 0x00045B34
			public LayerPoints(TomlTable table)
			{
				TomlObject tomlObject;
				if (table.TryGetValue("seed", ref tomlObject))
				{
					this.Seed = (int)((TomlValue<long>)tomlObject).Value;
				}
				if (table.TryGetValue("count", ref tomlObject))
				{
					this.PointCount = (int)((TomlValue<long>)tomlObject).Value;
				}
				if (table.TryGetValue("sourcefactor", ref tomlObject))
				{
					this.SrcFactor = (Color.BlendFactor)Enum.Parse(typeof(Color.BlendFactor), ((TomlValue<string>)tomlObject).Value);
				}
				if (table.TryGetValue("destfactor", ref tomlObject))
				{
					this.DstFactor = (Color.BlendFactor)Enum.Parse(typeof(Color.BlendFactor), ((TomlValue<string>)tomlObject).Value);
				}
				if (table.TryGetValue("farcolor", ref tomlObject))
				{
					this.FarColor = Color.FromHex(((TomlValue<string>)tomlObject).Value, null);
				}
				if (table.TryGetValue("closecolor", ref tomlObject))
				{
					this.CloseColor = Color.FromHex(((TomlValue<string>)tomlObject).Value, null);
				}
				if (table.TryGetValue("pointsize", ref tomlObject))
				{
					this.PointSize = (int)((TomlValue<long>)tomlObject).Value;
				}
				if (table.TryGetValue("mask", ref tomlObject))
				{
					this.Masked = ((TomlValue<bool>)tomlObject).Value;
				}
				if (table.TryGetValue("maskseed", ref tomlObject))
				{
					this.MaskSeed = (uint)((TomlValue<long>)tomlObject).Value;
				}
				if (table.TryGetValue("maskpersistence", ref tomlObject))
				{
					this.MaskPersistence = float.Parse(((TomlValue<string>)tomlObject).Value, CultureInfo.InvariantCulture);
				}
				if (table.TryGetValue("masklacunarity", ref tomlObject))
				{
					this.MaskLacunarity = float.Parse(((TomlValue<string>)tomlObject).Value, CultureInfo.InvariantCulture);
				}
				if (table.TryGetValue("maskfrequency", ref tomlObject))
				{
					this.MaskFrequency = float.Parse(((TomlValue<string>)tomlObject).Value, CultureInfo.InvariantCulture);
				}
				if (table.TryGetValue("maskoctaves", ref tomlObject))
				{
					this.MaskOctaves = (uint)((TomlValue<long>)tomlObject).Value;
				}
				if (table.TryGetValue("maskthreshold", ref tomlObject))
				{
					this.MaskThreshold = float.Parse(((TomlValue<string>)tomlObject).Value, CultureInfo.InvariantCulture);
				}
				if (table.TryGetValue("masknoise_type", ref tomlObject))
				{
					string value = ((TomlValue<string>)tomlObject).Value;
					if (!(value == "fbm"))
					{
						if (!(value == "ridged"))
						{
							throw new InvalidOperationException();
						}
						this.MaskNoiseType = 1;
					}
					else
					{
						this.MaskNoiseType = 0;
					}
				}
				if (table.TryGetValue("maskpower", ref tomlObject))
				{
					this.MaskPower = float.Parse(((TomlValue<string>)tomlObject).Value, CultureInfo.InvariantCulture);
				}
			}

			// Token: 0x06000C44 RID: 3140 RVA: 0x00047C78 File Offset: 0x00045E78
			public unsafe override void Apply(Image<Rgba32> bitmap)
			{
				using (Image<Rgba32> image = new Image<Rgba32>(Configuration.Default, bitmap.Width, bitmap.Height, new Rgba32(0, 0, 0, 0)))
				{
					if (this.Masked)
					{
						this.GenPointsMasked(image);
					}
					else
					{
						this.GenPoints(image);
					}
					Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(image);
					Span<Rgba32> pixelSpan2 = ImageSharpExt.GetPixelSpan<Rgba32>(bitmap);
					int width = bitmap.Width;
					int height = bitmap.Height;
					for (int i = 0; i < height; i++)
					{
						for (int j = 0; j < width; j++)
						{
							int index = i * width + j;
							Color color = ImageSharpExt.ConvertImgSharp(*pixelSpan2[index]);
							Color color2 = ImageSharpExt.ConvertImgSharp(*pixelSpan[index]);
							*pixelSpan2[index] = ImageSharpExt.ConvertImgSharp(Color.Blend(color, color2, this.DstFactor, this.SrcFactor));
						}
					}
				}
			}

			// Token: 0x06000C45 RID: 3141 RVA: 0x00047D74 File Offset: 0x00045F74
			private unsafe void GenPoints(Image<Rgba32> buffer)
			{
				int num = this.PointSize - 1;
				Random random = new Random(this.Seed);
				Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(buffer);
				for (int i = 0; i < this.PointCount; i++)
				{
					int num2 = random.Next(0, buffer.Width);
					int num3 = random.Next(0, buffer.Height);
					float num4 = RandomExtensions.NextFloat(random);
					for (int j = num3 - num; j <= num3 + num; j++)
					{
						for (int k = num2 - num; k <= num2 + num; k++)
						{
							int num5 = MathHelper.Mod(k, buffer.Width);
							int num6 = MathHelper.Mod(j, buffer.Height);
							Rgba32 rgba = ImageSharpExt.ConvertImgSharp(Color.InterpolateBetween(this.FarColor, this.CloseColor, num4));
							*pixelSpan[num6 * buffer.Width + num5] = rgba;
						}
					}
				}
			}

			// Token: 0x06000C46 RID: 3142 RVA: 0x00047E58 File Offset: 0x00046058
			private unsafe void GenPointsMasked(Image<Rgba32> buffer)
			{
				int num = this.PointSize - 1;
				Random random = new Random(this.Seed);
				NoiseGenerator noiseGenerator = new NoiseGenerator(this.MaskNoiseType);
				noiseGenerator.SetSeed(this.MaskSeed);
				noiseGenerator.SetFrequency(this.MaskFrequency);
				noiseGenerator.SetPersistence(this.MaskPersistence);
				noiseGenerator.SetLacunarity(this.MaskLacunarity);
				noiseGenerator.SetOctaves(this.MaskOctaves);
				noiseGenerator.SetPeriodX((float)buffer.Width);
				noiseGenerator.SetPeriodY((float)buffer.Height);
				float num2 = 1f / (1f - this.MaskThreshold);
				float y = 1f / this.MaskPower;
				int num3 = 0;
				Span<Rgba32> pixelSpan = ImageSharpExt.GetPixelSpan<Rgba32>(buffer);
				for (int i = 0; i < this.PointCount; i++)
				{
					int num4 = random.Next(0, buffer.Width);
					int num5 = random.Next(0, buffer.Height);
					float num6 = MathF.Min(1f, MathF.Max(0f, (noiseGenerator.GetNoiseTiled((float)num4, (float)num5) + 1f) / 2f));
					num6 = MathF.Max(0f, num6 - this.MaskThreshold);
					num6 *= num2;
					num6 = MathF.Pow(num6, y);
					if (RandomExtensions.NextFloat(random) > num6)
					{
						if (++num3 <= 9999)
						{
							i--;
						}
					}
					else
					{
						float num7 = RandomExtensions.NextFloat(random);
						for (int j = num5 - num; j <= num5 + num; j++)
						{
							for (int k = num4 - num; k <= num4 + num; k++)
							{
								int num8 = MathHelper.Mod(k, buffer.Width);
								int num9 = MathHelper.Mod(j, buffer.Height);
								Rgba32 rgba = ImageSharpExt.ConvertImgSharp(Color.InterpolateBetween(this.FarColor, this.CloseColor, num7));
								*pixelSpan[num9 * buffer.Width + num8] = rgba;
							}
						}
					}
				}
			}

			// Token: 0x040005FD RID: 1533
			private readonly int Seed = 1234;

			// Token: 0x040005FE RID: 1534
			private readonly int PointCount = 100;

			// Token: 0x040005FF RID: 1535
			private readonly Color CloseColor = Color.White;

			// Token: 0x04000600 RID: 1536
			private readonly Color FarColor = Color.Black;

			// Token: 0x04000601 RID: 1537
			private readonly Color.BlendFactor SrcFactor = 1;

			// Token: 0x04000602 RID: 1538
			private readonly Color.BlendFactor DstFactor = 1;

			// Token: 0x04000603 RID: 1539
			private readonly bool Masked;

			// Token: 0x04000604 RID: 1540
			private readonly NoiseGenerator.NoiseType MaskNoiseType;

			// Token: 0x04000605 RID: 1541
			private readonly uint MaskSeed = 1234U;

			// Token: 0x04000606 RID: 1542
			private readonly float MaskPersistence = 0.5f;

			// Token: 0x04000607 RID: 1543
			private readonly float MaskLacunarity = 2.0943952f;

			// Token: 0x04000608 RID: 1544
			private readonly float MaskFrequency = 1f;

			// Token: 0x04000609 RID: 1545
			private readonly uint MaskOctaves = 3U;

			// Token: 0x0400060A RID: 1546
			private readonly float MaskThreshold;

			// Token: 0x0400060B RID: 1547
			private readonly int PointSize = 1;

			// Token: 0x0400060C RID: 1548
			private readonly float MaskPower = 1f;
		}
	}
}
