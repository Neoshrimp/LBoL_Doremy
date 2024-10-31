using LBoL.ConfigData;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoL_Doremy.RootTemplates
{
    public abstract class DUltimateSkillDef : UltimateSkillTemplate
    {
        public override IdContainer GetId() => this.SelfId();

        public override LocalizationOption LoadLocalization() => Loc.UltimateSkillsBatchLoc.AddEntity(this);

        public override Sprite LoadSprite() => ResourceLoader.LoadSprite(GetId() + ".png", Sources.exAndBomb);

    }
}
