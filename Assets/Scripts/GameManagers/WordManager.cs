using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour {
    public static WordManager instance;

    readonly List<string> allWords = new List<string> {
        "Able", "Arch", "Back", "Beam", "Bell", "Bold", "Card", "Care", "Cast", "Chat", "Clip", "Club", "Code", "Cook", "Crop", "Dive", "Door", "Draw", "Drop", "Drum", "Earn", "East", "Edge", "Face", "Fall", "Basket", "Bottle", "Breeze", "Camera", "Canvas", "Circle", "Clever", "Closet", "Custom", "Danger", "Design", "Dotted", "Driver", "Effort", "Escape", "Farmer", "Forest", "Friend", "Frozen", "Garden", "Gather", "Global", "Hidden", "Impact", "Jacket", "Jump", "Knife", "Light", "Magic", "Night", "Ocean", "Power", "Quick", "River", "Stone", "Travel", "Vision", "Whale", "Xenon", "Young", "Zebra", "Apple", "Bright", "Cotton", "Desert", "Energy", "Flight", "Ground", "Hunter", "Island", "Jungle", "Kitten", "Lumber", "Market", "Nature", "Orange", "Player", "Rocket", "Shadow", "Turtle", "Utopia", "Victor", "Winter", "Yellow", "Zipper"
    };

    public List<string> wordBank = new List<string>();
    public List<string> usedWords = new List<string>();

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        wordBank.AddRange(allWords);
    }

    public string GetRandomWord() {
        string randomWord = null;
        List<string> potentialWords = wordBank.FindAll(word => !usedWords.Exists(usedWord => usedWord[0] == word[0]));

        if (potentialWords.Count > 0) {
            int randomIndex = Random.Range(0, potentialWords.Count);
            randomWord = potentialWords[randomIndex];
            wordBank.Remove(randomWord);
        }
        else {
            int randomIndex = Random.Range(0, wordBank.Count);
            randomWord = wordBank[randomIndex];
            wordBank.RemoveAt(randomIndex);
        }

        usedWords.Add(randomWord);

        if (wordBank.Count == 0) {
            wordBank.AddRange(allWords);
            usedWords.Clear();
        }

        return randomWord;
    }
    

    public void AssignWord(IActivatables obj) {
        obj.AssignedWord = GetRandomWord();
        Debug.Log("Assigned word: " + obj.AssignedWord);
    }
}
