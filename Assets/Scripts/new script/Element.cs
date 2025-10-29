using UnityEngine;
using System;

/**
 * JSONデータの各オブジェクトに対応するクラス 
**/
[Serializable]
public class Element
{
    // JSONのキー名と一致させる
    public string Abbreviation;  // 星座の「略称」
    public int Group;  // 星座線を構成する要素の「グループ番号」
    public string Abbreviation_Group;  // 星座の「略称」と「グループ番号」のグループ
    public string constellation_name;  // 星座名

    // 恒星のHIP番号 
    public int HIP_start;  // 始点となる星のHIP番号
    public int HIP_end;  // 終点となる星のHIP番号
    public int HIP_temp;  // 一時的に使用するHIP番号(=HIP_startになる場合が多い)

    // 位置情報（度単位）
    public float right_ascension_lon_temp;  // 経度（赤経）の開始点
    public float right_ascension_lon_end;  // 経度（赤経）の終了点
    public float dec_start;  // 緯度（赤緯）の開始点
    public float dec_end;  // 緯度（赤緯）の終了点

    // 明るさ（等級、見かけ）
    public float apparent_magnitude_temp;  // 開始点の明るさ
    public float apparent_magnitude_end;  // 終了点の明るさ

    // 星の名前
    public string Star_name_temp;  // 始点の星の名前
    public string Star_name_end;  // 終点の星の名前

    [Serializable]
    public class ConstellationData
    {
        // JSONファイル全体がこのオブジェクトの配列である場合
        public Element[] elements;  // Elementオブジェクトの配列
    }
}
