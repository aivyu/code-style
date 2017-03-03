using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProjectF
{
	public class ObjectPool
	{
		static ObjectPool instance_;
		public static ObjectPool instance
		{
			get
			{
				if( instance_ == null )
				{
					instance_ = new ObjectPool ();
				}

				return instance_;
			}
		}

		private Dictionary<string, Queue<GameObject>> dic_pool = new Dictionary<string, Queue<GameObject>>();

		private Dictionary<string, GameObject> dic_prefab = new Dictionary<string, GameObject> ();

		private ObjectPool() {}

		public void AllClear()
		{
			dic_pool.Clear ();
			dic_prefab.Clear ();
		}

		public void Clear(string name)
		{
			if (dic_pool.ContainsKey (name))
				dic_pool [name].Clear ();

			if (dic_prefab.ContainsKey (name))
				dic_prefab.Remove (name);
		}

		public void Add(string name, GameObject prefab, int count)
		{
			if (dic_prefab.ContainsKey (name))
				return;
			
			dic_prefab.Add (name, prefab);
			
			Queue<GameObject> que = null;

			if( dic_pool.TryGetValue(name, out que) )
			{
				que.Clear ();
			}
			else
			{
				que = new Queue<GameObject> ();

				dic_pool.Add (name, que);
			}

			for( int i=0; i<count; ++i )
			{
				var obj = GameObject.Instantiate (prefab);
				var helper = obj.AddComponent<ObjectPoolHelper> ();
				helper.poolKey = name;
				que.Enqueue (obj);

				obj.SetActive (false);
			}
		}

		public GameObject Fetch(string name)
		{
			Queue<GameObject> que = null;

			if( dic_pool.TryGetValue(name, out que) )
			{
				GameObject obj = null;
				if( que.Count <= 0 && dic_prefab.ContainsKey(name) )
				{
					obj = GameObject.Instantiate (dic_prefab [name]);
					var helper = obj.AddComponent<ObjectPoolHelper> ();
					helper.poolKey = name;

					obj.SetActive (false);
				}
				else
				{
					obj = que.Dequeue();
				}
				
				return obj;
			}
			else
			{
				return null;
			}
		}

		public T Fetch<T>(string name) where T : Component
		{
			var obj = Fetch (name);

			if (obj != null)
				return obj.GetComponent<T> ();

			return null;
		}

		public void Release(string name, GameObject obj)
		{
			Queue<GameObject> que = null;

			obj.transform.SetParent(null);

			if( dic_pool.TryGetValue(name, out que) )
			{
				que.Enqueue (obj);
			}
			else
			{
				que = new Queue<GameObject> ();
				que.Enqueue (obj);
				dic_pool.Add (name, que);
			}

			obj.SetActive (false);
		}
	}
}
