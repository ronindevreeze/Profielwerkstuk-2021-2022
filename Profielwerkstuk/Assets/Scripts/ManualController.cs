using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualController : IController {
    public List<float> predict(float[] inputs) {
        List<float> input = new List<float>(2);

        input.Add(Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1));
        input.Add(Input.GetAxis("Horizontal") / 2 + 0.5f);

        return input;
    }
}
