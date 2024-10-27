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
            DoremyAssets.CreatedIcon =  await ResourceLoader.LoadSpriteAsync("CreatedIcon.png", Sources.extraImgs);
            DoremyAssets.DreamLevel = await ResourceLoader.LoadSpriteAsync("DreamLevel.png", Sources.extraImgs);



        }


    }

    public class DoremyAssets
    {
        private Sprite createdIcon;
        private Sprite dreamLevel;

        public Sprite CreatedIcon { get => createdIcon; set => createdIcon = value; }
        public Sprite DreamLevel { get => dreamLevel; set => dreamLevel = value; }
    }
}
