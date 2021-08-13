using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
public class CarLights : NetworkBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject frontLightsGb;
    private Material front_lightsMat;
    private Material rear_lightsMat;
    /*
    7 materiales:
        0: Color base coche
        1: Plastico
        2: Color Cristales
        3: Nose
        4: Ojos de angel si tiene
        5: Luces Delante
        6: Intermitentes
        7: Luces detras
    */
    void Start()
    {
        front_lightsMat = meshRenderer.materials[5];
        rear_lightsMat = meshRenderer.materials[7];
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!front_lightsMat.IsKeywordEnabled(GameConstants.EMISSION))
                {
                    frontLightsGb.SetActive(true);
                    front_lightsMat.EnableKeyword(GameConstants.EMISSION);
                    rear_lightsMat.EnableKeyword(GameConstants.EMISSION);
                }
                else
                {
                    frontLightsGb.SetActive(false);
                    front_lightsMat.DisableKeyword(GameConstants.EMISSION);
                    rear_lightsMat.DisableKeyword(GameConstants.EMISSION);
                }

            }
        }
    }
}
