package hl.maple.mathjax.client;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Toast;

public class Views extends Activity {
	
	private String doubleEscapeTeX(String s) {
		String t = "";
		for (int i = 0; i < s.length(); i++) {
			if (s.charAt(i) == '\'')
				t += '\\';
			if (s.charAt(i) != '\n')
				t += s.charAt(i);
			if (s.charAt(i) == '\\')
				t += "\\";
		}
		return t;
	}
	
	private static WebView w;
	private ProgressDialog mProgressDialog;
	private String strQuery = "";
	
	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.views);
		w = (WebView) findViewById(R.id.webview);
		
		Intent i = getIntent();
		if (i != null && i.getStringExtra("query") != "")
			strQuery = i.getStringExtra("query");
		else
			strQuery = "x=\\frac{-b\\pm\\sqrt{b^2-4ac}}{2a}";
		new CreateWebView().execute();
	}
	
	public static void clearWebView() {
		w.loadDataWithBaseURL(
				"http://bar",
				"<!DOCTYPE html><html><head><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0,maximum-scale=1.0, user-scalable=no\"> <script type='text/x-mathjax-config'>"
						+ "MathJax.Hub.Config({ "
						+ "showMathMenu: false, "
						+ "jax: ['input/TeX','output/HTML-CSS'], "
						+ "extensions: ['tex2jax.js'], "
						+ "TeX: { extensions: ['AMSmath.js','AMSsymbols.js',"
						+ "'noErrors.js','noUndefined.js', 'MathZoom.js'"
						+ "] } "
						+ "});</script>"
						+ "<script type='text/javascript' "
						+ "src=\"file:///android_asset/MathJax/MathJax.js\""
						+ "></script></head><body><span id='solve'></span></body></html>",
				"text/html", "utf-8", "");
	}
	
	private class CreateWebView extends AsyncTask<Void, Void, Void> {
		
		@Override
		protected void onPostExecute(Void result) {
			w.loadUrl("javascript:document.getElementById('math').innerHTML='\\\\["
					+ doubleEscapeTeX(strQuery) + "\\\\]';");
			
			w.setWebViewClient(new WebViewClient() {
				@Override
				public void onPageFinished(WebView view, String url) {
					w.loadUrl("javascript:document.getElementById('mmlout').innerHTML='';");
					w.loadUrl("javascript:document.getElementById('math').innerHTML='\\\\["
							+ doubleEscapeTeX(strQuery) + "\\\\]';");
					w.loadUrl("javascript:MathJax.Hub.Queue(['Typeset',MathJax.Hub]);");
				}
				
			});
			
			mProgressDialog.dismiss();
		}
		
		@Override
		protected void onPreExecute() {
			super.onPreExecute();
			mProgressDialog = new ProgressDialog(Views.this);
			mProgressDialog.setTitle("MathJax");
			mProgressDialog.setMessage("Waitting ... ");
			mProgressDialog.setIndeterminate(false);
			
			mProgressDialog.show();
		}
		
		@SuppressLint("SetJavaScriptEnabled")
		@Override
		protected Void doInBackground(Void... params) {
			LoadWebView();
			return null;
		}
	}
	
	void LoadWebView() {
		w.getSettings().setJavaScriptEnabled(true);
		w.getSettings().setBuiltInZoomControls(true);
		w.loadDataWithBaseURL(
				"http://bar/",
				"<script type='text/x-mathjax-config'>"
						+ "MathJax.Hub.Config({ "
						+ "showMathMenu: false, "
						+ "jax: ['input/TeX','output/HTML-CSS'], " // output/SVG
						+ "extensions: ['tex2jax.js','toMathML.js'], "
						+ "TeX: { extensions: ['AMSmath.js','AMSsymbols.js',"
						+ "'noErrors.js','noUndefined.js'] }, "
						// +"'SVG' : { blacker: 30, "
						// +"styles: { path: { 'shape-rendering': 'crispEdges' } } } "
						+ "});</script>"
						+ "<script type='text/javascript' "
						+ "src='file:///android_asset/MathJax/MathJax.js'"
						+ "></script>"
						+ "<script type='text/javascript'>getLiteralMML = function() {"
						+ "math=MathJax.Hub.getAllJax('math')[0];"
						// below, toMathML() rerurns literal MathML string
						+ "mml=math.root.toMathML(''); return mml;"
						+ "}; getEscapedMML = function() {"
						+ "math=MathJax.Hub.getAllJax('math')[0];"
						// below, toMathMLquote() applies &-escaping to
						// MathML
						// string input
						+ "mml=math.root.toMathMLquote(getLiteralMML()); return mml;}"
						+ "</script>"
						+ "<span id='math'></span><pre><span id='mmlout'></span></pre>",
				"text/html", "utf-8", "");
		w.addJavascriptInterface(new Object() {
			public void clipMML(String s) {
				// uses android.text.ClipboardManager for compatibility with
				// pre-Honeycomb
				// for HC or later, use android.content.ClipboardManager
				android.text.ClipboardManager clipboard = (android.text.ClipboardManager) getSystemService(Context.CLIPBOARD_SERVICE);
				// next 2 comment lines have HC or later code, can also try
				// newHtmlText()
				// ClipData clip =
				// ClipData.newPlainText("MJ MathML text",s);//,s);
				// clipboard.setPrimaryClip(clip);
				// literal MathML (in parameter s) placed on system
				// clipboard
				clipboard.setText(s);
				Toast.makeText(getApplicationContext(),
						"MathML copied to clipboard", Toast.LENGTH_SHORT)
						.show();
			}
		}, "injectedObject");
	}
}
