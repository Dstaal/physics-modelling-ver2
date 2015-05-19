using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum springSelectionStage
{
	None,
	selecting,
	finalizing,
}

public class SpringOptions : MonoBehaviour {

	public MyMenuController myMenu;

	public GameObject SpringsOptionsCanvesPrefab;
	public GameObject addSpringBntFeild;

	public Slider strSlider;
	public Slider dampSlider;
	public Slider restSlider;

	public GameObject targetOneField;
	public GameObject targetTwoField;
	public GameObject dampDisplay;
	public GameObject strDisplay;
	public GameObject restDisplay;


	private float springStr = 1;
	private float springRest = 1;
	private float springDamp = 1;

	private bool finalizeSpring = false;
	private bool bSpringObt = false;

	public springSelectionStage currentSelectionStage = springSelectionStage.None;

	// Use this for initialization
	void Start () {

		myMenu = this.gameObject.GetComponent<MyMenuController>();
	
	}

	// Update is called once per frame
	void Update () {


		//--- stages for spring selection -----
		
		
		if (this.currentSelectionStage == springSelectionStage.selecting)
		{
			if (myMenu.targetOne != null)
			{
				// wait for the target two to be seleced.
				if(Input.GetMouseButtonDown (0))
				{
					myMenu.targetTwo = myMenu.GetParticleAtPos ();
					Debug.Log("trying to set targetTwo");
					targetTwoField.GetComponent<Text>().text = myMenu.targetTwo.name.ToString();
					myMenu.targetTwo.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
					this.currentSelectionStage = springSelectionStage.finalizing;
				}
			}
		}
		if(this.currentSelectionStage == springSelectionStage.finalizing)
		{
			addSpringBntFeild.GetComponent<Text>().text = ("Finalize spring");
			finalizeSpring = true;
		}
	}


	public void clickCreateSpring()
	{
		
		if (myMenu.particle != null && finalizeSpring == false)
		{
			//set the seleted particle as target one
			myMenu.targetOne = myMenu.particle;
			targetOneField.GetComponent<Text>().text = myMenu.targetOne.name.ToString();
			myMenu.targetOne.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
			currentSelectionStage = springSelectionStage.selecting;
			
			// the rest of the selection, is handin in the update - 
		}
		
		//if two targets create spring
		if (finalizeSpring == true && myMenu.targetOne != null && myMenu.targetTwo != null)
		{
			Debug.Log("finalize was clicked");
			
			
			springStr = strSlider.value;
			springRest = restSlider.value;
			springDamp = dampSlider.value;
			
			Debug.Log( "str = " + springStr + "rest :" + springRest + "damp : " + springDamp);
			
			addNewSpring(myMenu.myParticleSystem, myMenu.targetOne, myMenu.targetTwo,springStr, springRest, springDamp);
			
			//Debug.Log(" spring added " + _particleSystem.springs);
			
			// change color unless they are pinne and thus red
			if(myMenu.targetOne.pinned == false)
			{
				myMenu.targetOne.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
			}
			if(myMenu.targetTwo.pinned == false)
			{
				myMenu.targetTwo.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
			}
			
			//change the text on the bnt back to normal and eixt finalize stage
			addSpringBntFeild.GetComponent<Text>().text = ("Create spring");
			this.currentSelectionStage = springSelectionStage.None;
			finalizeSpring = false;
		}
	}

	public void sliderOnChange()
	{

		dampDisplay.GetComponent<Text>().text = dampSlider.value.ToString();
		strDisplay.GetComponent<Text>().text = strSlider.value.ToString();
		restDisplay.GetComponent<Text>().text = restSlider.value.ToString();

	}

	public void toggleSpringOpt()
	{
	
		if(bSpringObt == false)
		{
			this.SpringsOptionsCanvesPrefab.SetActive(true);
			bSpringObt = true;	
		}

		else if(bSpringObt == true)
		{
			this.SpringsOptionsCanvesPrefab.SetActive(false);
			bSpringObt = false;
		}
	}

	private MySpring addNewSpring(MyParticleSystem _particleSystem, MyParticle targetOne, MyParticle targetTwo, float springRest, float springStr, float springDamp) 
	{
		return new MySpring(_particleSystem, targetOne, targetTwo, springRest, springStr, springDamp);
	}

}
