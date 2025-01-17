using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour {
    public static WordManager instance;

    readonly List<string> allWords = new List<string> {
        "Able", "Arch", "Back", "Beam", "Bell", "Bold", "Card", "Care", "Cast", "Chat", "Clip", "Club", "Code", "Cook", "Crop", "Dive", "Door", "Draw", "Drop", "Drum", "Earn", "East", "Edge", "Face", "Fall", "Basket", "Bottle", "Breeze", "Camera", "Canvas", "Circle", "Clever", "Closet", "Custom", "Danger", "Design", "Dotted", "Driver", "Effort", "Escape", "Farmer", "Forest", "Friend", "Frozen", "Garden", "Gather", "Global", "Hidden", "Impact", "Jacket", "Jump", "Knife", "Light", "Magic", "Night", "Ocean", "Power", "Quick", "River", "Stone", "Travel", "Vision", "Whale", "Xenon", "Young", "Zebra", "Apple", "Bright", "Cotton", "Desert", "Energy", "Flight", "Ground", "Hunter", "Island", "Jungle", "Kitten", "Lumber", "Market", "Nature", "Orange", "Player", "Rocket", "Shadow", "Turtle", "Utopia", "Victor", "Winter", "Yellow", "Zipper"
    };

    public List<string> wordBank = new List<string>();

    private void Start() {
        instance = this;
        wordBank.AddRange(allWords);
    }

    public string GetRandomWord() {
        int randomIndex = Random.Range(0, wordBank.Count);
        string randomWord = wordBank[randomIndex];
        wordBank.RemoveAt(randomIndex);
        if (wordBank.Count == 0) {
            wordBank.AddRange(allWords);
        }
        return randomWord;
    }

    public void AssignWord(IActivatables obj) {
        obj.AssignedWord = GetRandomWord();
        Debug.Log("Assigned word: " + obj.AssignedWord);
    }
}
