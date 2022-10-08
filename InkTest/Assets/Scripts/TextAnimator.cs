using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextAnimator : MonoBehaviour
{
	
	public TMP_Text textComponent;
	

	//Effects =====================================================================
	//The various effects, and their parameters and relevant variables
	[Header("Waving")]
	[SerializeField] float WaveSize = 1f;
	[SerializeField] float WaveTime = 1f;
	[SerializeField] float XWeight = 1f; // in 1/1000ths
	private int[] waveArray;
	[Header("Rainbow")]
	[SerializeField] bool Rainbow = false;
	[SerializeField] float XColorWeight = 1f;
	[SerializeField] float TimeWeight = 1f;
	[SerializeField] Color referenceColor;
	private int[] rainbowArray;
	[Header("Shaking")]
	[SerializeField] float MaxShake = 1f;
	private int[] shakeArray;

	//Rich Text ======================================================================
	//https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichTextSupportedTags.html
	string[] richText = new string[]
	{
		"color",
		"alpha",
		"align",
		"allcaps",
		"b",
		"br",
		"cspace",
		"font",
		"font-weight",
		"gradient",
		"i",
		"indent",
		"line-height",
		"line-indent",
		"link",
		"lowercase",
		"margin",
		"mark",
		"mspace",
		"nobr",
		"noparse",
		"page",
		"pos",
		"rotate",
		"s",
		"size",
		"smallcaps",
		"space",
		"sprite",
		"strikethrough",
		"style",
		"sub",
		"sup",
		"u",
		"uppercase",
		"voffset",
		"width"
		
	};
	
	string[] customRichText = new string[]{
		"k",
		"w",
		"r"
	};
	
	
	
	
	
	void Start(){
		ReScan();
	}
    void Update()
	{
    	

	    textComponent.ForceMeshUpdate();
		var textInfo = textComponent.textInfo;
		
		
			
		
		EffectShake(textInfo);
		EffectWave(textInfo);
		EffectRainbow(textInfo);
		//EffectRainbow();
	    
	    for (int i =0; i<textInfo.meshInfo.Length;++i){
	    	var meshInfo = textInfo.meshInfo[i];
	    	meshInfo.mesh.vertices = meshInfo.vertices;
	    	textComponent.UpdateGeometry(meshInfo.mesh,i);
	    }
	}
	private void ReScan(){
		//need to remove all richtext; <color>, <alpha>, etc...
		string originalText = textComponent.text;
		char[] originalTextArray = originalText.ToCharArray();
		string scrubbedText = "";
		for(int i = 0; i<originalText.Length;i++){
			bool foundRichText = false;
			foreach(string n in richText){
				if(i+1+n.Length <= originalText.Length && originalText.Substring(i,1+n.Length).Equals("<" + n)){
					i = i + FindNext(originalText.Substring(i+1+n.Length),">") + 1+n.Length;
					foundRichText = true;
					break;
				}
			}
			
			if(!foundRichText)
				scrubbedText += originalTextArray[i];
		}
		//remove special characters to find actual list size
		string onlyCharacters = "";
		for(int i = 0; i<scrubbedText.Length;i++){
			bool foundRichText = false;
			foreach(string n in customRichText){
				if(i+1+n.Length <= scrubbedText.Length && scrubbedText.Substring(i,1+n.Length).Equals("<" + n)){
					i = i + FindNext(scrubbedText.Substring(i+1+n.Length),">") + 1+n.Length;
					foundRichText = true;
					break;
				}
			}
			
			if(!foundRichText)
				onlyCharacters += originalTextArray[i];
		}
		
		
		//Final One
		string finalText = "";
		for(int i = 0; i<originalText.Length;i++){
			bool foundRichText = false;
			foreach(string n in customRichText){
				if(i+1+n.Length <= originalText.Length && originalText.Substring(i,1+n.Length).Equals("<" + n)){
					i = i + FindNext(originalText.Substring(i+1+n.Length),">") + 1+n.Length;
					foundRichText = true;
					break;
				}
			}
			
			if(!foundRichText)
				finalText += originalTextArray[i];
		}
		
		textComponent.text = finalText;
		
		
		//Need to find my special rich text
		//Each effect will have its own separate handling hardcoded, as they have different needs
		
		//Shake Effect <k> =================================
		shakeArray = new int[onlyCharacters.Length];
		int charIndex = 0; //the "true" location of the characters 
		
		bool foundOutsideDelimiter = false;
		for(int i = 0; i<scrubbedText.Length;i++){
			if(i+2 <= scrubbedText.Length && scrubbedText.Substring(i,2).Equals("<k")){
				int start = FindNext(scrubbedText.Substring(i+2), ">") + 3 + i;
				int end = FindNext(scrubbedText.Substring(i+2), "<k>") + 1 + i;
				bool foundInsideDelimiter = false;
				for(int a = start; a<=end;a++){
					if(scrubbedText.ToCharArray()[a].Equals('>') || scrubbedText.ToCharArray()[a].Equals('<')){
						foundInsideDelimiter = !foundInsideDelimiter;
						continue;
					}
					if(foundInsideDelimiter)
						continue;
					shakeArray[charIndex] = 1;
					charIndex++;
					
				}
				i = end+3;
				continue;
			}
			if(scrubbedText.ToCharArray()[i].Equals('>') || scrubbedText.ToCharArray()[i].Equals('<')){
				foundOutsideDelimiter = !foundOutsideDelimiter;
				continue;
			}
			if(foundOutsideDelimiter)
				continue;
			charIndex++;
		}
		
		//Wave Effect <w> ===================================
		waveArray = new int[onlyCharacters.Length];
		charIndex = 0; //the "true" location of the characters
		
		foundOutsideDelimiter = false;
		for(int i = 0; i<scrubbedText.Length;i++){
			if(i+2 <= scrubbedText.Length && scrubbedText.Substring(i,2).Equals("<w")){
				int start = FindNext(scrubbedText.Substring(i+2), ">") + 3 + i;
				int end = FindNext(scrubbedText.Substring(i+2), "<w>") + 1 + i;
				bool foundInsideDelimiter = false;
				for(int a = start; a<=end;a++){
					if(scrubbedText.ToCharArray()[a].Equals('>') || scrubbedText.ToCharArray()[a].Equals('<')){
						foundInsideDelimiter = !foundInsideDelimiter;
						continue;
					}
					if(foundInsideDelimiter)
						continue;
					waveArray[charIndex] = 1;
					charIndex++;
				}
				i = end+3;
				continue;
			}
			if(scrubbedText.ToCharArray()[i].Equals('>') || scrubbedText.ToCharArray()[i].Equals('<')){
				foundOutsideDelimiter = !foundOutsideDelimiter;
				continue;
			}
			if(foundOutsideDelimiter)
				continue;
			charIndex++;
		}

		//Rainbow Effect <r> ========================================
		rainbowArray = new int[onlyCharacters.Length];
		charIndex = 0; //the "true" location of the characters
		
		foundOutsideDelimiter = false;
		for(int i = 0; i<scrubbedText.Length;i++){
			if(i+2 <= scrubbedText.Length && scrubbedText.Substring(i,2).Equals("<r")){
				int start = FindNext(scrubbedText.Substring(i+2), ">") + 3 + i;
				int end = FindNext(scrubbedText.Substring(i+2), "<r>") + 1 + i;
				bool foundInsideDelimiter = false;
				for(int a = start; a<=end;a++){
					if(scrubbedText.ToCharArray()[a].Equals('>') || scrubbedText.ToCharArray()[a].Equals('<')){
						foundInsideDelimiter = !foundInsideDelimiter;
						continue;
					}
					if(foundInsideDelimiter)
						continue;
					if(scrubbedText.ToCharArray()[a].Equals(" "))
						rainbowArray[charIndex] = -1;
					else
						rainbowArray[charIndex] = 1;
					charIndex++;
				}
				i = end+3;
				continue;
			}
			if(scrubbedText.ToCharArray()[i].Equals('>') || scrubbedText.ToCharArray()[i].Equals('<')){
				foundOutsideDelimiter = !foundOutsideDelimiter;
				continue;
			}
			if(foundOutsideDelimiter)
				continue;
			charIndex++;
		}
		
	}
	
	//ReScan Helper Functions =======================================================
	
	private int FindNext(string n, string x){
		return n.IndexOf(x);
	}
	
	//Effect Functions ===============================================================
	
	private void EffectWave(TMP_TextInfo textInfo){
		for(int i = 0; i<textInfo.characterCount; ++i){
			var charInfo = textInfo.characterInfo[i];
			if(!charInfo.isVisible){
				continue;
			}
			if(waveArray[i] == 0){
				continue;
			}	
			var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
			for(int j = 0; j<4; ++j){	    	
				var orig = verts[charInfo.vertexIndex + j];
				verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time*WaveTime + orig.x*XWeight*0.001f)*WaveSize*textComponent.fontSize,0);
			}
		}
	}
	private void EffectShake(TMP_TextInfo textInfo){
		for(int i = 0; i<textInfo.characterCount; ++i){
			var charInfo = textInfo.characterInfo[i];
			if(!charInfo.isVisible){
				continue;
			}
			if(shakeArray[i] == 0){
				continue;
			}			
			var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
			for(int j = 0; j<4; ++j){
				Vector3 shakeVector = new Vector3(Random.Range(0,MaxShake)*textComponent.fontSize*0.001f,Random.Range(0,MaxShake)*textComponent.fontSize*0.001f,0);
				var orig = verts[charInfo.vertexIndex + j];
				verts[charInfo.vertexIndex + j] = orig + shakeVector;
			}
		}
	}
	private void EffectRainbow(TMP_TextInfo textInfo){
		Vector3[] vertices = textComponent.mesh.vertices;
		Color[] colors = new Color[vertices.Length];
		
		for(int i = 0;i <colors.Length;i++){
			colors[i] = referenceColor;
		}
		
		int vertIndex = 0;
		for(int i = 0; i<textInfo.characterCount; ++i){
			var charInfo = textInfo.characterInfo[i];
			if(!charInfo.isVisible){
				continue;
			}
			if(rainbowArray[i] == 0){
				continue;
			}			
			var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
			for(int j = 0; j<4; ++j){
				float hue = Mathf.Lerp(0,1,0.5f*(Mathf.Sin(Time.time*TimeWeight + vertices[i].x*XColorWeight)+1));
				colors[vertIndex] = Color.HSVToRGB(hue, 1,1);
				vertIndex++;
			}
		}
		
		for (int i = 0; i < vertices.Length; i++){
			if(colors[i].Equals(referenceColor)){
				colors[i] = textComponent.mesh.colors[i];
			}
		}
		
		textComponent.mesh.colors = colors;
	}
	
	
	
	
	
	//UNUSED FUNCTIONS ==========================================================
	private void EffectRainbowLegacy(){
		Vector3[] vertices = textComponent.mesh.vertices;
		Color[] colors = new Color[vertices.Length];
		int currentChar = 0;
		for (int i = 0; i < vertices.Length; i++){
			Debug.Log(currentChar);
			if(i %4 == 0 && i != 0 )
				currentChar++;
			if(rainbowArray[currentChar] != 1){
				continue;
			}

			float hue = Mathf.Lerp(0,1,0.5f*(Mathf.Sin(Time.time*TimeWeight + vertices[i].x*XColorWeight)+1));
			colors[i] = Color.HSVToRGB(hue, 1,1);
			
		}
		textComponent.mesh.colors = colors;
	}

}
