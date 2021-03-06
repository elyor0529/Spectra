﻿using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

namespace NoiseBall
{
    [ExecuteInEditMode]
    public class NoiseBallRenderer : MonoBehaviour
    {
        #region Exposed Parameters

        [SerializeField]
        NoiseBallMesh _mesh;

        [Space]
        [SerializeField]
        float _radius = 0.6f;

        [SerializeField]
        float _noiseAmplitude = 0.05f;

        [SerializeField]
        float _noiseFrequency = 1.0f;

        [SerializeField]
        float _noiseMotion = 0.2f;

        [Space]
        [SerializeField, ColorUsage(false, true, 0, 8, 0.125f, 3)]
        Color _lineColor = Color.white;

        [SerializeField, ColorUsage(false)]
        Color _surfaceColor = Color.white;

        [SerializeField, Range(0, 1)]
        float _metallic = 0.5f;

        [SerializeField, Range(0, 1)]
        float _smoothness = 0.5f;

        [Space]
        [SerializeField]
        ShadowCastingMode _castShadows;

        [SerializeField]
        bool _receiveShadows;

        #endregion

        #region Private Resources

        [SerializeField, HideInInspector]
        Shader _surfaceShader;

        [SerializeField, HideInInspector]
        Shader _lineShader;

        #endregion

        #region Private Variables

        Material _surfaceMaterial;
        Material _lineMaterial;
        MaterialPropertyBlock _materialProperties;
        Vector3 _noiseOffset;
		float radiusOffset = 0;
		float polygonOffset = 0;


        #endregion
		public static Color currColor;
        public static Color barColor;
        #region MonoBehaviour Functions

        void Start()
        {
            Update();
            StartCoroutine("ColorShift");
			StartCoroutine ("RadiusShift");
			StartCoroutine ("PolygonShift");
        }

        void Update()
        {
            

            if (_surfaceMaterial == null)
            {
                _surfaceMaterial = new Material(_surfaceShader);
                _surfaceMaterial.hideFlags = HideFlags.DontSave;
            }

            if (_lineMaterial == null)
            {
                _lineMaterial = new Material(_lineShader);
                _lineMaterial.hideFlags = HideFlags.DontSave;
            }

            if (_materialProperties == null)
                _materialProperties = new MaterialPropertyBlock();


            _noiseOffset += new Vector3(0.13f, 0.82f, 0.11f) * _noiseMotion * Time.deltaTime;


            _surfaceMaterial.color = _surfaceColor;
            _lineMaterial.color = _lineColor;
			_lineMaterial.SetFloat("_Radius", _radius * 1.05f + 1 *radiusOffset);

            _surfaceMaterial.SetFloat("_Metallic", _metallic);
            _surfaceMaterial.SetFloat("_Glossiness", _smoothness);
			_surfaceMaterial.SetFloat("_Radius", _radius + 1 * polygonOffset);

            _materialProperties.SetFloat("_NoiseAmplitude", _noiseAmplitude);
            _materialProperties.SetFloat("_NoiseFrequency", _noiseFrequency);
            _materialProperties.SetVector("_NoiseOffset", _noiseOffset);

            Graphics.DrawMesh(
                _mesh.sharedMesh, transform.localToWorldMatrix,
                _surfaceMaterial, 0, null, 0, _materialProperties,
                _castShadows, _receiveShadows, transform
            );


            Graphics.DrawMesh(
                _mesh.sharedMesh, transform.localToWorldMatrix,
                _lineMaterial, 0, null, 1, _materialProperties,
                _castShadows, _receiveShadows, transform
            );
        }

		/*These are written by Marcus, Ludwig and Jonathan for project Spectra*/
        IEnumerator ColorShift()
        {
            float h = 0f;
            float h_bar = 0f;
            float s = 0f;
            float v = 0f;
            while (true)
            {
				currColor = Color.HSVToRGB(h, 1, SpectraCS.currentLow * 0.5f + 0.2f, true);
                barColor = Color.HSVToRGB(h_bar, 1, SpectraCS.currentLow * 0.5f + 0.2f, true);
				_surfaceMaterial.color = currColor;
                h += 0.005f;
                h = h % 1;

                h_bar += 0.001f;
                h_bar = h_bar % 1;

                s += 0.0001f;
                v += 0.0002f;
                yield return null;
            }
        }

		IEnumerator RadiusShift()
		{
			while (true) 
			{
				if (SpectraCS.currentMiddle > radiusOffset) {
					radiusOffset = SpectraCS.currentMiddle;
				} else {
					if (radiusOffset > 0) {
						radiusOffset -= 0.012f;
					}
				}
				yield return null;
			}
		}

		IEnumerator PolygonShift()
		{
			while (true) 
			{
				
				if (SpectraCS.currentHigh > polygonOffset  && Mathf.Abs(polygonOffset - SpectraCS.currentHigh) > 0.1f) {
					if (SpectraCS.currentHigh > radiusOffset) 
					{
						polygonOffset = radiusOffset;
					} else 
					{
						polygonOffset = SpectraCS.currentHigh;
					}
				} 
				else 
				{
					if (polygonOffset > 0) 
					{
						polygonOffset -= 0.02f;
					}
				}

                yield return null;
			}
		}
			

        #endregion
    }


}

