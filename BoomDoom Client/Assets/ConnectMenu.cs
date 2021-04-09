using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectMenu : Singleton<ConnectMenu>
{
    public GameObject menu;
    [SerializeField] TMP_InputField IPInput;
    [SerializeField] TMP_InputField portInput;
    [SerializeField] Button connectButton;

    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        Client.Instance.OnConnection += () => { Connect(); };
    }
    public void AttemptConnection()
    {
        try
        {
            Client.Instance.Connect(IPInput.text, int.Parse(portInput.text));
        }
        catch
        {
            IPInput.text = null;
            portInput.text = null;
            return;
        }
        IPInput.interactable = false;
        portInput.interactable = false;
        connectButton.interactable = false;
    }
    public void Connect()
    {
        Destroy(menu);
    }
}
