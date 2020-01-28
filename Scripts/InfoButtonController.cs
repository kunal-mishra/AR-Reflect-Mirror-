using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButtonController : MonoBehaviour
{
    private const string DIALOG_TEXT = "by Hendrik Pastunink\n"+
    "@hpastunink on Twitter"+
    "\n\n\n" +
    "Tap once to place mirror.\n" +
    "Hold with one finger to reposition mirror.\n" +
    "Pinch with two fingers to resize mirror.\n" +
    "\n\n\n\n\n" +
    "Libraries used:\n\n\n" +
    "ARCore\n\n" +
    "Copyright (c) 2017 Google Inc. All rights reserved.\n" +
    "More info: www.google.com/policies/privacy/partners/" +
    "\n\n\n" +
    "UnityNativeDialogPlugin\n\n" +
    "Licensed under:\n" +
    "The MIT License (MIT)\n\nCopyright (c) 2013 Koki Ibukuro\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";

    public void OnInfoButtonClick()
    {
        DialogManager.Instance.GetComponent<DialogManager>().ShowSubmitDialog("AR Mirror", DIALOG_TEXT, delegate (bool submit)
        {
            Debug.Log(submit);
        });
    }
}
