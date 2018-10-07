using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace M22
{
    namespace CustomFunctions
    {
        public class SakuraEffectObjectScript : MonoBehaviour
        {
            bool stopping = false;

            Material particleMat;

            const float stopSpeed = 0.5f;

            void Start()
            {
                particleMat = /*GetComponent<ParticleSystem>().*/GetComponent<Renderer>().material;
            }

            void Update()
            {
                if (stopping == true)
                {
                    if (particleMat.GetColor("_TintColor").a > 0)//particleMat.color.a > 0)
                    {
                        particleMat.SetColor(
                            "_TintColor",
                            new Color(1, 1, 1,
                                particleMat.GetColor("_TintColor").a - (stopSpeed * Time.deltaTime)
                            )
                        );
                    }
                    else
                    {
                        Destroy(this.gameObject);
                        return;
                    }
                }
            }

            public void Stop()
            {
                stopping = true;
            }
        }
    }
}