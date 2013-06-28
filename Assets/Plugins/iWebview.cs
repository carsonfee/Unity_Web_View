using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//Declare a callback delegate
using Callback = System.Action<string>;

public class iWebview : MonoBehaviour {

	Callback JavaScriptcallback;
	Callback Navigatecallback;
	
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
	IntPtr webView;
	bool visibility;
	Rect rect;
	Texture2D texture;
	string inputString;
#elif UNITY_IPHONE
	IntPtr webView;
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_OSX
	[DllImport("WebView")]
	private static extern IntPtr UnityWebView_Init(
		string gameObject, int width, int height, bool ineditor);
	[DllImport("WebView")]
	private static extern int UnityWebView_Destroy(IntPtr instance);
	[DllImport("WebView")]
	private static extern void UnityWebView_SetRect(
		IntPtr instance, int width, int height);
	[DllImport("WebView")]
	private static extern void UnityWebView_SetVisibility(
		IntPtr instance, bool visibility);
	[DllImport("WebView")]
	private static extern void UnityWebView_LoadURL(
		IntPtr instance, string url);
	[DllImport("WebView")]
	private static extern void UnityWebView_RunJS(
		IntPtr instance, string url);
	[DllImport("WebView")]
	private static extern void UnityWebView_Update(IntPtr instance,
		int x, int y, float deltaY, bool down, bool press, bool release,
		bool keyPress, short keyCode, string keyChars, int textureId);
#elif UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern IntPtr UnityWebView_Init(string gameObject);
	[DllImport("__Internal")]
	private static extern int UnityWebView_Destroy(IntPtr instance);
	[DllImport("__Internal")]
	private static extern void UnityWebView_SetMargins(
		IntPtr instance, int left, int top, int right, int bottom);
	[DllImport("__Internal")]
	private static extern void UnityWebView_SetVisibility(
		IntPtr instance, bool visibility);
	[DllImport("__Internal")]
	private static extern void UnityWebView_LoadURL(
		IntPtr instance, string url);
	[DllImport("__Internal")]
	private static extern void UnityWebView_RunJS(
		IntPtr instance, string url);
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_OSX
	private void CreateTexture(int x, int y, int width, int height)
	{
		int w = 1;
		int h = 1;
		while (w < width)
			w <<= 1;
		while (h < height)
			h <<= 1;
		rect = new Rect(x, y, width, height);
		texture = new Texture2D(w, h, TextureFormat.ARGB32, false);
	}
#endif

	public void Init(Callback JScb = null, Callback Navigatecb = null)
	{
		JavaScriptcallback = JScb;
		Navigatecallback = Navigatecb;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		CreateTexture(0, 0, Screen.width, Screen.height);
		webView = UnityWebView_Init(name, Screen.width, Screen.height,
			Application.platform == RuntimePlatform.OSXEditor);
#elif UNITY_IPHONE
		webView = UnityWebView_Init(name);
#endif
	}

	void OnDestroy()
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		if (webView == IntPtr.Zero)
			return;
		UnityWebView_Destroy(webView);
#elif UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return;
		UnityWebView_Destroy(webView);
#endif
	}

	public void SetMargins(int left, int top, int right, int bottom)
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		if (webView == IntPtr.Zero)
			return;
		int width = Screen.width - (left + right);
		int height = Screen.height - (bottom + top);
		CreateTexture(left, bottom, width, height);
		UnityWebView_SetRect(webView, width, height);
#elif UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return;
		UnityWebView_SetMargins(webView, left, top, right, bottom);
#endif
	}

	public void SetVisibility(bool v)
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		if (webView == IntPtr.Zero)
			return;
		visibility = v;
		UnityWebView_SetVisibility(webView, v);
#elif UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return;
		UnityWebView_SetVisibility(webView, v);
#endif
	}

	public void LoadURL(string url)
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return;
		UnityWebView_LoadURL(webView, url);
#endif
	}

	public void EvaluateJS(string js)
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_IPHONE
		if (webView == IntPtr.Zero)
			return;
		UnityWebView_RunJS(webView, js);
#endif
	}

	public void JS_Call(string message)
	{
		if (JavaScriptcallback != null)
		{
			JavaScriptcallback(message);
		}
	}
	
	public void NavigateCB( string message )
	{
		if (Navigatecallback != null)
		{
			Navigatecallback(message);
		}
	}
}
