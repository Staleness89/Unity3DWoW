#define LOAD_ADDITIVE_ASYNC

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

public class TileNavigation : MonoBehaviour
{
	public struct TerrainIndex
	{
		public int col;
		public int row;

		public TerrainIndex( int col, int row )
		{
			this.col = col;
			this.row = row;
		}
	}

	struct TerrainAttachment
	{
		public GameObject terrain;
		public List<GameObject> attachments;
	}

	public struct TerrainTask
	{
		public int col;
		public int row;
		public string terrainName;
	}

	public const int TERRAIN_SLICED_COLLUMN = 2;
	public const int TERRAIN_SLICED_ROW = 2;
	public const float TILE_SIZE = 512.0f; //2000.0f;
	public static string TERRAIN_NAME = "Terrain_sand_1024_test_128hightmap_resolution_Slice_{0:G}_{1:G}";
	//public const string SCENE_NAME = "Terrain_sand_withman_{0:G}_{1:G}";
	private StringBuilder terrainName = new StringBuilder( 50 );
	private StringBuilder sceneName = new StringBuilder( 20 );
    //public static string SCENE_NAME_DEL = "";
    private float mHalfFOV = 30.0f;
	private float mCameraMinForward;
	private float mCameraMaxForward;
	private float mHalfHorizontalFOV;
	private int mCameraCol;
	private int mCameraRow;
	
	public static float sTopSide;
	public static float sBottomSide;
	public static float sLeftSide;
	public static float sRightSide;
	
	public static List<TerrainTask> sTerrainTasks = new List<TerrainTask>();
	public static ArrayList sTerrainRecords;
	private List<TerrainAttachment> mTerrainAttachments;
	private int mTerrainNum = 1;
	
	// Use this for initialization
	void Start()
	{
		Camera camera = Camera.main;
		mHalfFOV = camera.fieldOfView * 0.5f; // half
		mHalfHorizontalFOV = mHalfFOV * camera.aspect;
		mHalfHorizontalFOV = Mathf.Deg2Rad * mHalfHorizontalFOV;
		mCameraMinForward = Vector3.Angle( camera.transform.forward, -Vector3.up );
		mCameraMinForward -= mHalfFOV;
		mCameraMinForward = Mathf.Deg2Rad * mCameraMinForward;
		mCameraMaxForward = mCameraMinForward + Mathf.Deg2Rad * camera.fieldOfView;

		sTerrainRecords = ArrayList.Synchronized( new ArrayList() );
		UpdateCameraTile();
		sTerrainRecords.Add( new TerrainIndex( mCameraCol, mCameraRow ) );
		mTerrainAttachments = new List<TerrainAttachment>();
		TerrainAttachment ta = new TerrainAttachment();
		ta.terrain = GameObject.Find( "Terrain_original" );
		ta.attachments = new List<GameObject>();
		mTerrainAttachments.Add( ta );
	}
	
