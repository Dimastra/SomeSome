using System;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Item;
using Content.Shared.Xenoarchaeology.XenoArtifacts;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Xenoarchaeology.XenoArtifacts
{
	// Token: 0x02000020 RID: 32
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomArtifactSpriteSystem : EntitySystem
	{
		// Token: 0x0600007B RID: 123 RVA: 0x00004A9C File Offset: 0x00002C9C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RandomArtifactSpriteComponent, MapInitEvent>(new ComponentEventHandler<RandomArtifactSpriteComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<RandomArtifactSpriteComponent, ArtifactActivatedEvent>(new ComponentEventHandler<RandomArtifactSpriteComponent, ArtifactActivatedEvent>(this.OnActivated), null, null);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004ACC File Offset: 0x00002CCC
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<RandomArtifactSpriteComponent, AppearanceComponent> valueTuple in this.EntityManager.EntityQuery<RandomArtifactSpriteComponent, AppearanceComponent>(false))
			{
				RandomArtifactSpriteComponent component = valueTuple.Item1;
				AppearanceComponent appearance = valueTuple.Item2;
				if (component.ActivationStart != null && (double)(this._time.CurTime - component.ActivationStart.Value).Seconds >= component.ActivationTime)
				{
					this._appearance.SetData(appearance.Owner, SharedArtifactsVisuals.IsActivated, false, appearance);
					component.ActivationStart = null;
				}
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004B90 File Offset: 0x00002D90
		private void OnMapInit(EntityUid uid, RandomArtifactSpriteComponent component, MapInitEvent args)
		{
			int randomSprite = this._random.Next(component.MinSprite, component.MaxSprite + 1);
			this._appearance.SetData(uid, SharedArtifactsVisuals.SpriteIndex, randomSprite, null);
			this._item.SetHeldPrefix(uid, "ano" + randomSprite.ToString("D2"), null);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004BF3 File Offset: 0x00002DF3
		private void OnActivated(EntityUid uid, RandomArtifactSpriteComponent component, ArtifactActivatedEvent args)
		{
			this._appearance.SetData(uid, SharedArtifactsVisuals.IsActivated, true, null);
			component.ActivationStart = new TimeSpan?(this._time.CurTime);
		}

		// Token: 0x0400005E RID: 94
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400005F RID: 95
		[Dependency]
		private readonly IGameTiming _time;

		// Token: 0x04000060 RID: 96
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x04000061 RID: 97
		[Dependency]
		private readonly SharedItemSystem _item;
	}
}
