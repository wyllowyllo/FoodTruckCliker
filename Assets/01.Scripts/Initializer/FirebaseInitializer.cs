using System;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

namespace Initializer
{
    public class FirebaseInitializer : MonoBehaviour
    {
        private FirebaseApp _app   = null;
        private FirebaseAuth _auth = null;
        private FirebaseFirestore _db = null;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        private void Start()
        {
            // Fire - and - Forget 패턴
            // 비동기 작업을 시작만 하고, 결과를 기다리지는 않겠다. 
            InitFirebase().Forget(); // 아래쪽에 실행할 코드가 없기 때문에 await를 안해도 된다. 
        }
        
        private async UniTask InitFirebase()
        {
            // 콜백 함수 : 특정 이벤트가 발생하고 나면 자동으로 호출되는 함수
            // 접속에 1MS ~~~ 
      
            // Firebase Task → UniTask로 변환
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            try
            {
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _app = FirebaseApp.DefaultInstance;
                    _auth = FirebaseAuth.DefaultInstance;
                    _db = FirebaseFirestore.DefaultInstance;

                    Debug.Log("✅ Firebase 초기화 성공!");
                }
                else
                {
                    Debug.LogError($"❌ Firebase 초기화 실패: {dependencyStatus}");
                    throw new System.Exception("Firebase 초기화 실패");
                }
            }
            catch (FirebaseException e)
            {

            }
            catch (Exception e)
            {
                
            }
        }
    }
}