	// Update is called once per frame
	void Update()
	{	
		UpdateCameraTile();
		
		Vector3 cameraPosition = GetComponent<Camera>().transform.position;
		float viewHeight1 = cameraPosition.y * Mathf.Tan( mCameraMinForward ); // the top edge
		float viewHeight2 = cameraPosition.y * Mathf.Tan( mCameraMaxForward ); // the bottom edge
		float viewWidth = cameraPosition.y / Mathf.Cos( mCameraMaxForward ) * Mathf.Tan( mHalfHorizontalFOV );
		
		float viewHeightCompensator1 = 512.0f; // the compensator for the top edge
		float viewHeightCompensator2 = 512.0f; // the compensator for the bottom edge
		float viewWidthCompensator = 512.0f; // the compensator for width
		
		sTopSide = cameraPosition.x - viewHeight2 - viewHeightCompensator2 - TILE_SIZE;
		sBottomSide = cameraPosition.x - viewHeight1 + viewHeightCompensator1 + TILE_SIZE;
		sLeftSide = cameraPosition.z - viewWidth - viewWidthCompensator - TILE_SIZE;
		sRightSide = cameraPosition.z + viewWidth + viewWidthCompensator + TILE_SIZE;
		
		/*//(column, row)//
		        
		 -1, 1	|  1, 1
				|
		-----------------
				|
		 -1, -1	|  1, -1
				|
		//////////////////*/
		
		TerrainIndex leftTopTile = GetTileInfo( cameraPosition.x - viewHeight2 - viewHeightCompensator2, cameraPosition.z - viewWidth - viewWidthCompensator );
		TerrainIndex rightTopTile = GetTileInfo( cameraPosition.x - viewHeight2 - viewHeightCompensator2, cameraPosition.z + viewWidth + viewWidthCompensator );
		TerrainIndex leftBottomTile = GetTileInfo( cameraPosition.x - viewHeight1 + viewHeightCompensator1, cameraPosition.z - viewWidth - viewWidthCompensator );

		for ( int row = leftTopTile.row; row >= leftBottomTile.row; row-- )
		{
			if ( row == 0 ) 
				continue;
			
			bool isBreak = false;
			for ( int col = leftTopTile.col; col <= rightTopTile.col; col++ )
			{
				if ( col == 0 ) continue;
				
				if ( !IsExistInScene( col, row ) )
				{
                    mTerrainNum++;

                    GetTerrainName(col, row);

                    float z = (col > 0 ? col - 1 : col) * TILE_SIZE;
                    float x = -(row < 0 ? row + 1 : row) * TILE_SIZE;

                    // for memory limitation, don't cache anything.
                    UnityEngine.Object t = Resources.Load("Prefabs/Maps/Simple/" + terrainName.ToString());
                    TerrainIndex tr = new TerrainIndex();
                    tr.col = col;
                    tr.row = row;
                    sTerrainRecords.Add(tr);
                    TerrainAttachment ta = new TerrainAttachment();

                    ta.attachments = new List<GameObject>(4);
                    ta.terrain = t as GameObject;
                    GameObject s = (GameObject)Instantiate(t, new Vector3(x, 0, z), Quaternion.identity);
                    t = null;
                    
					isBreak = true;
				}
			}
			
			if ( isBreak )
			{
				// add more tiles next frame
				break;
			}
		}
		
	}
	
	bool IsExistInScene( int col, int row )
	{
		for ( int i = 0; i < sTerrainRecords.Count; i++ )
		{
			TerrainIndex tr = (TerrainIndex)sTerrainRecords[i];
			if ( tr.col == col && tr.row == row )
			{
				return true;
			}
		}
		
		return false;
	}
	
	TerrainIndex GetTileInfo( float x, float z )
	{
		float rowX = x > 0 ? x + TILE_SIZE : x - TILE_SIZE;
		int row = -(int)( rowX / TILE_SIZE );
		float colZ = z > 0 ? z + TILE_SIZE : z - TILE_SIZE;
		int col = (int)( colZ / TILE_SIZE );
		
		return new TerrainIndex( col, row );
	}
	
	void UpdateCameraTile()
	{
		Vector3 cameraPosition = Camera.main.transform.position;
		float x = cameraPosition.x > 0 ? cameraPosition.x + TILE_SIZE : cameraPosition.x - TILE_SIZE;
		mCameraRow = -(int)( x / TILE_SIZE );
		float z = cameraPosition.z > 0 ? cameraPosition.z + TILE_SIZE : cameraPosition.z - TILE_SIZE;
		mCameraCol = (int)( z / TILE_SIZE );
	}

    void GetTerrainName( int col, int row )
	{
		int terrainCol = col % TERRAIN_SLICED_COLLUMN;
		int terrainRow = row % TERRAIN_SLICED_ROW;

		// the mapping function from col/row to the sliced terrain's index(terrainCol/terrainRow) is a piecewise function. 
		if ( terrainCol == 0 )
		{
			if ( col > 0 )
			{
				terrainCol = TERRAIN_SLICED_COLLUMN;
			}
			else
			{
				terrainCol = 1;
			}
		}
		else
		{
			if ( col < 0 )
			{
				terrainCol = TERRAIN_SLICED_COLLUMN + 1 - (int)Mathf.Abs( terrainCol );
			}
		}
		
		if ( terrainRow == 0 )
		{
			if ( row > 0 )
			{
				terrainRow = TERRAIN_SLICED_ROW;
			}
			else
			{
				terrainRow = 1;
			}
		}
		else
		{
			if ( row < 0 )
			{
				terrainRow = TERRAIN_SLICED_ROW + 1 - (int)Mathf.Abs( terrainRow );
			}
		}
		
		terrainName.Length = 0; // terrainName.Remove( 0, terrainName.Length);
		//sceneName.Length = 0;
		terrainName.AppendFormat( TERRAIN_NAME, terrainCol, terrainRow );
		//sceneName.AppendFormat( SCENE_NAME, terrainCol, terrainRow );
        //SCENE_NAME_DEL = "Terrain_sand_withman_" + terrainCol.ToString() + "_" + terrainRow.ToString();

    }
    

}
