using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

namespace OutGame.Upgrades.Repository
{
    public class FirebaseUpgradeRepository : IUpgradeRepository
    {
        private const string UPGRADE_COLLECTION_NAME = "Upgrade";

        private FirebaseAuth _auth = FirebaseAuth.DefaultInstance;
        private FirebaseFirestore _db = FirebaseFirestore.DefaultInstance;

        public async UniTaskVoid Save(UpgradeSaveData data)
        {
            try
            {
                string email = _auth.CurrentUser.Email;

                await _db.Collection(UPGRADE_COLLECTION_NAME).Document(email).SetAsync(data);
            }
            catch (Exception e)
            {
                Debug.LogError("Upgrade 저장 실패: " + e.Message);
            }
        }

        public async UniTask<UpgradeSaveData> Load()
        {
            try
            {
                string email = _auth.CurrentUser.Email;

                DocumentSnapshot snapshot = await _db.Collection(UPGRADE_COLLECTION_NAME).Document(email).GetSnapshotAsync();

                UpgradeSaveData data = snapshot.ConvertTo<UpgradeSaveData>();
                if (data.Levels != null)
                {
                    return data;
                }

                return UpgradeSaveData.Default;
            }
            catch (Exception e)
            {
                Debug.LogError("Upgrade 로드 실패: " + e.Message);
            }

            return UpgradeSaveData.Default;
        }
    }
}
