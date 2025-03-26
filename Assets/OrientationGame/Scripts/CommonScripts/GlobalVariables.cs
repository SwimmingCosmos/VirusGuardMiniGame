using UnityEngine;
using System.Collections.Generic;

// グローバル変数クラス
// シーンが遷移しても保存されたままのため、ミニゲーム開始時に初期化するスクリプトを作成する必要がある
public static class GlobalVariables
{
    // プレイヤーネーム毎のクレジット
    public static Dictionary<string, int> CREDIT_PER_PLAYER_NAME = new Dictionary<string, int>() {};

    public static void SetCreditPerPlayerName(List<string> playerNames, List<int> credits)
    {
        CREDIT_PER_PLAYER_NAME.Clear();
        for (int i = 0; i < playerNames.Count; i++)
        {
            CREDIT_PER_PLAYER_NAME.Add(playerNames[i], credits[i]);
        }
    }

    // PlayerId毎の順位
    public static Dictionary<int, int> RANK_PER_ID = new Dictionary<int, int>() {};

    public static void SetRankList(List<int> rankList)
    {
        RANK_PER_ID.Clear();
        for (int i = 0; i < rankList.Count; i++)
        {
            RANK_PER_ID.Add(i, rankList[i]);
        }
    }
}
