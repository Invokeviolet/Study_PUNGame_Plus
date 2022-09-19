using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HpBarInfo : MonoBehaviour
{
    //constant value
    [SerializeField] TextMeshProUGUI NickName = null;
    [SerializeField] Slider HpBar = null;

    public void SetName(string name)
    {
        NickName.text = name;
    }

    public void ChangedHp(float curHp, float maxHp)
    {
        HpBar.value = curHp / maxHp;
    }

    private void Update()
    {
        this.transform.rotation = Quaternion.LookRotation(this.transform.position - Camera.main.transform.position );
    }
}
