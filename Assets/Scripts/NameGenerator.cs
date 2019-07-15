using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameGenerator {
    private static string[] firstParts = new string[]{
        "Gnarly",
        "Wicked",
        "Epic",
        "Tubular",
        "Hangry",
        "Stumpy",
        "Jelly",
        "Shark",
        "Bob",
        "Timid",
    };
    private static string[] secondParts = new string[]{
        "Toeball",
        "Grind",
        "Heelfoot",
        "Tendrils",
        "Splash",
        "Nosedive",
        "Surfsocks",
        "Sunburn",
    };

    public static string Name() {
        string name = firstParts[Random.Range(0, firstParts.Length - 1)];
        name += " ";
        if (Random.value < 0.1) {
            name += "Mc";
        }
        name += secondParts[Random.Range(0, secondParts.Length - 1)];
        return name;
    }
}
