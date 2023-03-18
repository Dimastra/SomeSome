using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Sprite;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Reflection;

namespace Content.Client.Sprite
{
	// Token: 0x02000137 RID: 311
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomSpriteSystem : SharedRandomSpriteSystem
	{
		// Token: 0x06000850 RID: 2128 RVA: 0x000304B3 File Offset: 0x0002E6B3
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RandomSpriteComponent, ComponentHandleState>(new ComponentEventRefHandler<RandomSpriteComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x000304D0 File Offset: 0x0002E6D0
		private void OnHandleState(EntityUid uid, RandomSpriteComponent component, ref ComponentHandleState args)
		{
			RandomSpriteColorComponentState randomSpriteColorComponentState = args.Current as RandomSpriteColorComponentState;
			if (randomSpriteColorComponentState == null)
			{
				return;
			}
			if (randomSpriteColorComponentState.Selected.Equals(component.Selected))
			{
				return;
			}
			component.Selected.Clear();
			component.Selected.EnsureCapacity(randomSpriteColorComponentState.Selected.Count);
			foreach (KeyValuePair<string, ValueTuple<string, Color?>> keyValuePair in randomSpriteColorComponentState.Selected)
			{
				component.Selected.Add(keyValuePair.Key, keyValuePair.Value);
			}
			this.UpdateAppearance(uid, component, null);
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x00030584 File Offset: 0x0002E784
		private void UpdateAppearance(EntityUid uid, RandomSpriteComponent component, [Nullable(2)] SpriteComponent sprite = null)
		{
			if (!base.Resolve<SpriteComponent>(uid, ref sprite, false))
			{
				return;
			}
			foreach (KeyValuePair<string, ValueTuple<string, Color?>> keyValuePair in component.Selected)
			{
				Enum @enum;
				int num;
				if (this._reflection.TryParseEnumReference(keyValuePair.Key, ref @enum, true))
				{
					if (!sprite.LayerMapTryGet(@enum, ref num, true))
					{
						break;
					}
				}
				else if (!sprite.LayerMapTryGet(keyValuePair.Key, ref num, false))
				{
					string key = keyValuePair.Key;
					if (key == null || !int.TryParse(key, out num))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Invalid key `");
						defaultInterpolatedStringHandler.AppendFormatted(keyValuePair.Key);
						defaultInterpolatedStringHandler.AppendLiteral("` for entity with random sprite ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
						Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
						break;
					}
				}
				sprite.LayerSetState(num, keyValuePair.Value.Item1);
				sprite.LayerSetColor(num, keyValuePair.Value.Item2 ?? Color.White);
			}
		}

		// Token: 0x04000432 RID: 1074
		[Dependency]
		private readonly IReflectionManager _reflection;
	}
}
