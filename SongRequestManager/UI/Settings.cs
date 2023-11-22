//using CustomUI.MenuButton;
//using CustomUI.Settings;
//using CustomUI.Utilities;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using SongRequestManager;
//namespace SongRequestManager

//{
//    public class Settings
//    {
//        private static float[] incrementValues(float startValue = 0.0f, float step = 0.1f, int numberOfElements = 11)
//        {
//            if (step < 0.01f)
//            {
//                throw new Exception("Step value specified was too small! Minimum supported step value is 0.01");
//            }
//            Int64 multiplier = 100;
//            // Avoid floating point math as it results in rounding errors
//            Int64 fixedStart = (Int64)(startValue * multiplier);
//            Int64 fixedStep = (Int64)(step * multiplier);
//            var values = new float[numberOfElements];
//            for (int i = 0; i < values.Length; i++)
//                values[i] = (float)(fixedStart + (fixedStep * i)) / multiplier;
//            return values;
//        }

//        public static void OnLoad()
//        {
//            var menu = SettingsUI.CreateSubMenu("Song Request Manager");

//            var AutopickFirstSong = menu.AddBool("Autopick First Song", "Automatically pick the first song with sr!");
//            AutopickFirstSong.SetValue += (requests) => { RequestQueueConfigManager.Instance.Config.AutopickFirstSong = requests; };
//            AutopickFirstSong.GetValue += () => { return RequestQueueConfigManager.Instance.Config.AutopickFirstSong; };

//            var MiniumSongRating = menu.AddSlider("Minimum rating", "Minimum allowed song rating", 0, 100, 0.5f, false);
//            MiniumSongRating.SetValue += (scale) => { RequestQueueConfigManager.Instance.Config.LowestAllowedRating = scale; };
//            MiniumSongRating.GetValue += () => { return RequestQueueConfigManager.Instance.Config.LowestAllowedRating; };

//            var MaximumAllowedSongLength = menu.AddSlider("Maximum Song Length", "Longest allowed song length in minutes", 0, 999, 1.0f, false);
//            MaximumAllowedSongLength.SetValue += (scale) => { RequestQueueConfigManager.Instance.Config.MaximumSongLength =  scale; };
//            MaximumAllowedSongLength.GetValue += () => { return RequestQueueConfigManager.Instance.Config.MaximumSongLength; };

//            var MinimumNJS = menu.AddSlider("Minimum NJS allowed", "Disallow songs below a certain NJS", 0, 50, 1.0f, false);
//            MinimumNJS.SetValue += (scale) => { RequestQueueConfigManager.Instance.Config.MinimumNJS= scale; };
//            MinimumNJS.GetValue += () => { return RequestQueueConfigManager.Instance.Config.MinimumNJS; };

//            var TTSSupport = menu.AddBool("TTS Support", "Add ! to all command outputs for TTS Filtering");
//            TTSSupport.SetValue += (requests) => { RequestQueueConfigManager.Instance.Config.BotPrefix = requests ? "! " : ""; };
//            TTSSupport.GetValue += () => { return RequestQueueConfigManager.Instance.Config.BotPrefix!=""; };

//            var UserRequestLimit = menu.AddSlider("User Request limit", "Maximum requests in queue at one time", 0, 10, 1f, true);
//            UserRequestLimit.SetValue += (scale) => { RequestQueueConfigManager.Instance.Config.UserRequestLimit= (int ) scale; };
//            UserRequestLimit.GetValue += () => { return RequestQueueConfigManager.Instance.Config.UserRequestLimit; };

//            var SubRequestLimit = menu.AddSlider("Sub Request limit", "Maximum requests in queue at one time", 0, 10, 1f, true);
//            SubRequestLimit.SetValue += (scale) => { RequestQueueConfigManager.Instance.Config.SubRequestLimit = (int)scale; };
//            SubRequestLimit.GetValue += () => { return RequestQueueConfigManager.Instance.Config.SubRequestLimit; };

//            var ModRequestLimit = menu.AddSlider("Moderator Request limit", "Maximum requests in queue at one time", 0, 100, 1f, true);
//            ModRequestLimit.SetValue += (scale) => { RequestQueueConfigManager.Instance.Config.ModRequestLimit = (int)scale; };
//            ModRequestLimit.GetValue += () => { return RequestQueueConfigManager.Instance.Config.ModRequestLimit; };

//            var VIPBonus = menu.AddSlider("VIP Request bonus", "Additional requests allowed in queue", 0, 10, 1f, true);
//            VIPBonus.SetValue += (scale) => { RequestQueueConfigManager.Instance.Config.VipBonusRequests = (int)scale; };
//            VIPBonus.GetValue += () => { return RequestQueueConfigManager.Instance.Config.VipBonusRequests; };

//            var ModeratorRights = menu.AddBool("Full moderator rights", "Allow moderators access to ALL bot commands. Do you trust your mods?");
//            ModeratorRights.SetValue += (requests) => { RequestQueueConfigManager.Instance.Config.ModFullRights = requests ; };
//            ModeratorRights.GetValue += () => { return RequestQueueConfigManager.Instance.Config.ModFullRights; };

//        }
//    }
//}