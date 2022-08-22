using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField]
    private string URL;

    public static int score = 0;
    public static int highScore = 0;
    private string Token;
    public Text[] NameText;
    public Text[] ScoreText;

    public GameObject ScorePanel;
    private string Username;

    void Start()
    {
        score = 0;
        highScore = PlayerPrefs.GetInt("High Score", highScore);
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");
        Debug.Log("TOKEN:" + Token);
    }
    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }
    public void ExitPipe()
    {
        ScorePanel.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = score.ToString();
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("High Score", highScore);
        }
    }

    public void SumbitPoints()
    {
        Token = PlayerPrefs.GetString("token");
        UserData data = new UserData();
        data.username = PlayerPrefs.GetString("username");
        data.score = PlayerPrefs.GetInt("High Score");
        string postData = JsonUtility.ToJson(data);
        StartCoroutine(SetScore(postData));
    }

    IEnumerator SetScore(string postData)
    {
        Debug.Log("PATCH SCORE ");
        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);

        www.method = "PATCH";

        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }
    IEnumerator GetScores()
    {
        string url = URL + "/api/usuarios" + "?limit=5&sort=true";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.method = "GET";

        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            Debug.Log(www.downloadHandler.text);
            Scores resData = JsonUtility.FromJson<Scores>(www.downloadHandler.text);
            ScorePanel.SetActive(true);
            for (int i = 0; i < resData.usuarios.Length; i++)
            {
                Debug.Log(resData.usuarios[i].username + "," + resData.usuarios[i].score);
                ScoreText[i].text = resData.usuarios[i].score.ToString();
                NameText[i].text = resData.usuarios[i].username.ToString();
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }

}

[System.Serializable]
public class ScoreData
{
    public string user_id;
    public int score;
    public int highScore;
}

[System.Serializable]
public class Scores
{
    public UserData[] usuarios;
}