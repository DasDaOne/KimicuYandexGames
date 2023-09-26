﻿using System;
using UnityEngine;

namespace KiYandexSDK
{
    public static class WebGL
    {
        private static event Action<bool> OnBackgroundChanged;
        
        /// <summary>
        /// Инициализация WebGL
        /// </summary>
        /// <param name="onBackgroundChanged"></param>
        public static void Initialize(Action<bool> onBackgroundChanged = null)
        {
            Agava.WebUtility.WebApplication.InBackgroundChangeEvent += InBackgroundChange;
            OnBackgroundChanged = onBackgroundChanged;
        }

        private static void InBackgroundChange(bool value)
        {
            WebProperty.InGameView = !value;
            OnBackgroundChanged?.Invoke(value);
            if (!WebProperty.AdvertOpened && WebProperty.InGameView)
            {
                AudioListener.volume = 1;
            }
            else
            {
                AudioListener.volume = 0;
            }
        }
    }
}