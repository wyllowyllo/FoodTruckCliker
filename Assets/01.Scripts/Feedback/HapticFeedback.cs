using System.Runtime.InteropServices;
using UnityEngine;

namespace Feedback
{
    /// <summary>
    /// 햅틱 피드백 유틸리티 - Android/iOS 지원
    /// </summary>
    public static class HapticFeedback
    {
        private static bool _initialized;

#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject _vibrator;
#endif

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void _TriggerImpactHaptic(int style);
#endif

        /// <summary>
        /// 가벼운 진동 (일반 클릭)
        /// </summary>
        public static void LightImpact()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            VibrateAndroid(20);
#elif UNITY_IOS && !UNITY_EDITOR
            _TriggerImpactHaptic(0); // Light
#endif
        }

        /// <summary>
        /// 강한 진동 (크리티컬)
        /// </summary>
        public static void HeavyImpact()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            VibrateAndroid(50);
#elif UNITY_IOS && !UNITY_EDITOR
            _TriggerImpactHaptic(2); // Heavy
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static void VibrateAndroid(long milliseconds)
        {
            if (!_initialized)
            {
                InitializeAndroid();
            }

            _vibrator?.Call("vibrate", milliseconds);
        }

        private static void InitializeAndroid()
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    _vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                }
                _initialized = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[HapticFeedback] Android 초기화 실패: {e.Message}");
            }
        }
#endif
    }
}
