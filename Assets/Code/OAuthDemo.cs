using UnityEngine;
using System.Collections;

public class OAuthDemo : MonoBehaviour {

	public string Url;
	iWebview webView;
	System.Action<string> messageTarget; 
	
	void Start()
	{
		//Create a callback handler
		messageTarget = locCB;
		//instantiate the interface
		webView = (new GameObject("iWebview")).AddComponent<iWebview>();
		//set the callbac  JavaCB, NavigateCB
		webView.Init(null, messageTarget);
		//Open the web page
		webView.LoadURL(Url);
		webView.SetVisibility(true);
	}
	
	void locCB(string cbMsg)
	{
		//For the Facebook API we're specifically looking for the access token granted
		//when the user logs in.  Twitter and google both use a similar convention
		if(cbMsg.Contains("#access_token="))
		{
			//Here is where you parse the token and close down the web view
			//you generally don't show the success or fail redirect page 
			
			webView.LoadURL("http://www.facebook.com");
			//Destroy(webView);
		}
	}
}
