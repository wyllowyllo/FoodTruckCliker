using Core;
using OutGame.UserData.Domain;
using OutGame.UserData.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScene : MonoBehaviour
{
    // 로그인씬 (로그인/회원가입) -> 게임씬

    private enum SceneMode
    {
        Login,
        Register
    }
    
    private SceneMode _mode = SceneMode.Login;
    
    // 비밀번호 확인 오브젝트
    [SerializeField] private GameObject _passwordCofirmObject;
    [SerializeField] private Button _gotoRegisterButton;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _gotoLoginButton;
    [SerializeField] private Button _registerButton;

    [SerializeField] private TextMeshProUGUI _messageTextUI;
    
    [SerializeField] private TMP_InputField _idInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _passwordConfirmInputField;
    
    private void Start()
    {
        AddButtonEvents();
        Refresh();
    }

    public void OnEmailTextChanged(string _)
    {
        var emailSpec = new AccountEmailSpecification();
        bool isValid = emailSpec.IsSatisfiedBy(_idInputField.text);

        _messageTextUI.text = isValid ? "완벽한 이메일입니다." : emailSpec.ErrorMessage;
        _loginButton.interactable = isValid;
        _registerButton.interactable = isValid;
    }
    private void AddButtonEvents()
    {
        _gotoRegisterButton.onClick.AddListener(GotoRegister);
        _loginButton.onClick.AddListener(Login);
        _gotoLoginButton.onClick.AddListener(GotoLogin);
        _registerButton.onClick.AddListener(Register);
    }

    private void Refresh()
    {
        // 2차 비밀번호 오브젝트는 회원가입 모드일때만 노출
        _passwordCofirmObject.SetActive(_mode == SceneMode.Register);
        _gotoRegisterButton.gameObject.SetActive(_mode == SceneMode.Login);
        _loginButton.gameObject.SetActive(_mode == SceneMode.Login);
        _gotoLoginButton.gameObject.SetActive(_mode == SceneMode.Register);
        _registerButton.gameObject.SetActive(_mode == SceneMode.Register);

        // 현재 이메일 유효성에 따라 버튼 활성화 동기화
        var emailSpec = new AccountEmailSpecification();
        bool isValid = emailSpec.IsSatisfiedBy(_idInputField.text);
        _loginButton.interactable = isValid;
        _registerButton.interactable = isValid;
    }

    private void Login()
    {
        // 로그인
        string email = _idInputField.text;
        string password = _passwordInputField.text;
        
        var result = AccountManager.Instance.TryLogin(email, password);
        if (result.Success)
        {
            SceneLoader.LoadScene(SceneLoader.GameScene);
        }
        else
        {
            _messageTextUI.text = result.ErrorMessage;
        }
    }

    private void Register()
    {
        string email = _idInputField.text;
        string password = _passwordInputField.text;
        string password2 = _passwordConfirmInputField.text;
        
        if (string.IsNullOrEmpty(password2) || password != password2)
        {
            _messageTextUI.text = "패스워드를 확인해주세요.";
            return;
        }

        var result = AccountManager.Instance.TryRegister(email, password);
        if (result.Success)
        {
            _messageTextUI.text = "회원가입에 성공했습니다. 로그인해주세요.";
            GotoLogin();
        }
        else
        {
            _messageTextUI.text = result.ErrorMessage;
        }

    }

    private void GotoLogin()
    {
        _mode = SceneMode.Login;
        Refresh();
    }

    private void GotoRegister()
    {
        _mode = SceneMode.Register;
        Refresh();
    }
    
}