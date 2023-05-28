using UnityEngine;
namespace Ghost
{
	public class AssetRotater : MonoBehaviour
	{
		[Header("Rotation")]
		public bool rotateChildren;

		[Header("Scaling")]
		public bool lockXAndZFactors;
		public bool scaleChildren;

		public Vector2 xScaleBounds = new Vector2(1, 1);
		public Vector2 yScaleBounds = new Vector2(1, 1);
		public Vector2 zScaleBounds = new Vector2(1, 1);

		private void OnValidate()
		{
			if (lockXAndZFactors)
				zScaleBounds = xScaleBounds;
		}

		[ContextMenu("Use Grass Preset")]
		private void GrassPreset()
		{
			rotateChildren = true;
			scaleChildren = true;
			lockXAndZFactors = true;
			xScaleBounds = new Vector2(0.75f, 1.25f);
			zScaleBounds = new Vector2(0.75f, 1.25f);
		}

		[ContextMenu("Modify Transforms")]
		private void ModifyTransforms()
		{
			foreach (Transform child in transform)
			{
				if (scaleChildren)
				{
					child.localScale = new Vector3(
						Random.Range(xScaleBounds.x, xScaleBounds.y),
						Random.Range(yScaleBounds.x, yScaleBounds.y),
						Random.Range(zScaleBounds.x, zScaleBounds.y));
				}

				if (rotateChildren)
					child.transform.rotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);
			}
		}
	}
}

