#import <UIKit/UIKit.h>

#define kNAVCB  "NavigateCB"
#define kJavaCB "JS_Call"

extern UIViewController *UnityGetGLViewController();
extern "C" void PostUnityMessage(const char *, const char *, const char *);

@interface UnityWebView : NSObject<UIWebViewDelegate>
{
	UIWebView *webView;
	NSString *gameObjectName;
    NSString* access_token;
}
- (void)webViewDidFinishLoad:(UIWebView *)a_webView;
- (void)navigateCB:(NSURLRequest*)request;
@end

@implementation UnityWebView

- (id)initWithGameObjectName:(const char *)gameObjectName_
{
	self = [super init];

	UIView *view = UnityGetGLViewController().view;
	webView = [[UIWebView alloc] initWithFrame:view.frame];
	webView.delegate = self;
	webView.hidden = YES;
	[view addSubview:webView];
	gameObjectName = [[NSString stringWithUTF8String:gameObjectName_] retain];

	return self;
}

- (void)dealloc
{
	[webView removeFromSuperview];
	[webView release];
	[gameObjectName release];
	[super dealloc];
}

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType
{
	NSString *url = [[request URL] absoluteString];
	if ([url hasPrefix:@"unity:"]) {
		UnitySendMessage([gameObjectName UTF8String],
			kJavaCB, [[url substringFromIndex:6] UTF8String]);
		return NO;
	} else {
		return YES;
	}
}

- (void)setMargins:(int)left top:(int)top right:(int)right bottom:(int)bottom
{
	UIView *view = UnityGetGLViewController().view;

	CGRect frame = view.frame;
	CGFloat scale = view.contentScaleFactor;
	frame.size.width -= (left + right) / scale;
	frame.size.height -= (top + bottom) / scale;
	frame.origin.x += left / scale;
	frame.origin.y += top / scale;
	webView.frame = frame;
}

- (void)setVisibility:(BOOL)visibility
{
	webView.hidden = visibility ? NO : YES;
}

- (void)loadURL:(const char *)url
{
	NSString *urlStr = [NSString stringWithUTF8String:url];
	NSURL *nsurl = [NSURL URLWithString:urlStr];
	NSURLRequest *request = [NSURLRequest requestWithURL:nsurl];
	[webView loadRequest:request];
}

- (void)evaluateJS:(const char *)js
{
	NSString *jsStr = [NSString stringWithUTF8String:js];
	[webView stringByEvaluatingJavaScriptFromString:jsStr];
}

- (void)webViewDidFinishLoad:(UIWebView *)a_webView
{
    [self navigateCB:[a_webView request]];
}

- (void)navigateCB:(NSURLRequest*)request
{
    NSURL* url = [request mainDocumentURL];
    NSString* absoluteString = [url absoluteString];
    access_token = absoluteString;
    UnitySendMessage([gameObjectName UTF8String],
                     kNAVCB , [access_token UTF8String]);
}

@end

extern "C" {
	void *UnityWebView_Init(const char *gameObjectName);
	void UnityWebView_Destroy(void *instance);
	void UnityWebView_SetMargins(void *instance, int left, int top, int right, int bottom);
	void UnityWebView_SetVisibility(void *instance, BOOL visibility);
	void UnityWebView_LoadURL(void *instance, const char *url);
	void UnityWebView_RunJS(void *instance, const char *url);
}

void *UnityWebView_Init(const char *gameObjectName)
{
	id instance = [[UnityWebView alloc] initWithGameObjectName:gameObjectName];
	return (void *)instance;
}

void UnityWebView_Destroy(void *instance)
{
	UnityWebView *webViewPlugin = (UnityWebView *)instance;
	[webViewPlugin release];
}

void UnityWebView_SetMargins(
	void *instance, int left, int top, int right, int bottom)
{
	UnityWebView *webViewPlugin = (UnityWebView *)instance;
	[webViewPlugin setMargins:left top:top right:right bottom:bottom];
}

void UnityWebView_SetVisibility(void *instance, BOOL visibility)
{
	UnityWebView *webViewPlugin = (UnityWebView *)instance;
	[webViewPlugin setVisibility:visibility];
}

void UnityWebView_LoadURL(void *instance, const char *url)
{
	UnityWebView *webViewPlugin = (UnityWebView *)instance;
	[webViewPlugin loadURL:url];
}

void UnityWebView_RunJS(void *instance, const char *js)
{
	UnityWebView *webViewPlugin = (UnityWebView *)instance;
	[webViewPlugin evaluateJS:js];
}
