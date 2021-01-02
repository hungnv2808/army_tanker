using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingIcon : MonoBehaviour
{
    [SerializeField] private Transform m_smallGear;
    [SerializeField] private Transform m_bigGear;
    [SerializeField] private Text m_loadingLabel;
    private int m_count;

    public void Init() {
        m_count = 0;
        m_loadingLabel.text = "Loading";
        StartCoroutine(AnimationLoadingLabel());
    }

    private void Update()
    {
        m_bigGear.localEulerAngles = new Vector3(0, 0, m_bigGear.localEulerAngles.z -  Time.deltaTime * 100);
        m_smallGear.localEulerAngles = new Vector3(0, 0, m_smallGear.localEulerAngles.z - Time.deltaTime * 200);
    }
    private IEnumerator AnimationLoadingLabel() {
        while(true) {
            m_count += 1;
            if (m_count > 3) m_count = 1;
            yield return new WaitForSeconds(1.0f);
            switch (m_count) {
                case 1:
                    m_loadingLabel.text = "Loading.";
                    break;
                case 2:
                    m_loadingLabel.text = "Loading..";
                    break;
                case 3:
                    m_loadingLabel.text = "Loading...";
                    break;
            }
            
        }
    }
}
