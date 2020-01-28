using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomSheetController : MonoBehaviour
{

    private bool isOpen = false;

    private GameObject buttonText;

	private Animator animator;

    void Start()
    {
        buttonText = GameObject.Find("OpenBottomSheetPanelButtonText");
		animator = GetComponent<Animator>();
        GetComponentInChildren<ClickableSlider>().Toggle += delegate() {
            ToggleBottomSheet();
        };
    }

    public void ToggleBottomSheet()
    {
        if (isOpen)
        {
            isOpen = false;
            buttonText.GetComponent<Text>().text = ">";
			animator.SetBool("IsOpen", false);
        }
        else
        {
            isOpen = true;
            buttonText.GetComponent<Text>().text = "<";
			animator.SetBool("IsOpen", true);
        }
    }
}
