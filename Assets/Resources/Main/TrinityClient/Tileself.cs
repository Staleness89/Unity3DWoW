using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Tileself : MonoBehaviour
{ 
	int mCol = 1;
	int mRow = -1;
	
	// Use this for initialization
	void Start ()
	{
		lock( TileNavigation.sTerrainTasks )
		{
			for ( int i = 0; i < TileNavigation.sTerrainTasks.Count; i++ )
			{
				TileNavigation.TerrainTask tt = (TileNavigation.TerrainTask)TileNavigation.sTerrainTasks[i];
				if ( tt.terrainName == gameObject.name )
				{
					mCol = tt.col;
					mRow = tt.row;
					float z = ( mCol > 0 ? mCol - 1 : mCol ) * TileNavigation.TILE_SIZE;
					float x = -( mRow < 0 ? mRow + 1 : mRow ) * TileNavigation.TILE_SIZE;
					gameObject.transform.position = new Vector3( x, 0, z );
					TileNavigation.sTerrainTasks.RemoveAt( i );
					break;
				}
			}
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		bool isDelete = false;
		Vector3 pos = transform.position;
		if ( pos.x < TileNavigation.sTopSide )
		{
			isDelete = true;
		}
		else if ( pos.x > TileNavigation.sBottomSide )
		{
			isDelete = true;
		}
		else if ( pos.z < TileNavigation.sLeftSide )
		{
			isDelete = true;
		}
		else if ( pos.z > TileNavigation.sRightSide )
		{
			isDelete = true;
		}
		
		if ( isDelete )
		{
			lock ( TileNavigation.sTerrainRecords )
			{
				for ( int j = 0; j < TileNavigation.sTerrainRecords.Count; j++ )
				{
					TileNavigation.TerrainIndex tr = (TileNavigation.TerrainIndex)TileNavigation.sTerrainRecords[j];

					if ( tr.col == mCol && tr.row == mRow )
					{
			                        TileNavigation.sTerrainRecords.RemoveAt( j );
						break;
					}
				}
			}
		    
			// Because the purpose of multithread loading terrain is for memory limit, so don't cache them. besides this, every terrain object may have different state in games.
			DestroyImmediate( gameObject );
            Destroy(GameObject.Find(TileNavigation.TERRAIN_NAME));
            Resources.UnloadUnusedAssets();
		}
	}
}
