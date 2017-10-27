package hl.maple.mathjax.client;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;

public class Home extends Activity {
	private Button connect;
	private EditText ipAdress;
	
	@Override
	protected void onDestroy() {
		// TODO Auto-generated method stub
		super.onDestroy();
		System.exit(0);
	}
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		// TODO Auto-generated method stub
		super.onCreate(savedInstanceState);
		setContentView(R.layout.home);
		ipAdress = (EditText) findViewById(R.id.editText1);
		connect = (Button) findViewById(R.id.button1);
		
		connect.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View v) {
				// TODO Auto-generated method stub
				String ip = ipAdress.getText().toString();
				if (ip.equals("")) {
					// Client.SERVERIP = "192.168.121.1";
					Client.SERVERIP = "10.0.3.2";
				} else
					Client.SERVERIP = ip;
				//
				String strQuery = ipAdress.getText().toString();
				Intent intent = new Intent(getBaseContext(), MainActivity.class);
				Log.e("ServerIP", Client.SERVERIP);
				startActivity(intent);
				
				// Intent i = new Intent(getBaseContext(), Views.class);
				// if (!strQuery.equals(""))
				// i.putExtra("query", ipAdress.getText().toString());
				// else
				// i.putExtra("query", "x=\\frac{-b\\pm\\sqrt{b^2-4ac}}{2a}");
				//
				// startActivity(i);
			}
		});
	}
	
}
