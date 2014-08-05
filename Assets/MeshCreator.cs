using UnityEngine;
using System.Collections;

public class MeshCreator : MonoBehaviour 
{
    int count;

    public float squareSize;

    // Use this for initialization
	void Start () {
        count = 0;

		float x = -10.2f;
		float y = -4.6f;
		float z = 0.0f;

		for(int index = 0; index < 9700; index ++)
		{
			CreateGameobjectWithMesh(squareSize, new Vector3(x, y, z), true);

			x+= squareSize;

			if(x > 10.2f)
			{
				x = -10.2f;

				y += squareSize;

				if(y > 8.0f)
				{
					y = -4.6f;

					z += 0.2f;
				}
			}
		}

		for(int index = 9700; index < 100000; index ++)
		{
			CreateGameobjectWithMesh(squareSize, new Vector3(x, y, z), false);
			
			x+= squareSize;
			
			if(x > 10.2f)
			{
				x = -10.2f;
				
				y += squareSize;
				
				if(y > 7.0f)
				{
					y = -4.6f;
					
					z += 0.2f;
				}
			}
		}

//        for (float x = -10.2f; x <= 10.2f; x += squareSize)
//        {
//            for (float y = -4.6f; y <= 7.0f; y += squareSize)
//            {
//                for (float z = 0.0f; z >= 0.0f; z -= 1.0f)
//                {
//                    //x += Random.Range(-squareSize / 2.0f, squareSize / 2.0f);
//                    //y += Random.Range(-squareSize / 2.0f, squareSize / 2.0f);
//
//                    CreateGameobjectWithMesh(squareSize, new Vector3(x, y, z));
//                }
//            }
//        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void CreateGameobjectWithMesh(float size, Vector3 position, bool enable)
    {
        count++;

        GameObject meshContainer = new GameObject("SimplePlane" + count.ToString());
        meshContainer.AddComponent<MeshRenderer>();
        meshContainer.AddComponent<MeshFilter>();

        Mesh simpleMesh = new Mesh();
        meshContainer.GetComponent<MeshFilter>().mesh = simpleMesh;

        Vector3[] vertices = { 
                                 new Vector3(-size, -size, 0.0f), 
                                 new Vector3(size, -size, 0.0f), 
                                 new Vector3(size, size, 0.0f), 
                                 new Vector3(-size, size, 0.0f) 
                             };

        Vector2[] uvs = {
                             new Vector2 (0, 0), 
                             new Vector2 (0, 1), 
                             new Vector2(1, 1), 
                             new Vector2 (1, 0)
                        };

        int[] triangles = {0, 1, 2, 0, 2, 3};

        simpleMesh.vertices = vertices;
        simpleMesh.uv = uvs;
        simpleMesh.triangles = triangles;

        simpleMesh.RecalculateNormals();

        meshContainer.transform.position = position;

		meshContainer.SetActive (enable);
    }
}
