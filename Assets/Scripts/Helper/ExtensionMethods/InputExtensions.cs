using UnityEngine;
public static class InputExtensions
{
    public static bool GetNumberDown(out int number)
    {
        number = -1;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            number = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            number = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            number = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            number = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            number = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            number = 6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            number = 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            number = 8;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            number = 9;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            number = 0;
        }
        return number >= 0;
    }
}

