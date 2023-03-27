using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

namespace IroSphere
{
	[CreateAssetMenu(menuName = "SaveData", fileName = "SaveData")]
	public class SaveData : ScriptableObject
	{
		[SerializeField]
		Vector3[] positions;

		public Vector3[] Position { get { return positions; } set { positions = value; } }

	}
}