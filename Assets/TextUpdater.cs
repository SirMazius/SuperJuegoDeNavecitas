using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextUpdater : MonoBehaviour
{
    public TMPro.TextMeshProUGUI testo;
    public int score;
    public static TextUpdater textUpdaterInstance;
    // Start is called before the first frame update
    void Start()
    {
        if (textUpdaterInstance == null)
        {
            textUpdaterInstance = this;
            score = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetScore(int _score)
    {
        score += _score;
        testo.SetText(score.ToString());
    }

    
}
