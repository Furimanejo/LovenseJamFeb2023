using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    [SerializeField] GameObject menuCanvas;
    public TMP_InputField sessionName;
    public TMP_Dropdown regionDropdown;
    [SerializeField] UnityEngine.Events.UnityEvent onEnterGame;
    [SerializeField] UnityEngine.Events.UnityEvent onLeaverGame;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            enabled = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (menuCanvas.activeSelf)
        {
            menuCanvas.SetActive(false);
        }
        else
        {
            menuCanvas.SetActive(true);
        }
    }

    public static void TryStartGame()
    {
        NetworkManager.instance.CreateOrJoinSession();
    }

    public static void OnEnterGame()
    {
        instance.onEnterGame.Invoke();
    }

    public static void LeaveSession()
    {
        Destroy(NetworkManager.instance.gameObject);
        instance.onLeaverGame.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}