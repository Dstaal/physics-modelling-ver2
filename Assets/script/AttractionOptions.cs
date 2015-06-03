using UnityEngine;
using UnityEngine.UI;

public enum attractionSelectionStage
{
    None,
    selecting,
    finalizing,
}

public class AttractionOptions : MonoBehaviour
{
    public MyMenuController myMenu;
    public SpringOptions springObt;

    public GameObject AttracionOptionsCanvesPrefab;
    public GameObject addAttractionBntFeild;

    public Slider strSlider;
    public Slider minDistSlider;

    public GameObject targetOneField;
    public GameObject targetTwoField;

    public GameObject strDisplay;
    public GameObject minDisDisplay;

    public bool bAttractionObt = false;

    private float attStr = 1;
    private float attMinDist = 1;

    private bool finalizeAttraction = false;

    public attractionSelectionStage currentSelectionStage = attractionSelectionStage.None;

    // Use this for initialization
    private void Start()
    {
        myMenu = this.gameObject.GetComponent<MyMenuController>();
        springObt = this.gameObject.GetComponent<SpringOptions>();
    }

    // Update is called once per frame
    private void Update()
    {
        //--- stages for spring selection -----

        if (this.currentSelectionStage == attractionSelectionStage.selecting)
        {
            if (myMenu.targetOne != null)
            {
                // wait for the target two to be seleced.
                if (Input.GetMouseButtonDown(0))
                {
                    myMenu.targetTwo = myMenu.GetParticleAtPos();
                    Debug.Log("trying to set targetTwo");
                    targetTwoField.GetComponent<Text>().text = myMenu.targetTwo.name.ToString();
                    myMenu.targetTwo.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                    this.currentSelectionStage = attractionSelectionStage.finalizing;
                }
            }
        }
        if (this.currentSelectionStage == attractionSelectionStage.finalizing)
        {
            addAttractionBntFeild.GetComponent<Text>().text = ("Finalize attraction");
            finalizeAttraction = true;
        }
    }

    public void clickCreateAttraction()
    {
        if (myMenu.particle != null && finalizeAttraction == false)
        {
            //set the seleted particle as target one
            myMenu.targetOne = myMenu.particle;
            targetOneField.GetComponent<Text>().text = myMenu.targetOne.name.ToString();
            myMenu.targetOne.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            currentSelectionStage = attractionSelectionStage.selecting;

            // the rest of the selection, is handin in the update -
        }

        //if two targets create spring
        if (finalizeAttraction == true && myMenu.targetOne != null && myMenu.targetTwo != null)
        {
            attStr = strSlider.value;
            attMinDist = minDistSlider.value;

            addNewAttraction(myMenu.myParticleSystem, myMenu.targetOne, myMenu.targetTwo, attMinDist, attStr);

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
            addAttractionBntFeild.GetComponent<Text>().text = ("Create attracion");
            targetOneField.GetComponent<Text>().text = (" ");
            targetTwoField.GetComponent<Text>().text = (" ");

            this.currentSelectionStage = attractionSelectionStage.None;
            finalizeAttraction = false;
        }
    }

    public void attSliderOnChange()
    {
        strDisplay.GetComponent<Text>().text = strSlider.value.ToString();
        minDisDisplay.GetComponent<Text>().text = minDistSlider.value.ToString();
    }

    public void toggleAttractionOpt()
    {
        if (bAttractionObt == false)
        {
            this.AttracionOptionsCanvesPrefab.SetActive(true);
            if (springObt.bSpringObt == true)
            {
                springObt.SpringsOptionsCanvesPrefab.SetActive(false);
                springObt.bSpringObt = false;
            }

            bAttractionObt = true;
        }
        else if (bAttractionObt == true)
        {
            this.AttracionOptionsCanvesPrefab.SetActive(false);
            bAttractionObt = false;
        }
    }

    private MyAttraction addNewAttraction(MyParticleSystem _particleSystem, MyParticle targetOne, MyParticle targetTwo, float attMinDist, float attStr)
    {
        return new MyAttraction(_particleSystem, targetOne, targetTwo, attMinDist, attStr);
    }
}