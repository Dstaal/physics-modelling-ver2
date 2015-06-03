using UnityEngine;
using UnityEngine.UI;

public enum springSelectionStage
{
    None,
    selecting,
    finalizing,
}

public class SpringOptions : MonoBehaviour
{
    public MyMenuController myMenu;
    public AttractionOptions attOpt;

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

    public bool bSpringObt = false;

    private float springStr = 1;
    private float springRest = 1;
    private float springDamp = 1;

    private bool finalizeSpring = false;

    public springSelectionStage currentSelectionStage = springSelectionStage.None;

    // Use this for initialization
    private void Start()
    {
        myMenu = this.gameObject.GetComponent<MyMenuController>();
        attOpt = this.gameObject.GetComponent<AttractionOptions>();
    }

    // Update is called once per frame
    private void Update()
    {
        //--- stages for spring selection -----

        if (this.currentSelectionStage == springSelectionStage.selecting)
        {
            if (myMenu.targetOne != null)
            {
                // wait for the target two to be seleced.
                if (Input.GetMouseButtonDown(0))
                {
                    myMenu.targetTwo = myMenu.GetParticleAtPos();
                    Debug.Log("trying to set targetTwo");
                    targetTwoField.GetComponent<Text>().text = myMenu.targetTwo.name.ToString();
                    myMenu.targetTwo.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                    this.currentSelectionStage = springSelectionStage.finalizing;
                }
            }
        }
        if (this.currentSelectionStage == springSelectionStage.finalizing)
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
            springStr = strSlider.value;
            springRest = restSlider.value;
            springDamp = dampSlider.value;

            addNewSpring(myMenu.myParticleSystem, myMenu.targetOne, myMenu.targetTwo, springRest, springStr, springDamp);

            // change color unless they are pinne and thus red
            if (myMenu.targetOne.pinned == false)
            {
                myMenu.targetOne.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
            else if (myMenu.targetOne.pinned == true)
            {
                myMenu.targetOne.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }

            if (myMenu.targetTwo.pinned == false)
            {
                myMenu.targetTwo.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
            else if (myMenu.targetOne.pinned == true)
            {
                myMenu.targetTwo.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }

            //change the text on the bnt back to normal and eixt finalize stage
            addSpringBntFeild.GetComponent<Text>().text = ("Create spring");
            targetOneField.GetComponent<Text>().text = (" ");
            targetTwoField.GetComponent<Text>().text = (" ");

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
        if (bSpringObt == false)
        {
            this.SpringsOptionsCanvesPrefab.SetActive(true);

            if (attOpt.bAttractionObt == true)
            {
                attOpt.AttracionOptionsCanvesPrefab.SetActive(false);
                attOpt.bAttractionObt = false;
            }

            bSpringObt = true;
        }
        else if (bSpringObt == true)
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