using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace ProjectF
{
	public class GlobalMain : MonoBehaviour
	{
		static GlobalMain instance;

		Stack<Action> stk_escape = new Stack<Action> ();

		void Awake()
		{
			if( instance != null )
			{
				Destroy (gameObject);
				return;
			}
			
			instance = this;

			DontDestroyOnLoad (gameObject);

			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Screen.SetResolution (720, 1280, true);

			GoogleManager.Init (false);

			Load ();
		}

		void Start()
		{
			FontManager.instance.LoadFont (UserSetting.info.language);
		}

		void OnLevelWasLoaded(int level)
		{
			stk_escape.Clear ();

			ObjectPool.instance.AllClear ();

//			MessageBox.UnRegisterCallback ();
			FontManager.instance.LoadFont (UserSetting.info.language);
		}
		
		void OnApplicationPause(bool pause)
		{
			if( pause )
			{
//				Save ();

				var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene ();
				if( scene.name == GlobalDefine.SceneName.Battle )
					OpenMessagebox ();
			}
		}

		void OnApplicationQuit()
		{
//			OpenMessagebox ();
//			this.Save ();
		}

		void Save()
		{
			if( Player.instance != null && Player.instance.playerInfo != null )
				Server.instance.SetPlayerInfo (Player.instance.playerInfo);
			
//			UserSetting.WriteToFile ();
		}

		void Load()
		{
			UserSetting.LoadFromFile ();

			TableManager.Load ();

			if( Application.platform != RuntimePlatform.Android &&
				Application.platform != RuntimePlatform.IPhonePlayer )
			{
				var playerInfo = Server.instance.GetPlayerInfo ();
				Player.instance.SetPlayerInfo (playerInfo);	
			}
		}

		void OpenMessagebox()
		{
			EscapeActionPush (MessageBox.Close);

			MessageBox.OkCancel (Localization.Text("quit"), () => Application.Quit (), EscapeActionPop);
		}

		void Update()
		{
			if( Input.GetKeyDown(KeyCode.Escape) )
			{
				if ( stk_escape.Count > 0 )
				{
					var action = stk_escape.Peek();
					action ();
				}
				else
				{
					OpenMessagebox ();
				}	
			}
		}

		public static void EscapeActionPush(Action action)
		{
			instance.stk_escape.Push (action);
		}

		public static void EscapeActionPop()
		{
			if( instance.stk_escape.Count > 0 )
				instance.stk_escape.Pop ();
		}
	}
}
