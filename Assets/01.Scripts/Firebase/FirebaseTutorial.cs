using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Unity.VisualScripting;

public class FirebaseTutorial : MonoBehaviour
{
   
   private FirebaseApp _app   = null;
   private FirebaseAuth _auth = null;
   private FirebaseFirestore _db = null;
   
   private async UniTaskVoid Start()
   {
      // 과제.
      // 이 씬이 시작되면
      // 1. 파이베이스 초기화
      await InitFirebase();
      
      // 2. 로그아웃
      await Logout();
      // 3. 재로그인
      await Login("mingwan51910@gmail.com", "123456");
      // 4. 강아지 추가
      await SaveDog();
   }

   private async UniTask InitFirebase()
   {
      // 콜백 함수 : 특정 이벤트가 발생하고 나면 자동으로 호출되는 함수
      // 접속에 1MS ~~~ 
      
      // Firebase Task → UniTask로 변환
      var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        
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

   private async UniTask Register(string email, string password)
   {
      // .ContinueWithOnMainThread 대신 await 사용!
      var result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
        
      Debug.LogFormat("✅ 회원가입 성공: {0} ({1})", 
         result.User.DisplayName, result.User.UserId);
   }

   private async UniTask Login(string email, string password)
   {
      var result = await _auth.SignInWithEmailAndPasswordAsync(email, password);
        
      Debug.LogFormat("✅ 로그인 성공: {0} ({1})", 
         result.User.Email, result.User.UserId);
   }

   private UniTask Logout()
   {
      _auth.SignOut();
      Debug.Log("로그아웃 성공!");
      
      // 동기 함수는 UniTask.CompletedTask 반환
      return UniTask.CompletedTask;
   }

   private void CheckLoginStatus()
   {
      FirebaseUser user = _auth.CurrentUser;
      if (user == null)
      {
         Debug.Log("로그인 안됨");
      }
      else
      {            
         Debug.LogFormat("로그인 중: {0} ({1})", user.Email, user.UserId);
      }
      
   }

   private async UniTask SaveDog()
   {
      Dog dog = new Dog("소똥이", 1);

      // SetAsync도 await로 깔끔하게!
      await _db.Collection("Dogs")
         .Document("mingwan's dog")
         .SetAsync(dog);
        
      Debug.Log("✅ 강아지 저장 성공!");
   }

   private async UniTask LoadMyDog()
   {
      var snapshot = await _db.Collection("Dogs")
         .Document("mingwan's dog")
         .GetSnapshotAsync();
        
      if (snapshot.Exists)
      {
         Dog myDog = snapshot.ConvertTo<Dog>();
         Debug.Log($"🐕 {myDog.Name}({myDog.Age})");
      }
      else
      {
         Debug.LogError("❌ 데이터가 없습니다.");
      }
   }
   private async UniTask LoadDogs()
   {
      var snapshot = await _db.Collection("Dogs").GetSnapshotAsync();
        
      Debug.Log("🐕 강아지들-------------------------------------------");
      foreach (DocumentSnapshot doc in snapshot.Documents)
      {
         Dog myDog = doc.ConvertTo<Dog>();
         Debug.Log($"  - {myDog.Name}({myDog.Age})");
      }
   }

   private async UniTask DeleteDogs()
   {
      var snapshots = await _db.Collection("Dogs")
         .WhereEqualTo("Name", "소똥이")
         .GetSnapshotAsync();
        
      Debug.Log("🗑️ 소똥이들 삭제 중...");
        
      foreach (DocumentSnapshot snapshot in snapshots.Documents)
      {
         Dog myDog = snapshot.ConvertTo<Dog>();
         if (myDog.Name == "소똥이")
         {
            await _db.Collection("Dogs")
               .Document(snapshot.Id)
               .DeleteAsync();
                
            Debug.Log($"✅ {myDog.Name} 삭제 완료!");
         }
      }
   }

   
   private void Update()
   {
      if (_app == null) return;

      if (Input.GetKeyDown(KeyCode.Alpha1))
      {
         Register("mingwan51910@gmail.com", "123456");
      }
      if (Input.GetKeyDown(KeyCode.Alpha2))
      {
         Login("mingwan51910@gmail.com", "123456");
      }
      if (Input.GetKeyDown(KeyCode.Alpha3))
      {
         Logout();
      }
        
      if (Input.GetKeyDown(KeyCode.Alpha4))
      {
         CheckLoginStatus();
      }
      if (Input.GetKeyDown(KeyCode.Alpha5))
      {
         SaveDog();
      }
      if (Input.GetKeyDown(KeyCode.Alpha6))
      {
         LoadMyDog();
      }
      if (Input.GetKeyDown(KeyCode.Alpha7))
      {
         LoadDogs();
      }
   } 
   
   
}