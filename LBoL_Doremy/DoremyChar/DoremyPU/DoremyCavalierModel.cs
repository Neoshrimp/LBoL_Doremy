using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoL.Presentation;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.DoremyChar.DoremyPU
{
    public sealed class DoremyCavalierModel : UnitModelTemplate
    {
        public override IdContainer GetId() => nameof(DoremyCavalier);

        public override LocalizationOption LoadLocalization() => Loc.UnitModelBatchLoc.AddEntity(this);

        public override ModelOption LoadModelOptions()
        {
            return new ModelOption(ResourcesHelper.LoadSpineUnitAsync(DoremyCavalierDef.Name));
        }

        public override UniTask<Sprite> LoadSpellSprite()
        {
            return ResourceLoader.LoadSpriteAsync("DoremyStand.png", Sources.playerImgsSource, ppu: 1000);
        }

        public override UnitModelConfig MakeConfig()
        {
            return UnitModelConfig.FromName(DoremyCavalierDef.Name).Copy();
        }
    }
}
