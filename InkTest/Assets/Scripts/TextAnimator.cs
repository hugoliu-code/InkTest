using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextAnimator : MonoBehaviour
{

	public TMP_Text textComponent;
	[SerializeField] float WaveSize = 1f;
	[SerializeField] float WaveTime = 1f;
	[SerializeField] float XWeight = 1f; // in 1/1000ths
	
	[SerializeField] float XColorWeight = 1f;
	[SerializeField] float TimeWeight = 1f;
    // Update is called once per frame
    void Update()
	{
    	
		//Waving Text
	    textComponent.ForceMeshUpdate();
	    var textInfo = textComponent.textInfo;
	    
	    
	    for(int i = 0; i<textInfo.characterCount; ++i){
	    	var charInfo = textInfo.characterInfo[i];
	    	if(!charInfo.isVisible){
	    		continue;
	    	}
	    	
	    	var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
	    	for(int j = 0; j<4; ++j){	    	
	    		var orig = verts[charInfo.vertexIndex + j];
	    		verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time*WaveTime + orig.x*XWeight*0.001f)*WaveSize*textComponent.fontSize,0);
	    	}


	    	
	    }
	    
	    
	    
	    
		//Rainbow Effect
	    Vector3[] vertices = textComponent.mesh.vertices;
	    Color[] colors = new Color[vertices.Length];

	    for (int i = 0; i < vertices.Length; i++){
		    float hue = Mathf.Lerp(0,1,0.5f*(Mathf.Sin(Time.time*TimeWeight + vertices[i].x*XColorWeight)+1));
		    colors[i] = Color.HSVToRGB(hue, 1,1);
		    //colors[i] = Color.Lerp(Color.red, Color.green, Mathf.Sin(Time.time*TimeWeight + vertices[i].x*XColorWeight));
	    }
	    textComponent.mesh.colors = colors;
	    for (int i =0; i<textInfo.meshInfo.Length;++i){
	    	var meshInfo = textInfo.meshInfo[i];
	    	meshInfo.mesh.vertices = meshInfo.vertices;
	    	textComponent.UpdateGeometry(meshInfo.mesh,i);
	    }
    }
}
