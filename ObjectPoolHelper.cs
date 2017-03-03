using UnityEngine;
using System.Collections;

namespace ProjectF
{
	public class ObjectPoolHelper : MonoBehaviour
	{
		public string poolKey;
		
		public void Release()
		{
			if( string.IsNullOrEmpty(poolKey) == false )
			{
				ObjectPool.instance.Release (poolKey, gameObject);
			}
			else
			{
				Destroy (gameObject);
			}
		}

		public void Release(float time)
		{
			Invoke ("Release", time);
		}
	}
}
