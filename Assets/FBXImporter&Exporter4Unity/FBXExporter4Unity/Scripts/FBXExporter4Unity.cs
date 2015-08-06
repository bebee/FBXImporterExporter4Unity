/*
 * FBXExporter4Unity.cs
 * 
 *  	Developed by ほえたん(Hoetan) -- 2015/08/07
 *  	Copyright (c) 2015, ACTINIA Software. All rights reserved.
 * 		Homepage: http://actinia-software.com
 * 		E-Mail: hoetan@actinia-software.com
 * 		Twitter: https://twitter.com/hoetan3
 * 		GitHub: https://github.com/hoetan
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WWWDownloaderExporter : MonoBehaviour
{
	public bool bWWWDownloadEnd = false;
	public int iWWWDownloadCount = 0;

	public Texture2D texture = null;
	public byte[] bytes;

	public IEnumerator WWWDownloadToLoadFileTexture(string strURLFileName, string strWriteFileName, bool bUnloadUnusedAssets, System.Action callback)
	{
		bWWWDownloadEnd = false;
		iWWWDownloadCount++;
		
		WWW www = new WWW(strURLFileName);
		
		yield return www;
		
		while (www.isDone == false)
		{
			yield return null;
		}
		
		texture = www.texture;
		bytes = www.bytes;

		www.Dispose();
		www = null;
		
		if (bUnloadUnusedAssets)
		{
			Resources.UnloadUnusedAssets();
		}
		
		iWWWDownloadCount--;
		if (iWWWDownloadCount <= 0)
		{
			bWWWDownloadEnd = true;
		}
		
		callback();
	}
}

public class FBXExporter4Unity : MonoBehaviour {
	[DllImport("FBXExporter4Unity")]
	public static extern int FBXExporterInit();
    [DllImport("FBXExporter4Unity")]
	public static extern int FBXExporterExit();
    [DllImport("FBXExporter4Unity")]
	public static extern bool FBXExporterSave(string pcName, int iFileFormat, bool bEmbedMedia);
	[DllImport("FBXExporter4Unity")]
	public static extern bool FBXExporterSetNode(string pcAttachParentNodeName, string pcNodeName, float[] fLclTranslation, float[] fLclRotation, float[] fLclScale);
    [DllImport("FBXExporter4Unity")]
	public static extern bool FBXExporterSetAnimationStackBaseLayer(string pcStack, string pcLayer);
	[DllImport("FBXExporter4Unity")]
	public static extern bool FBXExporterSetAnimationTime(float fTime);
	[DllImport("FBXExporter4Unity")]
	public static extern bool FBXExporterSetAnimationTRS(string pcNodeName, string pcTakeName, float[] fTranslation, float[] fRotation, float[] fScale, bool bCreate);
	[DllImport("FBXExporter4Unity")]
	public static extern bool FBXExporterSetAnimationsTRS(string pcNodeName, string pcTakeName, int FrameCount, float[] fTime, float[] fTranslation, float[] fRotation, float[] fScale, bool bCreate);
	[DllImport("FBXExporter4Unity")]
	public static extern bool FBXExporterSetNodeMesh(string pcAttachParentNodeName, string pcNodeMeshName, float[] fVertex, float[] fNormal, float[] fUV, int iVertexCount, int[] iVertexIndex, int[] iVertexIndexMaterialID, int iVertexIndexCount, float[] fLclTranslation, float[] fLclRotation, float[] fLclScale, bool bDirectControlPointsMode, bool bNormalSmoothing);
	[DllImport("FBXExporter4Unity")]
	public static extern bool FBXExporterSetSkinBoneName(string pcNodeMeshName, int i, string pcBoneName);
	[DllImport("FBXExporter4Unity")]
	public static extern bool FBXExporterSetSkinBoneWeight(string pcNodeMeshName, int[] iBoneIndex, float[] fBoneWeight, int iVertexCount);
    [DllImport("FBXExporter4Unity")]
	public static extern bool FBXExporterSetBindPose(string pcNodeMeshName);
    [DllImport("FBXExporter4Unity")]
	public static extern bool FBXExporterAddMaterial(string pcAddNodeName, string strMaterialName, float[] fAmbient, float[] fDiffuse, float[] fEmissive, float[] fTransparency, string strTextureDiffuseName);

	//private WWWDownloaderExporter sWWWDownloaderExporter;

	public enum EFBXFileFormat
	{
		Default = -1,
		Binary = 0,
		ASCII = 1,
		BinaryCompression = 2,
	}

	public enum EFBXImportTextrureMode
	{
		eAutoImportAssets,
		eLocalEmbedMedia,
		eLocalProject,
		eLocalStreamingAssets,
		eLocalAssets,
		eLocalFullPath,
		//eWWWFullPath,
		//eWWWURL,
	};

	public enum EFBXExportToolTarget
	{
		eDefault,
		eFBXImporter4Unity,
		eDCCTools,
		eUnity,
	};

	public struct SAnimBufferTRS
	{
		public List<float> listTime;
		public List<float> listLclPosition;
		public List<float> listLclRotation;
		public List<float> listLclScale;
	};

	public bool bDebugLog = true;
	public string strFilename = "Unity.fbx";
	public EFBXExportToolTarget eExportToolTarget = EFBXExportToolTarget.eDefault;
	public EFBXFileFormat eFileFormat = EFBXFileFormat.Default;
	public bool bOutDirectControlPointsMode = true;
	public bool bOutNormalSmoothing = true;
	public float fGlobalScale = 100.0f;
	public bool bOutRootNode = false;
	public string strOutRootNodeName = "Root";
	public EFBXImportTextrureMode eImportTextrureMode = EFBXImportTextrureMode.eAutoImportAssets;
	public bool bImportEmbedMediaTexturePath = true;
	public string strLocalEmbedMediaTexturePath = "";
	public string strLocalProjectTexturePath = "InputTextures/";
	public string strLocalStreamingAssetsTexturePath = "InputTextures/";
	public string strLocalAssetsTexturePath = "Assets/InputTextures/";
	public string strLocalFullPathTexturePath = "";
	//public string strWWWFullPathTexturePath = "file://";
	//public string strWWWURLTexturePath = "http://";
	public bool bEmbedMedia = true;
	public bool bOutMesh = true;
	public bool bOutAnimation = true;
	public bool bOutAnimationCustomFrame = false;
	public bool bAnimationBufferRecMode = true;
	public bool bEditorAutoStop = false;
	public string strStack = "Stack0";
	public string strLayer = "Layer0";
	public string strTake = "Take0";
	public float fTime = 0.0f;
	public float fTimeStart = 0.0f;
	public float fTimeRecStart = 0.0f;
	public float fTimeRecEnd = 3.0f;
	public int iFrameRate = 30;
	public bool bEnableSync = true;
	public int iTargetFrameRate = 30;
	public int iVSyncCount = 0;
	public bool bOutRealTime = false;
	public bool bOutTexture = true;
	public bool bConvertAllPNG = true;
	public bool bOverWriteTexture = true;
	public string strOutputTexturePath = "OutputTextures/";
	private bool bFirst = false;
	private bool bStartupFBX = true;
	private bool bNowFBX = false;
	private bool bEndFBX = false;
	private bool bSave = false;
	
	private bool bTrial = false;

	private GameObject[] goRoot;
	private Dictionary<string, string> dicDiffuseTexs = new Dictionary<string, string>();

	private Dictionary<string, SAnimBufferTRS> dicAnimBufferTRSs = new Dictionary<string, SAnimBufferTRS>();
	
	void Awake() {
		// WWW Downloader
		//sWWWDownloaderExporter = gameObject.AddComponent<WWWDownloaderExporter>();

		if (bEnableSync)
		{
			Application.targetFrameRate = iTargetFrameRate;
			QualitySettings.vSyncCount = iVSyncCount;
		}

		switch (eExportToolTarget)
		{
			case EFBXExportToolTarget.eDefault:
				break;
			case EFBXExportToolTarget.eFBXImporter4Unity:
				bOutDirectControlPointsMode = true;
				break;
			case EFBXExportToolTarget.eDCCTools:
				bOutDirectControlPointsMode = true;
				bOutNormalSmoothing = true;
				break;
			case EFBXExportToolTarget.eUnity:
				bOutDirectControlPointsMode = false;
				break;
		}
	}

	void StartFBX() {
		bool bOutFBX = true;
		if (bOutFBX) {
			bTrial = false;
			bSave = false;
			dicDiffuseTexs.Clear();
			dicAnimBufferTRSs.Clear();
			if (bDebugLog) Debug.Log("FBXExporterInit()");
			FBXExporterInit();
			if (bDebugLog) Debug.Log("ExportSceneRoot()");
			fTime = fTimeStart;
			FBXExporterSetAnimationTime(fTime);
			ExportSceneRoot(bOutRootNode);
			goRoot = transform.FindRootObject();
			if (bDebugLog) Debug.Log("ExportTransfromRoot(Mesh Off) Time:" + fTime);
			ExportTransfromRoot(goRoot, false);
			if (bOutMesh)
			{
				if (bDebugLog) Debug.Log("ExportTransfromRoot(Mesh On) Time:" + fTime);
				ExportTransfromRoot(goRoot, true);
			}
			if (bOutAnimation || bOutAnimationCustomFrame) {
				if (bDebugLog) Debug.Log("ExportAnimTransfromRoot(Create On) Time:" + fTime);
				FBXExporterSetAnimationStackBaseLayer(strStack, strLayer);
				ExportAnimTransfromRoot(goRoot, true);
				bNowFBX = true;
			}
			bFirst = true;
		}
    }

	void LateUpdate()
    {
		if (!bFirst) {
			Process();
		}
	}

	void OnPostRender()
	{
		Process();
	}

	void Process() {
		if (!bFirst) {
			if (bStartupFBX && !bEndFBX) {
				StartFBX();
			}
		}
		else {
			if (bOutAnimation) {
				if (bOutRealTime) {
					fTime += Time.deltaTime;
				}
				else {
					fTime += (1.0f / iFrameRate);
				}
				if (bNowFBX && !bEndFBX) {
					if (fTime >= fTimeRecStart) {
						if (bDebugLog) Debug.Log("ExportAnimTransfromRoot(Create Off) Time:" + fTime);
						if (!bAnimationBufferRecMode)
						{
							FBXExporterSetAnimationTime(fTime);
						}
						ExportAnimTransfromRoot(goRoot, false);
					}
				}
			}
			if (fTime >= fTimeRecEnd || !bOutAnimation || bTrial || bEndFBX)
			{
				if (!bOutAnimation)
				{
					if (bOutAnimationCustomFrame) {
						return;
					}
				}
				if (!bSave) {
					fTime = 0.0f;
					bFirst = false;
					bSave = true;
					if (bAnimationBufferRecMode)
					{
						ExportAllNodesAnimBufferTRSs();
					}
					if (bDebugLog) Debug.Log("FBXExporterSave()");
					FBXExporterSave(strFilename, (int)eFileFormat, bEmbedMedia);
					if (bDebugLog) Debug.Log("FBXExporterExit()");
					FBXExporterExit();
					if (bTrial || bAnimationBufferRecMode) {
						if (bDebugLog) Debug.LogWarning("Free Trial version is limited over 90 frames(30fps = 3sec) animations!");
					}
					bNowFBX = false;
					bEndFBX = true;
					#if UNITY_EDITOR
					if (bEditorAutoStop)
					{
						EditorApplication.isPlaying = false;
					}
					#endif
				}
			}
		}
	}
	
	void OnApplicationQuit() {
		if (bFirst) {
			bool bOutFBX = true;
			if (!bSave && bOutFBX) {
				fTime = 0.0f;
				bFirst = false;
				bSave = true;
				if (bAnimationBufferRecMode)
				{
					ExportAllNodesAnimBufferTRSs();
				}
				if (bDebugLog) Debug.Log("FBXExporterSave()");
				FBXExporterSave(strFilename, (int)eFileFormat, bEmbedMedia);
				if (bDebugLog) Debug.Log("FBXExporterExit()");
				FBXExporterExit();
				if (bTrial) {
					if (bDebugLog) Debug.Log("Free Trial version is limited over 90 frames(30fps = 3sec) animations!");
				}
				bNowFBX = false;
				bEndFBX = true;
				#if UNITY_EDITOR
				if (bEditorAutoStop)
				{
					EditorApplication.isPlaying = false;
				}
				#endif
			}
		}
	}
	
	void ExportAnimTransfromChildsFromParent(Transform transParent, bool bCreate)
    {
        for (int i = 0; i < transParent.transform.childCount; i++)
        {
			Transform trnsChild = transParent.transform.GetChild(i);
			if (!trnsChild.gameObject.activeInHierarchy) {
				continue;
			}
			float[] fTranslation = new float[3];
			fTranslation[0] = -trnsChild.localPosition.x * fGlobalScale;
			fTranslation[1] = trnsChild.localPosition.y * fGlobalScale;
			fTranslation[2] = trnsChild.localPosition.z * fGlobalScale;
			float[] fRotation = new float[4];
			fRotation[0] = -trnsChild.localRotation.x;
			fRotation[1] = trnsChild.localRotation.y;
			fRotation[2] = trnsChild.localRotation.z;
			fRotation[3] = -trnsChild.localRotation.w;
			float[] fScale = new float[3];
            fScale[0] = trnsChild.localScale.x;
            fScale[1] = trnsChild.localScale.y;
            fScale[2] = trnsChild.localScale.z;
			if (bAnimationBufferRecMode)
			{
				SAnimBufferTRS sAnimBufferTRS;
				if (dicAnimBufferTRSs.TryGetValue(trnsChild.name, out sAnimBufferTRS))
				{
					sAnimBufferTRS.listTime.Add(fTime);
					sAnimBufferTRS.listLclPosition.Add(fTranslation[0]);
					sAnimBufferTRS.listLclPosition.Add(fTranslation[1]);
					sAnimBufferTRS.listLclPosition.Add(fTranslation[2]);
					sAnimBufferTRS.listLclRotation.Add(fRotation[0]);
					sAnimBufferTRS.listLclRotation.Add(fRotation[1]);
					sAnimBufferTRS.listLclRotation.Add(fRotation[2]);
					sAnimBufferTRS.listLclRotation.Add(fRotation[3]);
					sAnimBufferTRS.listLclScale.Add(fScale[0]);
					sAnimBufferTRS.listLclScale.Add(fScale[1]);
					sAnimBufferTRS.listLclScale.Add(fScale[2]);
				}
				else
				{
					sAnimBufferTRS.listTime = new List<float>();
					sAnimBufferTRS.listTime.Add(fTime);
					sAnimBufferTRS.listLclPosition = new List<float>();
					sAnimBufferTRS.listLclPosition.Add(fTranslation[0]);
					sAnimBufferTRS.listLclPosition.Add(fTranslation[1]);
					sAnimBufferTRS.listLclPosition.Add(fTranslation[2]);
					sAnimBufferTRS.listLclRotation = new List<float>();
					sAnimBufferTRS.listLclRotation.Add(fRotation[0]);
					sAnimBufferTRS.listLclRotation.Add(fRotation[1]);
					sAnimBufferTRS.listLclRotation.Add(fRotation[2]);
					sAnimBufferTRS.listLclRotation.Add(fRotation[3]);
					sAnimBufferTRS.listLclScale = new List<float>();
					sAnimBufferTRS.listLclScale.Add(fScale[0]);
					sAnimBufferTRS.listLclScale.Add(fScale[1]);
					sAnimBufferTRS.listLclScale.Add(fScale[2]);
					dicAnimBufferTRSs.Add(trnsChild.name, sAnimBufferTRS);
				}
			}
			else
			{
				if (FBXExporterSetAnimationTRS(trnsChild.name, strTake, fTranslation, fRotation, fScale, bCreate) == false)
				{
					bTrial = true;
				}
			}
            ExportAnimTransfromChildsFromParent(trnsChild, bCreate);
        }
    }

    void ExportAnimTransfromRoot(GameObject[] goRoot, bool bCreate)
    {
        for (int i = 0; i < goRoot.Length; i++)
        {
			Transform trnsRoot = goRoot[i].transform;
			if (!trnsRoot.gameObject.activeInHierarchy) {
				continue;
			}
			float[] fRootTranslation = new float[3];
			fRootTranslation[0] = -trnsRoot.localPosition.x * fGlobalScale;
			fRootTranslation[1] = trnsRoot.localPosition.y * fGlobalScale;
			fRootTranslation[2] = trnsRoot.localPosition.z * fGlobalScale;
			float[] fRootRotation = new float[4];
			fRootRotation[0] = -trnsRoot.localRotation.x;
			fRootRotation[1] = trnsRoot.localRotation.y;
			fRootRotation[2] = trnsRoot.localRotation.z;
			fRootRotation[3] = -trnsRoot.localRotation.w;
			float[] fRootScale = new float[3];
			fRootScale[0] = trnsRoot.localScale.x;
			fRootScale[1] = trnsRoot.localScale.y;
			fRootScale[2] = trnsRoot.localScale.z;
			if (bAnimationBufferRecMode)
			{
				SAnimBufferTRS sAnimBufferTRS;
				if (dicAnimBufferTRSs.TryGetValue(trnsRoot.name, out sAnimBufferTRS))
				{
					sAnimBufferTRS.listTime.Add(fTime);
					sAnimBufferTRS.listLclPosition.Add(fRootTranslation[0]);
					sAnimBufferTRS.listLclPosition.Add(fRootTranslation[1]);
					sAnimBufferTRS.listLclPosition.Add(fRootTranslation[2]);
					sAnimBufferTRS.listLclRotation.Add(fRootRotation[0]);
					sAnimBufferTRS.listLclRotation.Add(fRootRotation[1]);
					sAnimBufferTRS.listLclRotation.Add(fRootRotation[2]);
					sAnimBufferTRS.listLclRotation.Add(fRootRotation[3]);
					sAnimBufferTRS.listLclScale.Add(fRootScale[0]);
					sAnimBufferTRS.listLclScale.Add(fRootScale[1]);
					sAnimBufferTRS.listLclScale.Add(fRootScale[2]);
				}
				else 
				{
					sAnimBufferTRS.listTime = new List<float>();
					sAnimBufferTRS.listTime.Add(fTime);
					sAnimBufferTRS.listLclPosition = new List<float>();
					sAnimBufferTRS.listLclPosition.Add(fRootTranslation[0]);
					sAnimBufferTRS.listLclPosition.Add(fRootTranslation[1]);
					sAnimBufferTRS.listLclPosition.Add(fRootTranslation[2]);
					sAnimBufferTRS.listLclRotation = new List<float>();
					sAnimBufferTRS.listLclRotation.Add(fRootRotation[0]);
					sAnimBufferTRS.listLclRotation.Add(fRootRotation[1]);
					sAnimBufferTRS.listLclRotation.Add(fRootRotation[2]);
					sAnimBufferTRS.listLclRotation.Add(fRootRotation[3]);
					sAnimBufferTRS.listLclScale = new List<float>();
					sAnimBufferTRS.listLclScale.Add(fRootScale[0]);
					sAnimBufferTRS.listLclScale.Add(fRootScale[1]);
					sAnimBufferTRS.listLclScale.Add(fRootScale[2]);
					dicAnimBufferTRSs.Add(trnsRoot.name, sAnimBufferTRS);
				}
			}
			else
			{
				if (FBXExporterSetAnimationTRS(trnsRoot.name, strTake, fRootTranslation, fRootRotation, fRootScale, bCreate) == false)
				{
					bTrial = true;
				}
			}
			ExportAnimTransfromChildsFromParent(trnsRoot, bCreate);
        }
	}

	void ExportAllNodesAnimBufferTRSs()
	{
		foreach (string key in dicAnimBufferTRSs.Keys)
		{
			SAnimBufferTRS sAnimBufferTRS = dicAnimBufferTRSs[key];
			float[] fTimes = sAnimBufferTRS.listTime.ToArray();
			float[] fTranslations = sAnimBufferTRS.listLclPosition.ToArray();
			float[] fRotations = sAnimBufferTRS.listLclRotation.ToArray();
			float[] fScales = sAnimBufferTRS.listLclScale.ToArray();
			FBXExporterSetAnimationsTRS(key, strTake, fTimes.Length, fTimes, fTranslations, fRotations, fScales, true);
		}
	}
    
    void ExportTransfromChildsFromParent(Transform transParent, bool bMesh)
    {
		for (int i = 0; i < transParent.transform.childCount; i++) {
			Transform trnsChild = transParent.transform.GetChild(i);
			if (!trnsChild.gameObject.activeInHierarchy) {
				continue;
			}
			ExportSetVetexIndex(transParent.name, trnsChild.gameObject, bMesh);
            ExportTransfromChildsFromParent(trnsChild, bMesh);
		}
	}

	void ExportTransfromRoot(GameObject[] goRoot, bool bMesh) {
		for (int i = 0; i < goRoot.Length; i++) {
			if (!goRoot[i].gameObject.activeInHierarchy) {
				continue;
			}
			ExportSetVetexIndex(strOutRootNodeName, goRoot[i].gameObject, bMesh);
			for (int j = 0; j < goRoot[i].transform.childCount; j++) {
				Transform trnsChild = goRoot[i].transform.GetChild(j);
				if (!trnsChild.gameObject.activeInHierarchy) {
					continue;
				}
				ExportSetVetexIndex(goRoot[i].name, trnsChild.gameObject, bMesh);
                ExportTransfromChildsFromParent(trnsChild, bMesh);
			}
		}
	}

	void ExportSceneRoot(bool bOutRootNode)
	{
		if (bOutRootNode) {
			// Transform
			float[] fLclTranslation = new float[3];
			fLclTranslation [0] = -0.0f;
			fLclTranslation [1] = 0.0f;
			fLclTranslation [2] = 0.0f;
			float[] fLclRotation = new float[4];
			Quaternion qRot = Quaternion.Euler (0.0f, 0.0f, 0.0f);
			fLclRotation [0] = -qRot.x;
			fLclRotation [1] = qRot.y;
			fLclRotation [2] = qRot.z;
			fLclRotation [3] = -qRot.w;
			float[] fLclScale = new float[3];
			fLclScale [0] = 1.0f;
			fLclScale [1] = 1.0f;
			fLclScale [2] = 1.0f;
			FBXExporterSetNode ("", strOutRootNodeName, fLclTranslation, fLclRotation, fLclScale);
		}
		else {
			strOutRootNodeName = "";
		}
	}

    void ExportSetVetexIndex(string strParentNodeName, GameObject goChild, bool bMesh)
    {
		// Mesh
		if (bMesh)
		{
			SkinnedMeshRenderer meshSkin = goChild.GetComponent<SkinnedMeshRenderer>();
            MeshFilter meshF = goChild.GetComponent<MeshFilter>();
            if (meshSkin || meshF)
            {
				Mesh mesh = null;
                if (meshSkin)
                {
                	mesh = meshSkin.sharedMesh;
				}
                else
                {
                    mesh = meshF.mesh;
				}
                if (mesh)
                {
					// Vertex
                    float[] fVertex = new float[mesh.vertices.Length * 3];
                    Vector3[] vertices = mesh.vertices;
                    int i = 0;
					foreach (Vector3 vertex in vertices)
                    {
						fVertex[i + 0] = -vertex.x * fGlobalScale;
						fVertex[i + 1] = vertex.y * fGlobalScale;
						fVertex[i + 2] = vertex.z * fGlobalScale;
                        i = i + 3;
                    }

                    // Normal
                    float[] fNormal = new float[mesh.vertices.Length * 3];
                    Vector3[] normals = mesh.normals;
                    i = 0;
                    foreach (Vector3 nomral in normals)
                    {
						fNormal[i + 0] = -nomral.x;
						fNormal[i + 1] = nomral.y;
						fNormal[i + 2] = nomral.z;
                        i = i + 3;
                    }

                    // UV
                    float[] fUV = new float[mesh.vertices.Length * 2];
                    Vector2[] uvs = mesh.uv;
                    i = 0;
                    foreach (Vector3 uv in uvs)
                    {
                        fUV[i + 0] = uv.x;
                        fUV[i + 1] = uv.y;
                        i = i + 2;
                    }

                    // Index
                    int iIndexCount = 0;
                    for (i = 0; i < mesh.subMeshCount; i++)
                    {
                        int[] iVIndexss = mesh.GetTriangles(i);
                        iIndexCount += iVIndexss.Length;
                    }
                    int[] iVIndexs = new int[iIndexCount * 3];
                    int[] iVIndexIDs = new int[iIndexCount];
                    int k = 0;
                    int m = 0;
                    for (i = 0; i < mesh.subMeshCount; i++)
                    {
                        int[] iVIndexss = mesh.GetTriangles(i);
                        for (int j = 0; j < (iVIndexss.Length / 3); j++)
                        {
                            iVIndexs[k + (j * 3) + 0] = iVIndexss[(j * 3) + 0];
                            iVIndexs[k + (j * 3) + 1] = iVIndexss[(j * 3) + 2];
                            iVIndexs[k + (j * 3) + 2] = iVIndexss[(j * 3) + 1];
                            // MaterialID
                            iVIndexIDs[m + j] = i;
                        }
                        k = k + iVIndexss.Length;
                        m = m + iVIndexss.Length / 3;
                    }

                    // Transform
                    float[] fLclTranslation = new float[3];
					fLclTranslation[0] = -goChild.transform.localPosition.x * fGlobalScale;
					fLclTranslation[1] = goChild.transform.localPosition.y * fGlobalScale;
					fLclTranslation[2] = goChild.transform.localPosition.z * fGlobalScale;
					float[] fLclRotation = new float[4];
					fLclRotation[0] = -goChild.transform.localRotation.x;
					fLclRotation[1] = goChild.transform.localRotation.y;
					fLclRotation[2] = goChild.transform.localRotation.z;
					fLclRotation[3] = -goChild.transform.localRotation.w;
					float[] fLclScale = new float[3];
					fLclScale[0] = goChild.transform.localScale.x;
					fLclScale[1] = goChild.transform.localScale.y;
					fLclScale[2] = goChild.transform.localScale.z;
					FBXExporterSetNodeMesh(strParentNodeName, goChild.name, fVertex, fNormal, fUV, fVertex.Length, iVIndexs, iVIndexIDs, iVIndexs.Length, fLclTranslation, fLclRotation, fLclScale, bOutDirectControlPointsMode, bOutNormalSmoothing);

                    // Material
                    Renderer rendrMain = goChild.GetComponent<Renderer>();
                    if (rendrMain)
                    {
                        Material[] matMains;
                        if (meshSkin)
                        {
                            matMains = meshSkin.sharedMaterials;
                        }
                        else
                        {
                            matMains = goChild.GetComponent<Renderer>().materials;
                        }
                        for (i = 0; i < matMains.Length; i++)
                        {
                            Material material = matMains[i];
                            if (material)
                            {
                                float[] fAmbient = new float[3];
                                fAmbient[0] = 0.0f;
                                fAmbient[1] = 0.0f;
                                fAmbient[2] = 0.0f;
                                float[] fDiffuse = new float[3];
                                fDiffuse[0] = 1.0f;
                                fDiffuse[1] = 1.0f;
                                fDiffuse[2] = 1.0f;
                                float[] fEmissive = new float[3];
                                fEmissive[0] = 0.0f;
                                fEmissive[1] = 0.0f;
                                fEmissive[2] = 0.0f;
                                float[] fTransparency = new float[1];
                                fTransparency[0] = 0.0f;
                                if (material.HasProperty("_Color"))
                                {
                                    //Color colAmbient = material.GetColor("_Color");
                                    fAmbient[0] = 0.0f;//colAmbient.r;
                                    fAmbient[1] = 0.0f;//colAmbient.g;
                                    fAmbient[2] = 0.0f;//colAmbient.b;
                                    Color colDiffuse = material.GetColor("_Color");
                                    fDiffuse[0] = colDiffuse.r;
                                    fDiffuse[1] = colDiffuse.g;
                                    fDiffuse[2] = colDiffuse.b;
                                    //Color colEmissive = material.GetColor("_Color");
                                    fEmissive[0] = 0.0f;//colEmissive.r;
                                    fEmissive[1] = 0.0f;//colEmissive.g;
                                    fEmissive[2] = 0.0f;//colEmissive.b;
                                    fTransparency[0] = 0.0f;
                                }
								string strEmbedMediaPath = "";
								if (bImportEmbedMediaTexturePath)
								{
									strEmbedMediaPath = strFilename.Replace(".fbx", "").Replace(".FBX", "") + ".fbm/";
								}
								string strDestPathDiffuseTex = "";
                                if (material.HasProperty("_MainTex"))
                                {
								    Texture2D tex2dMain = material.GetTexture("_MainTex") as Texture2D;
                                    if (tex2dMain != null)
                                    {
                                        string strDiffuseTex = tex2dMain.name;
                                        if (strDiffuseTex != "")
                                        {
                                            if (!dicDiffuseTexs.TryGetValue(strDiffuseTex, out strDestPathDiffuseTex))
                                            {
                                                string strAssetPathDiffuseTex = "";
                                                string strDestDiffuseTex = "";
                                                string strDestDiffuseTex2 = "";
                                                string strDataPath = "";
                                                switch (eImportTextrureMode)
                                                {
                                                    case EFBXImportTextrureMode.eLocalEmbedMedia:
                                                    case EFBXImportTextrureMode.eLocalProject:
                                                    case EFBXImportTextrureMode.eLocalStreamingAssets:
                                                        strDataPath = Application.dataPath.Replace("/Assets", "");
                                                        string[] strFilePaths = strDataPath.Split('/');
                                                        strDataPath = "";
                                                        int iDataPathLength = strFilePaths.Length;
                                                        if (strFilePaths[iDataPathLength - 1].LastIndexOf("_Data") > 0)
                                                        {
                                                            iDataPathLength = iDataPathLength - 1;
                                                        }
                                                        for (int j = 0; j < iDataPathLength; j++)
                                                        {
                                                            strDataPath += strFilePaths[j] + "/";
                                                        }
                                                        break;
                                                }
                                                switch (eImportTextrureMode)
                                                {
                                                    case EFBXImportTextrureMode.eAutoImportAssets:
#if UNITY_EDITOR
                                                        strAssetPathDiffuseTex = AssetDatabase.GetAssetPath(tex2dMain);
                                                        string[] strAssetPathDiffuseTexs = strAssetPathDiffuseTex.Split('/');
                                                        string strAssetPathDiffuseTex2 = "";
                                                        for (int j = 0; j < strAssetPathDiffuseTexs.Length - 1; j++)
                                                        {
                                                            strAssetPathDiffuseTex2 += strAssetPathDiffuseTexs[j] + "/";
                                                        }
                                                        string[] strDestDiffuseTexs = strAssetPathDiffuseTexs[strAssetPathDiffuseTexs.Length - 1].Split('.');
                                                        strDestDiffuseTex = strDestDiffuseTexs[0];
                                                        strDestDiffuseTex2 = strAssetPathDiffuseTexs[strAssetPathDiffuseTexs.Length - 1];
#else 
												        strAssetPathDiffuseTex = strDataPath + strLocalEmbedMediaTexturePath + strEmbedMediaPath + strDiffuseTex;
												        strDestDiffuseTex = tex2dMain.name;
												        strDestDiffuseTex2 = tex2dMain.name;
#endif
                                                        break;
                                                    case EFBXImportTextrureMode.eLocalEmbedMedia:
                                                        strAssetPathDiffuseTex = strDataPath + strLocalEmbedMediaTexturePath + strEmbedMediaPath + strDiffuseTex;
                                                        strDestDiffuseTex = tex2dMain.name;
                                                        strDestDiffuseTex2 = tex2dMain.name;
                                                        break;
                                                    case EFBXImportTextrureMode.eLocalProject:
                                                        strAssetPathDiffuseTex = strDataPath + strLocalProjectTexturePath + strEmbedMediaPath + strDiffuseTex;
                                                        strDestDiffuseTex = tex2dMain.name;
                                                        strDestDiffuseTex2 = tex2dMain.name;
                                                        break;
                                                    case EFBXImportTextrureMode.eLocalStreamingAssets:
                                                        strAssetPathDiffuseTex = Application.streamingAssetsPath + "/" + strLocalStreamingAssetsTexturePath + strEmbedMediaPath + strDiffuseTex;
                                                        strDestDiffuseTex = tex2dMain.name;
                                                        strDestDiffuseTex2 = tex2dMain.name;
                                                        break;
                                                    case EFBXImportTextrureMode.eLocalAssets:
                                                        strAssetPathDiffuseTex = strDataPath + strLocalAssetsTexturePath + strDiffuseTex;
                                                        strDestDiffuseTex = tex2dMain.name;
                                                        strDestDiffuseTex2 = tex2dMain.name;
                                                        break;
                                                    case EFBXImportTextrureMode.eLocalFullPath:
                                                        strAssetPathDiffuseTex = strLocalFullPathTexturePath + strEmbedMediaPath + strDiffuseTex;
                                                        strDestDiffuseTex = tex2dMain.name;
                                                        strDestDiffuseTex2 = tex2dMain.name;
                                                        break;
                                                }
                                                string strOutPath = Directory.GetCurrentDirectory() + "/" + strOutputTexturePath + strEmbedMediaPath;
                                                if (!Directory.Exists(strOutPath))
                                                {
                                                    Directory.CreateDirectory(strOutPath);
                                                }
                                                if (bConvertAllPNG)
                                                {
#if UNITY_EDITOR
                                                    string assetPath = AssetDatabase.GetAssetPath(tex2dMain);
                                                    TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                                                    //if (importer.isReadable == false)
                                                    {
                                                        importer.textureType = TextureImporterType.Advanced;
                                                        importer.textureFormat = TextureImporterFormat.ARGB32;
                                                        importer.isReadable = true;
                                                        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
                                                    }
                                                    byte[] bytes = tex2dMain.EncodeToPNG();
                                                    strDestPathDiffuseTex = strOutputTexturePath + strEmbedMediaPath + strDestDiffuseTex;
                                                    bool bExists = bOverWriteTexture;
                                                    if (File.Exists(strDestPathDiffuseTex + ".png") && !bOverWriteTexture)
                                                    {
                                                        bExists = false;
                                                    }
                                                    if (bOutTexture && bExists)
                                                    {
                                                        if (bDebugLog) Debug.Log("Output PNG Texture:<" + strDestPathDiffuseTex + ".png>");
                                                        File.WriteAllBytes(strDestPathDiffuseTex + ".png", bytes);
                                                    }
                                                    strDestPathDiffuseTex = strOutputTexturePath + strEmbedMediaPath + strDestDiffuseTex;
#else
											        strDestPathDiffuseTex = strOutputTexturePath + strEmbedMediaPath + strDestDiffuseTex2;
											        if (bOutTexture) {
												        // strOutPath + strAssetPathDiffuseTexs[strAssetPathDiffuseTexs.Length - 1]
												        if (File.Exists(strAssetPathDiffuseTex + ".png"))
												        {
													        strAssetPathDiffuseTex += ".png";
													        strDestPathDiffuseTex += ".png";
												        }
												        else 
												        {
													        if (File.Exists(strAssetPathDiffuseTex + ".tga"))
													        {
														        strAssetPathDiffuseTex += ".tga";
														        strDestPathDiffuseTex += ".tga";
													        }
													        else
													        {
														        if (File.Exists(strAssetPathDiffuseTex + ".jpg"))
														        {
															        strAssetPathDiffuseTex += ".jpg";
															        strDestPathDiffuseTex += ".jpg";
														        }
													        }
												        }
												        if (bDebugLog) Debug.Log("Input Texture:<" + strAssetPathDiffuseTex + "> Output Texture:<" + strDestPathDiffuseTex + ">");
												        File.Copy(strAssetPathDiffuseTex, strDestPathDiffuseTex, bOverWriteTexture);
											        }
											        strDestPathDiffuseTex = strOutputTexturePath + strEmbedMediaPath + strDestDiffuseTex;
#endif
                                                }
                                                else
                                                {
                                                    strDestPathDiffuseTex = strOutputTexturePath + strEmbedMediaPath + strDestDiffuseTex2;
                                                    if (bOutTexture)
                                                    {
                                                        // strOutPath + strAssetPathDiffuseTexs[strAssetPathDiffuseTexs.Length - 1]
                                                        if (File.Exists(strAssetPathDiffuseTex + ".png"))
                                                        {
                                                            strAssetPathDiffuseTex += ".png";
                                                            strDestPathDiffuseTex += ".png";
                                                        }
                                                        else
                                                        {
                                                            if (File.Exists(strAssetPathDiffuseTex + ".tga"))
                                                            {
                                                                strAssetPathDiffuseTex += ".tga";
                                                                strDestPathDiffuseTex += ".tga";
                                                            }
                                                            else
                                                            {
                                                                if (File.Exists(strAssetPathDiffuseTex + ".jpg"))
                                                                {
                                                                    strAssetPathDiffuseTex += ".jpg";
                                                                    strDestPathDiffuseTex += ".jpg";
                                                                }
                                                            }
                                                        }
                                                        if (bDebugLog) Debug.Log("Input Texture:<" + strAssetPathDiffuseTex + "> Output Texture:<" + strDestPathDiffuseTex + ">");
                                                        File.Copy(strAssetPathDiffuseTex, strDestPathDiffuseTex, bOverWriteTexture);
                                                    }
                                                    strDestPathDiffuseTex = strOutputTexturePath + strEmbedMediaPath + strDestDiffuseTex;
                                                }
                                                dicDiffuseTexs.Add(strDiffuseTex, strDestPathDiffuseTex);
                                            }
                                        }
                                    }
                            	}
								FBXExporterAddMaterial(goChild.name, material.name, fAmbient, fDiffuse, fEmissive, fTransparency, strDestPathDiffuseTex);
							}
                        }
                    }

                    // Skin Mesh
                    if (meshSkin)
                    {
						if (meshSkin.bones.Length > 0)
						{
							for (i = 0; i < meshSkin.bones.Length; i++)
	                        {
								FBXExporterSetSkinBoneName(goChild.name, i, meshSkin.bones[i].name);
	                        }
							if (bOutDirectControlPointsMode)
							{
		                        int iBoneWeightCount = mesh.boneWeights.Length;
		                        BoneWeight[] boneweight = mesh.boneWeights;
		                        int[] iBoneIndex = new int[4 * iBoneWeightCount];
		                        float[] fBoneWeight = new float[4 * iBoneWeightCount];
								for (i = 0; i < iBoneWeightCount; i++)
		                        {
		                            iBoneIndex[(i * 4) + 0] = boneweight[i].boneIndex0;
		                            iBoneIndex[(i * 4) + 1] = boneweight[i].boneIndex1;
		                            iBoneIndex[(i * 4) + 2] = boneweight[i].boneIndex2;
		                            iBoneIndex[(i * 4) + 3] = boneweight[i].boneIndex3;
		                            fBoneWeight[(i * 4) + 0] = boneweight[i].weight0;
		                            fBoneWeight[(i * 4) + 1] = boneweight[i].weight1;
		                            fBoneWeight[(i * 4) + 2] = boneweight[i].weight2;
		                            fBoneWeight[(i * 4) + 3] = boneweight[i].weight3;
								}
								if (iBoneWeightCount > 0) {
									FBXExporterSetSkinBoneWeight(goChild.name, iBoneIndex, fBoneWeight, mesh.vertices.Length);
									FBXExporterSetBindPose(goChild.name);
								}
							}
							else
							{
								int iBoneWeightCount = iVIndexs.Length;
								BoneWeight[] boneweight = mesh.boneWeights;
								int[] iBoneIndex = new int[4 * iBoneWeightCount];
								float[] fBoneWeight = new float[4 * iBoneWeightCount];
								for (i = 0; i < iBoneWeightCount; i++)
								{
									int index = iVIndexs[i];
									iBoneIndex[(i * 4) + 0] = boneweight[index].boneIndex0;
									iBoneIndex[(i * 4) + 1] = boneweight[index].boneIndex1;
									iBoneIndex[(i * 4) + 2] = boneweight[index].boneIndex2;
									iBoneIndex[(i * 4) + 3] = boneweight[index].boneIndex3;
									fBoneWeight[(i * 4) + 0] = boneweight[index].weight0;
									fBoneWeight[(i * 4) + 1] = boneweight[index].weight1;
									fBoneWeight[(i * 4) + 2] = boneweight[index].weight2;
									fBoneWeight[(i * 4) + 3] = boneweight[index].weight3;
								}
								if (iBoneWeightCount > 0) {
									FBXExporterSetSkinBoneWeight(goChild.name, iBoneIndex, fBoneWeight, iBoneWeightCount);
									FBXExporterSetBindPose(goChild.name);
								}
							}
						}
					}
				}
			}
		}
		else
		{
			// Transform
			float[] fLclTranslation = new float[3];
			fLclTranslation[0] = -goChild.transform.localPosition.x * fGlobalScale;
			fLclTranslation[1] = goChild.transform.localPosition.y * fGlobalScale;
			fLclTranslation[2] = goChild.transform.localPosition.z * fGlobalScale;
			float[] fLclRotation = new float[4];
			fLclRotation[0] = -goChild.transform.localRotation.x;
			fLclRotation[1] = goChild.transform.localRotation.y;
			fLclRotation[2] = goChild.transform.localRotation.z;
			fLclRotation[3] = -goChild.transform.localRotation.w;
			float[] fLclScale = new float[3];
			fLclScale[0] = goChild.transform.localScale.x;
			fLclScale[1] = goChild.transform.localScale.y;
			fLclScale[2] = goChild.transform.localScale.z;
			FBXExporterSetNode(strParentNodeName, goChild.name, fLclTranslation, fLclRotation, fLclScale);
		}
	}
}

public static class TransformExtension{
	public static GameObject[] FindRootObject (this Transform transform) {
		return Array.FindAll (GameObject.FindObjectsOfType<GameObject> (), (item) => item.transform.parent == null);
	}
}
