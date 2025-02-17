﻿using System;
using System.Collections;
using UnityEngine;
using Coroutine = Kimicu.YandexGames.Utils.Coroutine;

namespace Kimicu.YandexGames
{
    public static partial class Advertisement
    {
        public static bool AdvertisementIsAvailable { get; private set; } = true;
        public static bool Initialized { get; private set; } = false;
        private static readonly Coroutine ReloadCoroutine = new Coroutine();

        private const float INTERSTITIAL_AD_COOLDOWN = 70;
        
        private static bool s_isInVideoAd;

        public static void Initialize()
        {
            ReloadCoroutine.StartRoutine(AdvertisementReloadRoutine());
            Initialized = true;
        }

        public static void ShowInterstitialAd(Action onOpenCallback = null, Action onCloseCallback = null, Action<string> onErrorCallback = null, Action onOfflineCallback = null)
        {
            if (!Initialized) throw new Exception($"{nameof(Advertisement)} not initialized!");

            if (!AdvertisementIsAvailable)
            {
                onErrorCallback?.Invoke("Advertising not available yet!");
                return;
            }

            AdvertisementIsAvailable = false;
            
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.InterstitialAd.Show(() =>
                {
                    onOpenCallback?.Invoke();
                },
                (_) =>
                {
                    onCloseCallback?.Invoke();
                    AdvertisementIsAvailable = false;
                },
                (error) =>
                {
                    onErrorCallback?.Invoke(error);
                    AdvertisementIsAvailable = false;
                },
                () =>
                {
                    onOfflineCallback?.Invoke();
                    AdvertisementIsAvailable = false;
                });
            #elif UNITY_EDITOR
            AdvertisementIsAvailable = false;
            onOpenCallback?.Invoke();
            onCloseCallback?.Invoke();
            #endif
        }

        public static void ShowVideoAd(Action onOpenCallback = null, Action onRewardedCallback = null, Action onCloseCallback = null, Action<string> onErrorCallback = null)
        {
            if (!Initialized) throw new Exception($"{nameof(Advertisement)} not initialized!");

            if (s_isInVideoAd)
            {
                onErrorCallback?.Invoke("Video Ad has already been called!");
                return;
            }

            s_isInVideoAd = true;
            
            #if !UNITY_EDITOR && UNITY_WEBGL
            Agava.YandexGames.VideoAd.Show(() =>
                {
                    onOpenCallback?.Invoke();
                }, ()
                    => onRewardedCallback?.Invoke(),
                () =>
                {
                    onCloseCallback?.Invoke();
                    s_isInVideoAd = false;
                }, (error) =>
                {
                    onErrorCallback?.Invoke(error);
                    s_isInVideoAd = false;
                });
            #elif UNITY_EDITOR
            onOpenCallback?.Invoke();
            onRewardedCallback?.Invoke();
            onCloseCallback?.Invoke();
            s_isInVideoAd = false;
            #endif
        }

        public static void StickySetActive(bool value)
        {
            if (!Initialized) throw new Exception($"{nameof(Advertisement)} not initialized!");

            #if !UNITY_EDITOR && UNITY_WEBGL
            if (value) Agava.YandexGames.StickyAd.Show();
            else Agava.YandexGames.StickyAd.Hide();
            #endif
        }

        private static IEnumerator AdvertisementReloadRoutine()
        {
            while (true)
            {
                if (AdvertisementIsAvailable)
                {
                    yield return null;
                    continue;
                }
                yield return new WaitForSecondsRealtime(INTERSTITIAL_AD_COOLDOWN);
                AdvertisementIsAvailable = true;
            }
        }
    }
}
