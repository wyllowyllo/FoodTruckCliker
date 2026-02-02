using System;
using OutGame.UserData.Domain;
using OutGame.UserData.Repository;
using UnityEngine;

namespace OutGame.UserData.Manager
{
    // 매니저의 역할 : 
    // 1. 도메인 관리 : 생성/조회/수정/삭제와 같은 비즈니스 로직
    // 2. 외부와의 소통 창구
    public class AccountManager : MonoBehaviour
    {
        public static AccountManager Instance { get; private set; }

        private Account _currentAccount = null;
        
        public bool IsLogin => _currentAccount != null;
        public string Email => _currentAccount.Email ?? string.Empty;

        private IAccountRepository _repository;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _repository = new LocalAccountRepository();
        }

        public AuthResult TryLogin(string email, string password)
        {
            try
            {
                Account account = new Account(email, password);
            }
            catch(Exception ex)
            {
                // 1. 유효성 검증 통과 못하면 실패
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                };
            }
        
            // 2. 레포지토리를 이용한 로그임
            AuthResult result = _repository.Login(email, password);
            if (result.Success)
            {
                _currentAccount = result.Account;
                return new AuthResult
                {
                    Success = true,
                    Account = result.Account,
                };
            }
            else
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = result.ErrorMessage,
                };
            }
        }

        public AuthResult TryRegister(string email, string password)
        {
            try
            {
                Account account = new Account(email, password);
            }
            catch(Exception ex)
            {
                // 2. 유효성 검증 통과 못하면 실패
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                };
            }
        
          
            AuthResult result = _repository.Register(email, password);
            if (result.Success)
            {
                return new AuthResult
                {
                    Success = true,
                };
            }
            else
            {
                return new AuthResult()
                {
                    Success = false,
                    ErrorMessage = result.ErrorMessage,
                };
            }
        }

        public void Logout()
        {
            _repository.Logout();
        }
        
        
    }
}