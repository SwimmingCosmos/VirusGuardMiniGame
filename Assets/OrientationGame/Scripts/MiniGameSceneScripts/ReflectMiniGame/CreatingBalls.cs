using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CreatingBalls : MonoBehaviour
{
    // キーとプレイヤー番号のマッピング、別のキーに書き換える場合はKeycodeのところ変更するだけでいけると思う
    private Dictionary<KeyCode, int> keyMappings = new Dictionary<KeyCode, int>
    {
        { KeyCode.D, 0 },
        { KeyCode.F, 1 },
        { KeyCode.G, 2 },
        { KeyCode.H, 3 },
        { KeyCode.J, 4 },
        { KeyCode.K, 5 }
    };
    
    // キーの押下開始時間を記録する辞書
    private Dictionary<KeyCode, float> keyPressStartTime = new Dictionary<KeyCode, float>();
    
    //プレイヤーネームのリスト、すごろくから持ってくる
    private List<string> playerNameList = new List<string>();
    public List<string> PlayerNameList
    {
        get { return playerNameList; }
        set { playerNameList = value; }
    }
    
    
    //各プレイヤーの色(color型)、すごろくから持ってくる
    private List<Color> playerColor = new List<Color>();
    private List<Color> PlayerColor
    {
        get { return playerColor; }
        set { playerColor = value; }
    }
    
    //各プレイヤーの順位(int型)
    private List<int> playerRanking = new List<int>();
    private List<int> PlayerRanking
    {
        get { return playerRanking; }
        set { playerRanking = value; }
    }
    
    
    //残り時間;
    private float remainTime = 30.0f;

    //プレイヤーの壁オブジェクトのリスト
    [SerializeField] private List<GameObject> playerWallList = new List<GameObject>();
    
    //プレイヤーの壁のマテリアルのリスト(透明にする際に使います)
    private  List<Material> playerWallMaterialList = new List<Material>();

    // 時間をカウントする変数
    private float time;

    [Header("テキスト関連")]
    
    //プレイヤーネームのリストを表示するテキストのリスト
    [SerializeField] private List<Text> playerNameTextList = new List<Text>();

    //プレイヤーの残り時間を表示するテキストのリスト
    [SerializeField] private List<Text> playerRemainTimeTextList = new List<Text>();
    
    //ルール説明のテキスト
    [SerializeField] private GameObject ruleText;
    
    //残り時間を表示するテキスト
    [SerializeField] private Text remainTimeText;

    [SerializeField] private List<Slider> remainTimeBar = new List<Slider>();
    
    //ゲーム終了時のテキスト
    [SerializeField] private GameObject finishText;
    
    //プレイヤーの残り時間を示すリスト(数値)
    private List<float> playerRemainTimeList = new List<float>();
    
    //ランキング表示用(消しても良い)
    [SerializeField] private GameObject rankingPanel;
    
    //ゲーム説明用変数 
    [SerializeField] private List<GameObject> rules  = new List<GameObject>();
    private int currentRuleNumber = 0;
    
    //Endingに移行しているかどうかのフラグ
    bool isEnding = false;
    
    //ミニゲームが始まっているかどうかのフラグ
    bool isMiniGameStarted = false;

    [Header("その他")]
    //爆発オブジェクト、よりいいのがあったら変えたい
    [SerializeField]GameObject explosionPrefab;
    // ボールのプレハブ
    [SerializeField] private GameObject ballPrefab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        // 仮置きとしてプレイヤーネームリストにプレイヤー名を追加
        for (int i = 0; i < 6; i++)
        {
            playerRemainTimeList.Add(10.0f);
            
            //時間を表示
            //playerRemainTimeTextList[i].text = playerRemainTimeList[i].ToString("F1");
            remainTimeBar[i].value = playerRemainTimeList[i];
            
            //プレイヤーの壁のコライダーを無効にする（すり抜ける状態）
            playerWallList[i].GetComponent<BoxCollider>().enabled = false;

            //プレイヤーネームリストを設定する（デバック用のため消してください。）
            playerNameList.Add("Player"+i.ToString());
            
            //仮設定、本番は消す
            playerColor.Add(new Color(0.2f * i, 0.5f, 1.0f, 1.0f)); // 適当な色を設定
            //プレイヤーネームリストにプレイヤー名を表示
            playerNameTextList[i].text = playerNameList[i];
            
            // マテリアルの初期設定
            Material mat = playerWallList[i].GetComponent<Renderer>().material;
            mat.SetFloat("_Surface", 1); // Transparentモード
            mat.SetFloat("_Blend", 0);
            mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            mat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            mat.SetFloat("_ZWrite", 0); // 透過用にZWriteを無効化
            mat.renderQueue = (int)RenderQueue.Transparent;

            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_SURFACE_TYPE_OPAQUE");
            
            
            
            Color baseColor = mat.GetColor("_BaseColor");
            //////////////お
            //ここで色を設定できる　basecolor = 色　　a = 透明度
            //////////////
            baseColor = playerColor[i];
            baseColor.a = 0.4f;
            mat.SetColor("_BaseColor", baseColor);

            playerWallMaterialList.Add(mat);


        }

    }

    public void startGame()
    {
        if(rules.Count-1  == currentRuleNumber)
        {
            rules[currentRuleNumber].SetActive(false);
            isMiniGameStarted = true;
        }
        else
        {
            //次の説明に移る
            rules[currentRuleNumber].SetActive(false);
            currentRuleNumber++;
            rules[currentRuleNumber].SetActive(true);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        //ゲームが始まるときにボールを生成する
        if (isMiniGameStarted)
        {
            // 3秒ごとにボールを生成する
            time += Time.deltaTime;

            if (remainTime > 0 && isEnding == false)
            {
                if (time >= 3)
                {
                    CreateBall();
                    time = 0;
                }

                remainTime -= Time.deltaTime;
                remainTimeText.text = remainTime.ToString("F1");
            }
            else
            {
                remainTimeText.text = "0.0";
                EndGame();
            }
        }
        
       
    foreach (var pair in keyMappings)
    {
        KeyCode key = pair.Key;
        int keyNumber = pair.Value;

        // キーが押された瞬間の処理
        if (Input.GetKeyDown(key))
        {
            keyPressStartTime[key] = Time.time; // 押した時間を記録
        }

        // キーを押している間の処理
        if (Input.GetKey(key))
        {
            if (playerRemainTimeList[keyNumber] > 0)
            {
                bool isBursted = playerRanking.Contains(keyNumber + 1);

                // プレイヤーの壁を有効化
                playerWallList[keyNumber].GetComponent<BoxCollider>().enabled = true;

                // 通常の時間減少処理
                if (isMiniGameStarted) playerRemainTimeList[keyNumber] -= Time.deltaTime;
                if (!isBursted)remainTimeBar[keyNumber].value = playerRemainTimeList[keyNumber]; //playerRemainTimeTextList[keyNumber].text = playerRemainTimeList[keyNumber].ToString("F1");

                // 透明化解除（不透明にする）
                Color baseColor = playerWallMaterialList[keyNumber].GetColor("_BaseColor");
                baseColor.a = 1f;
                playerWallMaterialList[keyNumber].SetColor("_BaseColor", baseColor);
            }
            else
            {
                //playerRemainTimeTextList[keyNumber].text = "0.0";
                //playerRemainTimeTextList[keyNumber].color = new Color32(0x8b, 0x00, 0x00, 0xFF);
                remainTimeBar[keyNumber].value = 0;
                playerWallList[keyNumber].GetComponent<BoxCollider>().enabled = false;

                // 壁を薄くする
                Color baseColor = playerWallMaterialList[keyNumber].GetColor("_BaseColor");
                baseColor.a = 0.4f;
                playerWallMaterialList[keyNumber].SetColor("_BaseColor", baseColor);
            }
        }

        // キーを離した瞬間の処理
        if (Input.GetKeyUp(key))
        {
            if (keyPressStartTime.ContainsKey(key))
            {
                float pressDuration = Time.time - keyPressStartTime[key]; // 押していた時間を計算
                if (pressDuration < 0.1f) // 0.1秒未満ならペナルティ(連打対策)
                {
                    playerRemainTimeList[keyNumber] -= 0.1f; // 0.1秒減らす
                    remainTimeBar[keyNumber].value = playerRemainTimeList[keyNumber];
                    //playerRemainTimeTextList[keyNumber].text = playerRemainTimeList[keyNumber].ToString("F1");
                }
                keyPressStartTime.Remove(key); // 記録を削除
            }

            // 壁を無効化
            playerWallList[keyNumber].GetComponent<BoxCollider>().enabled = false;

            // 壁を薄くする
            Color baseColor = playerWallMaterialList[keyNumber].GetColor("_BaseColor");
            baseColor.a = 0.4f;
            playerWallMaterialList[keyNumber].SetColor("_BaseColor", baseColor);
        }
    }

        // (デバック用)スペースキーを押したらボールを生成する
        if(Input.GetKeyDown(KeyCode.Space))
        {
            CreateBall();
        }
    }
    
    //ボールを生成する処理
    private void CreateBall()
    {
        // ボールを生成
        Instantiate(ballPrefab, new Vector3(0, 0, 10), Quaternion.identity);
    }

    //サーバーに当たってしまった場合の処理(Ball.csから呼び出される
    public void ServerHit(int outServerNumber ,Vector3 ballPosition)
    {
        bool burstedServer = false;
        //サーバーの番号を受け取り、そのプレイヤーをアウトとする
        Instantiate(explosionPrefab, ballPosition, Quaternion.identity);
        foreach (var playerNumber in playerRanking)
        {
            if (playerNumber == outServerNumber + 1)
            {
                burstedServer = true;
            }
        }

        if (burstedServer == false)
        {
            //残り時間は無限に多くして、脱落した後も動かせるような仕様にしたい。
            playerRemainTimeList[outServerNumber] = 100;
            //ここで爆発する()
            playerRemainTimeTextList[outServerNumber].text = "Fail";
            playerRemainTimeTextList[outServerNumber].color = new Color32(0x8b,0x00,0x00,0xFF);
            playerRanking.Add(outServerNumber + 1);
            StartCoroutine(WaitRanking(outServerNumber, playerRanking.Count));
            if (playerRanking.Count == 5)
            {
                EndGame();
            }
        }
    }
    private IEnumerator WaitRanking(int outServerNumber, int rank)
    {
        yield return new WaitForSeconds(2f);
         //Debug.Log("Ranking:" + rank);
        //ランキングに反映
        if (rank == 5) playerRemainTimeTextList[outServerNumber].text = "2nd";
        else if (rank == 4) playerRemainTimeTextList[outServerNumber].text = "3rd";
        else playerRemainTimeTextList[outServerNumber].text = 7 - rank + "st";
    }

    //ゲーム終了時に呼び出し
    private void EndGame()
    {
        if (isEnding == false)
        {
            finishText.SetActive(true);
            isEnding = true;
            if (playerRanking.Count == 5)
            {
                
                //playerRankingの中にある1~6の数字の中で、１つ足りないものを加える
                for (int i = 1; i <= 6; i++)
                {
                    if (!playerRanking.Contains(i))
                    {
                        playerRanking.Add(i);
                    }
                }
            }
            
            //もし順位が確定していない場合は壁ごとの残り時間がどれだけ余っているかによって順位を決定する
            else if (playerRanking.Count <= 4)
            {
                while (playerRanking.Count < 6)
                {
                    //playerRankingの中にある1~6の数字の中で、一番残り時間が少ないものを加える
                    
                    //初期化変数
                    float tempRemainTime = 1000f;
                    int a = 0;
                    for (int i = 1; i <= 6; i++)
                    {
                        if (!playerRanking.Contains(i))
                        {
                            if (playerRemainTimeList[i - 1] < tempRemainTime)
                            {
                                tempRemainTime = playerRemainTimeList[i - 1];
                                a = i;
                            }
                        }
                    }
                    Debug.Log("a:" + a);
                    playerRanking.Add(a);
                }
            }
            
            
            //ランキングの入れ替え
            (playerRanking[0], playerRanking[5]) = (playerRanking[5], playerRanking[0]); //ライキングの入れ替え、このままだと負け打順になっている
            (playerRanking[1], playerRanking[4]) = (playerRanking[4], playerRanking[1]);
            (playerRanking[2], playerRanking[3]) = (playerRanking[3], playerRanking[2]);
            
            //ランキングの表示
            for (int i = 0; i < 6; i++)
            {
                Debug.Log("rank:" + playerRanking[i]);
            }
            
            //マテリアルの設定を元に戻す
            for (int i = 0; i < 6; i++)
            {
                Color baseColor = playerWallMaterialList[i].GetColor("_BaseColor");
                baseColor.a = 0.4f;
                playerWallMaterialList[i].SetColor("_BaseColor", baseColor);
                playerWallMaterialList[i].SetFloat("_ZWrite", 1); // ZWriteを無効化（透過用）
                playerWallMaterialList[i].SetFloat("_Surface", 0); // 0 = Opaque, 1 = Transparent
                playerWallMaterialList[i].EnableKeyword("_SURFACE_TYPE_OPAQUE");
                playerWallMaterialList[i].DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                // playerWallMaterialList[i].SetFloat("_Blend", 0); // 0 = Alpha, 1 = Premultiply, 2 = Additive, 3 = Multiply
            }
        }
        //ランキングの表示、子供のテキストを取得
        rankingPanel.SetActive(true);
        StartCoroutine(MiniGameEnd());
    }
    IEnumerator MiniGameEnd()
    {
        yield return new WaitForSeconds(2.5f);
        GameObject rankingText = rankingPanel.transform.GetChild(1).gameObject;
        rankingText.GetComponent<Text>().text = "1st:" + 
                                                playerNameList[playerRanking[0] - 1] + "\n" + "2nd:" +
                                                playerNameList[playerRanking[1] - 1] + "\n" + "3rd:" +
                                                playerNameList[playerRanking[2] - 1] + "\n" + "4th:" +
                                                playerNameList[playerRanking[3] - 1] + "\n" + "5th:" +
                                                playerNameList[playerRanking[4] - 1] + "\n" + "6th:" +
                                                playerNameList[playerRanking[5] - 1];
    }

    IEnumerator explainRules()
    {
        yield return new WaitForSeconds(5f);
    }
}


