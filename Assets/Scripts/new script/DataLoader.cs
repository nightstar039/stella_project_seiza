using UnityEngine;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class ConstellationData
{
    public Element[] elements;
}

public class DataLoader : MonoBehaviour {

    private Element[] elements;
    private float skyRadius = 500f;  // 天球
    public float minStarSize = 0.05f;  // 星の最小サイズ
    public float maxStarSize = 0.5f;   // 星の最大サイズ

    void Start()
    {
        LoadConstellationData();
        DrawConstellations(elements);
    }

    void LoadConstellationData()
    {
        // 1. JSONファイルを読み込む
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("constellation_data");

        if (jsonTextAsset == null)
        {
            Debug.LogError("constellation_data.jsonが見つかりません。フォルダに配置されていますか？");
            return;
        }

        string jsonContext = jsonTextAsset.text;

        // 2. JSONをC#オブジェクトにデシリアライズする
        try
        {
            elements = JsonUtility.FromJson<ConstellationData>("{\"elements\":" + jsonContext + "}").elements;

            if (elements.Length > 0)
            {
                Debug.Log("データの読み込みに成功しました。要素数は " + elements.Length);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSONのデシリアライズに失敗しました: " + e.Message);
        }
    }

    void DrawConstellations(Element[] elements)
    {
        GameObject constellationParent = new GameObject("Constellations");
        LineRenderer currentLineRenderer = null;
        string currentConstellationName = "";

        foreach (var element in elements)
{
            if (currentConstellationName != element.constellation_name)
            {
                // 新しい星座のLineRendererを作成
                GameObject constellationObject = new GameObject(element.constellation_name);
                constellationObject.transform.SetParent(constellationParent.transform);
                currentLineRenderer = constellationObject.AddComponent<LineRenderer>();

                // LineRendererの設定
                currentLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                currentLineRenderer.startWidth = 0.1f;
                currentLineRenderer.endWidth = 0.1f;
                currentLineRenderer.positionCount = 0;
                currentLineRenderer.useWorldSpace = true;
                currentConstellationName = element.constellation_name;
            }

            // 座標変換
            Vector3 startPos = EquatorialToCartesian(element.right_ascension_lon_temp, element.dec_start, skyRadius);
            Vector3 endPos = EquatorialToCartesian(element.right_ascension_lon_end, element.dec_end, skyRadius);

            // 星を配置する
            // 開始点
            DrawStar(startPos, element.apparent_magnitude_temp, element.Star_name_temp, element.constellation_name);
            // 終了点
            DrawStar(endPos, element.apparent_magnitude_end, element.Star_name_end, element.constellation_name);

            // LineRendererに頂点を追加
            if (currentLineRenderer != null)
            {
                // LineRendererに座標を追加（線の始点と終点）
                int count = currentLineRenderer.positionCount;
                currentLineRenderer.positionCount = count + 2;
                currentLineRenderer.SetPosition(count, startPos);
                currentLineRenderer.SetPosition(count + 1, endPos);
            }
        }
    }

    // 星を描画する
    void DrawStar(Vector3 position, float apparentMagnitude, string starName, string constellationName)
    {
        if (string.IsNullOrEmpty(starName))
        {
            return;
        }

        // 星の大きさを等級から計算
        float size = Mathf.Lerp(maxStarSize, minStarSize, (apparentMagnitude + 1) / 6.0f);

        // すでに配置されているかチェック
        GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        star.name = starName + " (" + constellationName + ")";

        // マテリアル
        Material star_material = new Material(Shader.Find("Unlit/Color"));  // 背景に関係なく光る
        star_material.color = Color.white;  // 一旦白に
        star.GetComponent<Renderer>().material = star_material;

        // 星を親オブジェクトの下に配置
        star.transform.SetParent(GameObject.Find("Constellations").transform);

        // デバッグ用に一時的にサイズを大きくしてみる
        star.transform.position = position;
        star.transform.localScale = Vector3.one * size;
    // 
    }

    // 3D座標に変換する
    Vector3 EquatorialToCartesian(float ra, float dec, float radius)
    {
        float raRad = ra * Mathf.Deg2Rad;  // 赤経をラジアンに変換
        float decRad = dec * Mathf.Deg2Rad;  // 赤緯をラジアンに変換

        float x = radius * Mathf.Cos(decRad) * Mathf.Cos(raRad);  // x軸
        float z = radius * Mathf.Cos(decRad) * Mathf.Sin(raRad);  // z軸
        float y = radius * Mathf.Sin(decRad); // y軸

        return new Vector3(x, y, z);
    }
}

