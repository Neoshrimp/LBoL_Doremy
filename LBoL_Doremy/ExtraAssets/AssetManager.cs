using Cysharp.Threading.Tasks;
using HarmonyLib;
using LBoL.Presentation;
using LBoL_Doremy.StaticResources;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

namespace LBoL_Doremy.ExtraAssets
{
    public static class AssetManager
    {

        public static DoremyAssets DoremyAssets { get; private set; }
        public static void RegisterLoad()
        {
            EntityManager.AddPostLoadAction(DoLoadAsync);
        }

        private static async void DoLoadAsync()
        {
            DoremyAssets = new DoremyAssets();
            DoremyAssets.createdIcon =  await ResourceLoader.LoadSpriteAsync("CreatedIcon.png", Sources.extraImgs);
            DoremyAssets.dreamLevel = await ResourceLoader.LoadSpriteAsync("DreamLevel.png", Sources.extraImgs);

            DoremyAssets.purpleBar = await ResourceLoader.LoadSpriteAsync("PurpleBar.png", Sources.extraImgs);

        }


    }

    public class DoremyAssets
    {
        public Sprite createdIcon;
        public Sprite dreamLevel;
        public Sprite purpleBar;


    }
}
