using System.Text.RegularExpressions;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private float ballSpeed = 10.0f;
    private float minBallSpeed = 6.0f;
    private float maxBallSpeed = 20.0f;
    private CreatingBalls creatingBalls;
    private Rigidbody myBallRigidbody;
    
    void Start()
    {
        myBallRigidbody = GetComponent<Rigidbody>();
        var randomDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0).normalized;
        myBallRigidbody.linearVelocity = randomDirection * ballSpeed;
        creatingBalls = GameObject.Find("Script").GetComponent<CreatingBalls>();
    }
    
    void Update()
    {
        Vector3 currentBallVelocity = myBallRigidbody.linearVelocity;
        float adjustSpeed = Mathf.Clamp(currentBallVelocity.magnitude, minBallSpeed, maxBallSpeed);
        myBallRigidbody.linearVelocity = currentBallVelocity.normalized * adjustSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // ランダムに跳ね返るようにしたい
            int randomNum = Random.Range(0, 1);

            switch (randomNum)
            {
                case 0:
                    //シンプルに反射
                    ContactPoint contact1 = collision.contacts[0];
                    Vector3 normal1 = contact1.normal;
            
                    // 反射ベクトルを計算
                    Vector3 reflectDir1 = Vector3.Reflect(myBallRigidbody.linearVelocity.normalized, normal1);
                    myBallRigidbody.linearVelocity = reflectDir1 * myBallRigidbody.linearVelocity.magnitude;
                    break;
                case 1:
                    //内角の方向に反射する
                    
                    // プレイヤーの位置を取得
                    Vector3 playerPos = collision.transform.position;
                    // ボールの位置を取得
                    Vector3 ballPos = transform.position;
                    // プレイヤーから見たボールの方向を計算
                    Vector3 direction = (ballPos - playerPos).normalized;
                    // 現在の速さを取得
                    float speed = myBallRigidbody.linearVelocity.magnitude;
                    // 速度を変更
                    myBallRigidbody.linearVelocity = direction * speed;
                    break;
                
                //使わないことにしました
                case 2:
                default:
                    
                    ContactPoint contact = collision.contacts[0];
                    Vector3 normal = contact.normal;
            
                    // 反射ベクトルを計算
                    Vector3 reflectDir = Vector3.Reflect(myBallRigidbody.linearVelocity.normalized, normal);
            
                    // 反射前の方向ベクトルを取得
                    float currentAngle = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
            
                    // ランダムな角度を現在の角度から±30度の範囲で変更
                    float randomAngle = Random.Range(-30.0f, 30.0f);
                    float newAngle = currentAngle + randomAngle;
            
                    // 新しい方向ベクトルを計算
                    Vector3 newReflectDir = new Vector3(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad), 0);
            
                    // 速度を維持して適用
                    myBallRigidbody.linearVelocity = newReflectDir * myBallRigidbody.linearVelocity.magnitude;
                    break;

            }
            
        }
        else if (collision.gameObject.CompareTag("ServerWall"))
        {
            collision.gameObject.GetComponent<Renderer>().material.color = Color.red;
            //ぶつかった壁の名前から数字を抽出する
            int serverNumber = CollisionCallbackNumber(collision.gameObject.name);
            creatingBalls.ServerHit(serverNumber,collision.contacts[0].point);
            //Debug.Log("serverNumber:"+serverNumber);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ball"))
        {
            ContactPoint contact1 = collision.contacts[0];
            Vector3 normal1 = contact1.normal;
            
            // 反射ベクトルを計算
            Vector3 reflectDir1 = Vector3.Reflect(myBallRigidbody.linearVelocity.normalized, normal1);
            myBallRigidbody.linearVelocity = reflectDir1 * myBallRigidbody.linearVelocity.magnitude;
        }
    }
    
    //ぶつかったサーバーを識別するための関数。int型で返す
    private int CollisionCallbackNumber(string serverName)
    {
        // `serverWall(数字)` の部分を抽出する
        Match match = Regex.Match(serverName, @"\((\d+)\)"); 
        if (match.Success)
        {
            
            return (int.Parse(match.Groups[1].Value)-1); 
        }
        return -1; // 見つからなかった場合
    }
    
}

