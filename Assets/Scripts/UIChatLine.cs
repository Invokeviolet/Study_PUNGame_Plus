using TMPro;
using UnityEngine;
using TMPro;

public class UIChatLine : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtNickName;
    [SerializeField] TextMeshProUGUI txtChat;


    void Start()
    {
        
    }

    
    public void SetChat(string name,string chat)
    {
        txtNickName.text = name;
        txtChat.text = chat;
    }
}
