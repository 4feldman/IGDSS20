using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Texture2D heightmap;

    public GameObject waterPrefab;
    public GameObject sandPrefab;
    public GameObject grassPrefab;
    public GameObject forestPrefab;
    public GameObject stonePrefab;
    public GameObject mountainPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < heightmap.height; i++)
        {
            for (int j = 0; j < heightmap.width; j++)
            {
                float val = heightmap.GetPixel(i,j).b;
                Vector3 pos;
                if (i % 2 == 1)
                {
                    pos = new Vector3(i * 8.66f, val * 10, j * 10 + 5);
                }
                else
                {
                    pos = new Vector3(i * 8.66f, val * 10, j * 10);
                }


                if(val == 0.0f)
                {
                    Instantiate(waterPrefab, pos, Quaternion.identity);
                }
                else if(val < 0.2f)
                {
                     Instantiate(sandPrefab, pos, Quaternion.identity);
                }
                else if(val < 0.4f)
                {
                     Instantiate(grassPrefab, pos, Quaternion.identity);
                }
                else if(val < 0.6f)
                {
                     Instantiate(forestPrefab, pos, Quaternion.identity);
                }
                else if(val < 0.8f)
                {
                     Instantiate(stonePrefab, pos, Quaternion.identity);
                }
                else
                {
                     Instantiate(mountainPrefab, pos, Quaternion.identity);
                }
            }
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
