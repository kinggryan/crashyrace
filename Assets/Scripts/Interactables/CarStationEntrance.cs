using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStationEntrance : CarAttachment {

    public CarAttachment useStation;

    public override void Use(GrapplerCharacterController character)
    {
        base.Use(character);

        // tell the character to enter the station we give it
        character.EnterStation(useStation);
    }
}
