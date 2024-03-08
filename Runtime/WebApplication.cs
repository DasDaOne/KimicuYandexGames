﻿using System;

namespace Kimicu.YandexGames
{
    public static class WebApplication
    {
        private static bool _inBackground;
        private static bool _inAdvert;
        private static bool _inPurchaseWindow;
        private static bool _customValue;

        public static bool InBackground
        {
            get => _inBackground;
            internal set
            {
                _inBackground = value;
                TryStopGame();
            }
        }

        public static bool InAdvert
        {
            get => _inAdvert;
            set
            {
                _inAdvert = value;
                TryStopGame();
            }
        }

        public static bool InPurchaseWindow
        {
            get => _inPurchaseWindow;
            set
            {
                _inPurchaseWindow = value;
                TryStopGame();
            }
        }

        public static bool CustomValue
        {
            get => _customValue;
            set
            {
                _customValue = value;
                TryStopGame();
            }
        }

        private static event Action<bool> OnStopGame;

        public static void Initialize(Action<bool> onStopGame)
        {
            OnStopGame = onStopGame;
            InBackground = Agava.WebUtility.WebApplication.InBackground;
            Agava.WebUtility.WebApplication.InBackgroundChangeEvent += InBackgroundChange;
        }

        private static void InBackgroundChange(bool inBackground) => InBackground = inBackground;
        private static void TryStopGame() => OnStopGame?.Invoke(InBackground || InAdvert || InPurchaseWindow || CustomValue);
    }
}