using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class star_drawing : MonoBehaviour
{
    public TextAsset starCSV;
    public TextAsset constellationLineCsv;
    public TextAsset constellationNameCSV;

    public GameObject starPrefab;
    public Material lineMaterial;
    public float moveSpeed = 50f;

    private Dictionary<int, star_data> hipToStar = new Dictionary<int, star_data>();
    private Dictionary<string, string> abbrtoName = new Dictionary<string, string>();

    private const float skySphereradius = 1000f;
    private const float magnigtudeScale = -0.5f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, -skySphereradius + 50);

        ParseStarData();
        DrawStars();
        ParseConstellationNames();
        DrawConstellationLines();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ParseStarData()
    {
        string[] lines = starCSV.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            if (row.Length >= 4)
            {
                star_data star = new star_data();
                star.hip = int.Parse(row[0]);
                star.rightAscention = float.Parse(row[1]);
                star.declination = float.Parse(row[2]);
                star.apparentMagnitude = float.Parse(row[3]);
                star.starName = row.Length > 4 && !string.IsNullOrEmpty(row[4]) ? row[4] : "";

                float raRad = star.rightAscention * Mathf.Deg2Rad;
                float decRad = star.declination * Mathf.Deg2Rad;

                float x = skySphereradius * Mathf.Cos(decRad) * Mathf.Cos(raRad);
                float y = skySphereradius * Mathf.Sin(decRad);
                float z = skySphereradius * Mathf.Cos(decRad) * Mathf.Sin(raRad);

                star.position = new UnityEngine.Vector3(x, y, z);

                hipToStar[star.hip] = star;
            }
        }
    }

    public void DrawStars()
    {
        foreach (var star in hipToStar.Values)
        {
            GameObject starObject = Instantiate(starPrefab, star.position, UnityEngine.Quaternion.identity, transform);

            float scale = Mathf.Pow(2f, (4f - star.apparentMagnitude) * magnigtudeScale);
            starObject.transform.localScale = UnityEngine.Vector3.one * scale;

            //starObject.GetComponentInChildren<TextMesh>().text = star.starName;
        }
    }

    public void ParseConstellationNames()
    {
        string[] lines = constellationNameCSV.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            if (row.Length >= 3)
            {
                abbrtoName[row[1]] = row[2];
            }
        }
    }

    public void DrawConstellationLines()
    {
        string[] lines = constellationLineCsv.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            if (row.Length >= 5)
            {
                int hip1 = int.Parse(row[3]);
                int hip2 = int.Parse(row[4]);

                if (hipToStar.ContainsKey(hip1) && hipToStar.ContainsKey(hip2))
                {
                    star_data star1 = hipToStar[hip1];
                    star_data star2 = hipToStar[hip2];

                    GameObject lineObject = new GameObject("Line_" + row[2]);
                    lineObject.transform.SetParent(transform);
                    LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

                    lineRenderer.material = lineMaterial;
                    lineRenderer.startWidth = 0.5f;
                    lineRenderer.endWidth = 0.5f;
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, star1.position);
                    lineRenderer.SetPosition(1, star2.position);
                }
            }
        }
    }

}
