using Btkalman.Unity.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScreen : MonoBehaviour {
    public static FinishScreen i;

    [SerializeField] private GameObject m_winText;
    [SerializeField] private GameObject m_loseText;

    private Canvas m_canvas;

    public bool Visible() {
        return m_canvas.enabled;
    }

    public void Hide() {
        m_canvas.enabled = false;
    }

    public void Win() {
        m_canvas.enabled = true;
        m_winText.SetActive(true);
        m_loseText.SetActive(false);
    }

    public void Lose() {
        m_canvas.enabled = true;
        m_winText.SetActive(false);
        m_loseText.SetActive(true);
    }

    private void Awake() {
        Singleton.Awake(this, ref i);
        m_canvas = GetComponent<Canvas>();
    }

    private void Start() {
        m_canvas.enabled = false;
    }
}
